using Game.Tutorials;
using Pathfinding;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AIPath), typeof(Seeker))]
public class MineAI : BaseEnemy
{
    [Header("Stone Settings")]
    public float chopRange = 1f;
    public float chopInterval = 2f;
    public float searchRadius = 10f;
    public LayerMask stoneLayer;

    [Header("UI Elements")]
    public TextMeshProUGUI woodText;
    [Header("Stone Carry Settings")]
    public int woodCapacity = 10;
    private int carriedStone = 0;
    private bool isDepositingWood = false;

    private Stone targetStone;
    private AIPath aiPath;
    private float chopTimer = 2f;

    private Vector3 offsetPosition;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckCheckDelay = 1f;

    private TreeGroupManager groupManager;

    [Header("Mine Gain Panel")]
    public GameObject mineGainPanelPrefab;
    public Transform minePanelSpawnParent;

    protected override void Start()
    {
        base.Start();
        aiPath = GetComponent<AIPath>();
        lastPosition = transform.position;
        UpdateWoodUI();
        InvokeRepeating(nameof(AutoFindNearestStone), 0f, 2f);
    }

    protected override void Update()
    {
        base.Update();
        if (currentState == EnemyState.Die) return;
        if (carriedStone > 0 || carriedStone < woodCapacity)
        {
            UpdateWoodUI();
        }
        UpdateSpriteDirection();
        if (isDepositingWood)
        {
            HandleDepositingWood();
            return;
        }

        if (targetStone == null || targetStone.currentHealth <= 0)
        {
            SetState(EnemyState.Idle);
            return;
        }

        float distToOffset = Vector2.Distance(transform.position, offsetPosition);
        float distToStone = Vector2.Distance(transform.position, targetStone.transform.position);

        if (distToStone <= chopRange)
        {
            aiPath.canMove = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("isRun", false);
            SetState(EnemyState.Attack);

            chopTimer += Time.deltaTime;
            if (chopTimer >= chopInterval)
            {
                if (carriedStone < woodCapacity)
                {
                    targetStone.Mine();
                    carriedStone++;
                    if (mineGainPanelPrefab != null && minePanelSpawnParent != null)
                    {
                        Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
                        GameObject panel = Instantiate(mineGainPanelPrefab, minePanelSpawnParent);
                        panel.transform.position = spawnPos;
                    }
                    if (carriedStone >= woodCapacity)
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
                    Stone oldTarget = targetStone;
                    FindNearestStone(exclude: oldTarget);

                    if (targetStone != null && targetStone != oldTarget)
                    {
                        SetTargetWithOffset(targetStone);
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
            woodText.text = "Stone: " + carriedStone.ToString() + "/" + woodCapacity.ToString();
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
                castle.AddResource(ResourceType.Stone, carriedStone);
            }

            Stone.totalStore += carriedStone;
            carriedStone = 0;
            isDepositingWood = false;

            targetStone.InvokeChoppedEvent();
            AutoFindNearestStone();
            UpdateWoodUI();
        }
    }
    private void UpdateWood(int amount)
    {
        carriedStone += amount;
        if (carriedStone > woodCapacity) carriedStone = woodCapacity;
        UpdateWoodUI();
    }
    private void GoToDepositWood()
    {
        isDepositingWood = true;
        aiPath.canMove = true;
        aiPath.destination = WoodStorage.Instance.storagePoint.position;
        SetState(EnemyState.Run);
    }

    public void AutoFindNearestStone()
    {
        FindNearestStone();
    }

    public void FindNearestStone(Stone exclude = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, stoneLayer);

        float closestDist = Mathf.Infinity;
        Stone closestStone = null;

        foreach (var hit in hits)
        {
            Stone stone = hit.GetComponent<Stone>();
            if (stone != null && stone.currentHealth > 0 && stone != exclude)
            {
                float dist = Vector3.Distance(transform.position, stone.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestStone = stone;
                }
            }
        }

        if (closestStone != null)
        {
            SetTargetWithOffset(closestStone);
        }
    }

    private void SetTargetWithOffset(Stone stone)
    {
        if (groupManager != null)
        {
            groupManager.UnregisterMiner(this);
            groupManager = null;
        }

        targetStone = stone;

        groupManager = stone.GetComponent<TreeGroupManager>();
        if (groupManager == null)
            groupManager = stone.gameObject.AddComponent<TreeGroupManager>();

        groupManager.RegisterMiner(this);

        SetOffsetPosition(stone.transform.position + (Vector3)(Random.insideUnitCircle * 0.5f));
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
