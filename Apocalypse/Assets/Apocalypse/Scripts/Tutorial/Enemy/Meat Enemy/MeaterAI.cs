using Game.Tutorials;
using Pathfinding;
using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AIPath), typeof(Seeker))]
public class MeaterAI : BaseEnemy
{
    [Header("Meat Settings")]
    public float gatherRange = 1f;
    public float gatherInterval = 2f;
    public float searchRadius = 10f;
    public LayerMask meatLayer;

    [Header("UI Elements")]
    public TextMeshProUGUI meatText;

    [Header("Meat Carry Settings")]
    public int meatCapacity = 10;
    private int carriedMeat = 0;
    private bool isDepositingMeat = false;

    private Meat targetMeat;
    private AIPath aiPath;
    private float gatherTimer = 2f;

    private Vector3 offsetPosition;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckCheckDelay = 1f;

    private TreeGroupManager groupManager;

    [Header("Meat Gain Panel")]
    public GameObject meatGainPanelPrefab;
    public Transform meatPanelSpawnParent;

    [Header("Random Walk Settings")]
    public float moveRadius = 3f;
    public float minRestTime = 1f;
    public float maxRestTime = 3f;
    private bool isRandomWalking = false;
    [SerializeField] private float offsetUpdateThreshold = 0.5f; // Thêm biến để điều chỉnh trong Inspector
    private bool isUpdatingTargetPosition = false;

    bool hasAttacked = false;
    protected override void Start()
    {
        base.Start();
        aiPath = GetComponent<AIPath>();
        lastPosition = transform.position;
        UpdateMeatUI();
        InvokeRepeating(nameof(AutoFindNearestMeat), 0f, 2f);
    }

