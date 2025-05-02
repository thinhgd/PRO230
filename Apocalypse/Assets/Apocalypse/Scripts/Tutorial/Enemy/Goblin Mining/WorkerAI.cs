using Game.Tutorials;
using Pathfinding;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AIPath), typeof(Seeker))]
public class WorkerAI : BaseEnemy
{
    [Header("Tree Settings")]
    public float chopRange = 1f;
    public float chopInterval = 2f;
    public float searchRadius = 10f;
    public LayerMask treeLayer;

    [Header("UI Elements")]
    public TextMeshProUGUI woodText;
    [Header("Wood Carry Settings")]
    public int woodCapacity = 10;
    private int carriedWood = 0;
    private bool isDepositingWood = false;

    private Tree targetTree;
    private AIPath aiPath;
    private float chopTimer = 2f;

    private Vector3 offsetPosition;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckCheckDelay = 1f;

    private TreeGroupManager groupManager;
    
    [Header("Wood Gain Panel")]
    public GameObject woodGainPanelPrefab;
    public Transform woodPanelSpawnParent;


    protected override void Start()
    {
        base.Start();
        aiPath = GetComponent<AIPath>();
        lastPosition = transform.position;
        UpdateWoodUI();
        InvokeRepeating(nameof(AutoFindNearestTree), 0f, 2f);
    }

    protected override void Update()
    {
        base.Update();
        if (currentState == EnemyState.Die) return;
        if (carriedWood > 0 || carriedWood < woodCapacity)
        {
            UpdateWoodUI();
        }
        UpdateSpriteDirection();
        if (isDepositingWood)
        {
            HandleDepositingWood();
            return;
        }

        if (targetTree == null || targetTree.currentHealth <= 0)
        {
            SetState(EnemyState.Idle);
            return;
        }

        float distToOffset = Vector2.Distance(transform.position, offsetPosition);
        float distToTree = Vector2.Distance(transform.position, targetTree.transform.position);

        if (distToTree <= chopRange)
        {
            aiPath.canMove = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("isRun", false);
            SetState(EnemyState.Attack);

            chopTimer += Time.deltaTime;
            if (chopTimer >= chopInterval)
            {
                if (carriedWood < woodCapacity)
                {
                    targetTree.Chop();
                    carriedWood++;
                    if (woodGainPanelPrefab != null && woodPanelSpawnParent != null)
                    {
                        Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
                        GameObject panel = Instantiate(woodGainPanelPrefab, woodPanelSpawnParent);
                        panel.transform.position = spawnPos;
                    }
                    if (carriedWood >= woodCapacity)
                    {
                        GoToDepositWood();
                        return;
                    }
                }

                chopTimer = 0f;
            }
        }
        else if (distToOffset > 0.2f)
        {
            aiPath.canMove = true;
            aiPath.destination = offsetPosition;
            SetState(EnemyState.Run);
        }
        else
        {
            if (Vector3.Distance(transform.position, lastPosition) < 0.05f)
            {
                stuckTimer += Time.deltaTime;

                if (stuckTimer >= stuckCheckDelay)
                {
                    Tree oldTarget = targetTree;
                    FindNearestTree(exclude: oldTarget);

                    if (targetTree != null && targetTree != oldTarget)
                    {
                        SetTargetWithOffset(targetTree);
                        aiPath.canMove = true;
                        SetState(EnemyState.Run);
                        stuckTimer = 0f;
                        return;
                    }

                    aiPath.canMove = false;
                    SetState(EnemyState.Idle);
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            lastPosition = transform.position;
        }

    }
    private void UpdateWoodUI()
    {
        if (woodText != null)
            woodText.text = "Wood: " + carriedWood.ToString() +"/" + woodCapacity.ToString();
    }

    private void HandleDepositingWood()
    {
        if (WoodStorage.Instance == null) return;

        aiPath.destination = WoodStorage.Instance.storagePoint.position;

        if (Vector2.Distance(transform.position, WoodStorage.Instance.storagePoint.position) < 0.5f)
        {
            CastleManager castle = FindObjectOfType<CastleManager>();
            if (castle != null)
            {
                castle.AddResource(ResourceType.Wood, carriedWood);
            }

            Tree.totalWood += carriedWood;
            carriedWood = 0;
            isDepositingWood = false;

            targetTree.InvokeChoppedEvent();
            AutoFindNearestTree();
            UpdateWoodUI();
        }
    }
    private void UpdateWood(int amount)
    {
        carriedWood += amount;
        if (carriedWood > woodCapacity) carriedWood = woodCapacity;
        UpdateWoodUI();
    }
    private void GoToDepositWood()
    {
        isDepositingWood = true;
        aiPath.canMove = true;
        aiPath.destination = WoodStorage.Instance.storagePoint.position;
        SetState(EnemyState.Run);
    }

    public void AutoFindNearestTree()
    {
        FindNearestTree();
    }

    public void FindNearestTree(Tree exclude = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, treeLayer);

        float closestDist = Mathf.Infinity;
        Tree closestTree = null;

        foreach (var hit in hits)
        {
            Tree tree = hit.GetComponent<Tree>();
            if (tree != null && tree.currentHealth > 0 && tree != exclude)
            {
                float dist = Vector3.Distance(transform.position, tree.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTree = tree;
                }
            }
        }

        if (closestTree != null)
        {
            SetTargetWithOffset(closestTree);
        }
    }

    private void SetTargetWithOffset(Tree tree)
    {
        if (groupManager != null)
        {
            groupManager.UnregisterWorker(this);
            groupManager = null;
        }

        targetTree = tree;

        groupManager = tree.GetComponent<TreeGroupManager>();
        if (groupManager == null)
            groupManager = tree.gameObject.AddComponent<TreeGroupManager>();

        groupManager.RegisterWorker(this);

        SetOffsetPosition(tree.transform.position + (Vector3)(Random.insideUnitCircle * 0.5f));
    }

    public void SetOffsetPosition(Vector3 pos)
    {
        offsetPosition = pos;

        if (aiPath != null)
        {
            aiPath.destination = offsetPosition;
        }
    }

    private void UpdateSpriteDirection()
    {
        if (aiPath == null || spriteRenderer == null) return;

        if (Mathf.Abs(aiPath.velocity.x) > 0.01f)
        {
            spriteRenderer.flipX = aiPath.velocity.x < 0f;
        }
    }


    protected override void Run()
    {
        base.Run();
        animator.SetBool("isRun", aiPath.velocity.magnitude > 0.1f);
    }

    protected override void Attack()
    {
        base.Attack();
    }
}