    protected override void Update()
    {
        base.Update();
        if (currentState == EnemyState.Die) return;

        if (carriedMeat > 0 || carriedMeat < meatCapacity)
        {
            UpdateMeatUI();
        }

        UpdateSpriteDirection();

        if (isDepositingMeat)
        {
            HandleDepositingMeat();
            return;
        }

        if (targetMeat == null || targetMeat.currentHealth <= 0)
        {
            SetState(EnemyState.Idle);
            if (!isRandomWalking) StartCoroutine(RandomWalkRoutine());
            return;
        }

        float distToMeat = Vector2.Distance(transform.position, targetMeat.transform.position);

        if (!isDepositingMeat && !isUpdatingTargetPosition)
        {
            StartCoroutine(UpdateTargetPositionWithDelay());
        }
        if (distToMeat <= gatherRange)
        {
            if (!hasAttacked)
            {
                Attack();
                hasAttacked = true;
                return;
            }
            StopCoroutine(RandomWalkRoutine());
            aiPath.canMove = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("isRun", false);
            SetState(EnemyState.Attack);

            gatherTimer += Time.deltaTime;
            if (gatherTimer >= gatherInterval)
            {
                if (carriedMeat < meatCapacity)
                {
                    targetMeat.Gather();
                    carriedMeat++;
                    hasAttacked = false;
                    if (meatGainPanelPrefab != null && meatPanelSpawnParent != null)
                    {
                        Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
                        GameObject panel = Instantiate(meatGainPanelPrefab, meatPanelSpawnParent);
                        panel.transform.position = spawnPos;
                    }

                    if (carriedMeat >= meatCapacity)
                    {
                        GoToDepositMeat();
                        return;
                    }
                }

                gatherTimer = 0f;
            }
        }
        else
        {
            aiPath.canMove = true;
            aiPath.destination = offsetPosition;
            SetState(EnemyState.Run);

            if (Vector3.Distance(transform.position, lastPosition) < 0.05f)
            {
                stuckTimer += Time.deltaTime;

                if (stuckTimer >= stuckCheckDelay)
                {
                    Meat oldTarget = targetMeat;
                    FindNearestMeat(exclude: oldTarget);

                    if (targetMeat != null && targetMeat != oldTarget)
                    {
                        SetTargetWithOffset(targetMeat);
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

    private void UpdateMeatUI()
    {
        if (meatText != null)
            meatText.text = "Meat: " + carriedMeat + "/" + meatCapacity;
    }

    private void HandleDepositingMeat()
    {
        if (WoodStorage.Instance == null) return;

        aiPath.destination = WoodStorage.Instance.storagePoint.position;

        if (Vector2.Distance(transform.position, WoodStorage.Instance.storagePoint.position) < 0.5f)
        {
            CastleManager castle = FindObjectOfType<CastleManager>();
            if (castle != null)
            {
                castle.AddResource(ResourceType.Meat, carriedMeat);
            }

            Meat.totalStore += carriedMeat;
            carriedMeat = 0;
            isDepositingMeat = false;

            if (targetMeat != null) targetMeat.InvokeGatheredEvent();
            targetMeat = null;
            hasAttacked = false;

            AutoFindNearestMeat();
            UpdateMeatUI();
        }
    }

    private void GoToDepositMeat()
    {
        isDepositingMeat = true;
        aiPath.canMove = true;
        aiPath.destination = WoodStorage.Instance.storagePoint.position;
        SetState(EnemyState.Run);
    }

    public void AutoFindNearestMeat()
    {
        FindNearestMeat();
    }

    public void FindNearestMeat(Meat exclude = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, meatLayer);

        float closestDist = Mathf.Infinity;
        Meat closestMeat = null;

        foreach (var hit in hits)
        {
            Meat meat = hit.GetComponent<Meat>();
            if (meat != null && meat.currentHealth > 0 && meat != exclude)
            {
                float dist = Vector3.Distance(transform.position, meat.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestMeat = meat;
                }
            }
        }

        if (closestMeat != null)
        {
            StopAllCoroutines();
            SetTargetWithOffset(closestMeat);
            aiPath.canMove = true;
            SetState(EnemyState.Run);
            StartCoroutine(UpdateTargetPositionWithDelay());
        }
    }


    private void SetTargetWithOffset(Meat meat)
    {
        if (groupManager != null)
        {
            groupManager.UnregisterMeater(this);
            groupManager = null;
        }

        targetMeat = meat;
        groupManager = meat.GetComponent<TreeGroupManager>();
        if (groupManager == null)
            groupManager = meat.gameObject.AddComponent<TreeGroupManager>();

        groupManager.RegisterMeater(this);
        SetOffsetPosition(meat.transform.position + (Vector3)(Random.insideUnitCircle * 0.5f));
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

    IEnumerator RandomWalkRoutine()
    {
        isRandomWalking = true;

        while (targetMeat == null && !isDepositingMeat)
        {
            Vector3 randomOffset = Random.insideUnitCircle * moveRadius;
            SetOffsetPosition(transform.position + randomOffset);
            aiPath.canMove = true;
            animator.SetBool("isRun", true);
            SetState(EnemyState.Run);

            yield return new WaitForSeconds(Random.Range(minRestTime, maxRestTime));
        }

        isRandomWalking = false;
        animator.SetBool("isRun", false);
    }
    IEnumerator UpdateTargetPositionWithDelay()
    {
        isUpdatingTargetPosition = true;

        if (targetMeat != null)
        {
            SetOffsetPosition(targetMeat.transform.position + (Vector3)(Random.insideUnitCircle * 0.5f));
        }

        yield return new WaitForSeconds(1f);
        isUpdatingTargetPosition = false;
    }
}
//public float roamRadius = 2f;
//     public float idleMoveInterval = 4f;

//     private AIPath aiPath;
//     private AIDestinationSetter destinationSetter;
//     private SpriteRenderer spriteRenderer;
//     private Animator animator;

//     private enum PetState { Idle, Run }
// private PetState currentState = PetState.Idle;

// void Start()
// {
//     aiPath = GetComponent<AIPath>();
//     destinationSetter = GetComponent<AIDestinationSetter>();
//     spriteRenderer = GetComponent<SpriteRenderer>();
//     animator = GetComponent<Animator>();

//     aiPath.canMove = true;
//     StartCoroutine(RandomWalkRoutine()); // Bắt đầu di chuyển ngẫu nhiên
// }

// void Update()
// {
//     if (aiPath.desiredVelocity.x > 0.1f)
//         spriteRenderer.flipX = false;
//     else if (aiPath.desiredVelocity.x < -0.1f)
//         spriteRenderer.flipX = true;

//     if (aiPath.velocity.magnitude < 0.01f)
//         ChangeState(PetState.Idle);
//     else
//         ChangeState(PetState.Run);
// }

// void ChangeState(PetState newState)
// {
//     if (currentState == newState) return;

//     currentState = newState;
//     PlayAnimation(currentState);
// }

// void PlayAnimation(PetState state)
// {
//     switch (state)
//     {
//         case PetState.Idle:
//             animator.Play("Idle");
//             break;
//         case PetState.Run:
//             animator.Play("Run");
//             break;
//     }
// }

// IEnumerator RandomWalkRoutine()
// {
//     while (true)
//     {
//         Vector2 randomDir = Random.insideUnitCircle.normalized;
//         Vector3 roamTargetPos = transform.position + (Vector3)randomDir * Random.Range(1f, roamRadius);

//         GameObject roamTarget = new GameObject("MeaterRoamTarget");
//         roamTarget.transform.position = roamTargetPos;
//         destinationSetter.target = roamTarget.transform;

//         aiPath.canMove = true;
//         aiPath.maxSpeed = 1.5f;

//         Destroy(roamTarget, idleMoveInterval);

//         yield return new WaitForSeconds(idleMoveInterval);
//     }
// }