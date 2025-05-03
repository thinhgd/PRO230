using Game.Tutorials;
using Pathfinding;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AIPath), typeof(Seeker))]
public class BuilderAI : BaseEnemy
{
    public float workInterval = 2f;
    public float searchRadius = 15f;
    public LayerMask constructionLayer;
    public Transform homePosition;

    private AIPath aiPath;
    private ConstructionProgress targetConstruction;
    private bool isWorking = false;
    private Vector3 offsetPosition;

    protected override void Start()
    {
        base.Start();
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        InvokeRepeating(nameof(FindConstructionToBuild), 0f, 2f);
    }

    protected override void Update()
    {
        base.Update();
        if (currentState == EnemyState.Die) return;

        float distToOffset = Vector2.Distance(transform.position, offsetPosition);
        UpdateSpriteDirection();
        if (distToOffset <= 0.5f)
        {
            aiPath.canMove = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("isRun", false);
            SetState(EnemyState.Attack);

            workInterval -= Time.deltaTime;
            if (workInterval <= 0f)
            {
                targetConstruction?.WorkOnConstruction(this);
                workInterval = 2f;
            }
        }
        else
        {
            aiPath.canMove = true;
            SetState(EnemyState.Run);
        }
        if (!isWorking && Vector2.Distance(transform.position, homePosition.position) <= 0.5f)
        {
            aiPath.canMove = false;
            rb.velocity = Vector2.zero;
            SetState(EnemyState.Idle);
            animator.SetBool("isRun", false);
            gameObject.SetActive(false);
        }

    }

    private void FindConstructionToBuild()
    {
        if (isWorking) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, constructionLayer);
        foreach (var hit in hits)
        {
            var cp = hit.GetComponent<ConstructionProgress>();
            if (cp != null && !cp.IsFullyBuilt() && !cp.HasBuilderAssigned())
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    transform.position = homePosition.position;
                }

                AssignConstruction(cp);
                return;
            }
        }
    }

    public void AssignConstruction(ConstructionProgress target)
    {
        targetConstruction = target;
        target.AssignBuilder(this);
        isWorking = true;

        offsetPosition = target.transform.position + (Vector3)(Random.insideUnitCircle * 1.5f);
        aiPath.destination = offsetPosition;
        aiPath.canMove = true;
        SetState(EnemyState.Run);
    }

    public void FinishConstruction()
    {
        ReturnHome();
    }

    private void ReturnHome()
    {
        isWorking = false;
        targetConstruction = null;
        offsetPosition = homePosition.position;
        aiPath.destination = homePosition.position;
        aiPath.canMove = true;
        SetState(EnemyState.Run);
    }


    public bool IsAvailable()
    {
        return !isWorking;
    }
    private void UpdateSpriteDirection()
    {
        if (aiPath == null || spriteRenderer == null) return;

        if (Mathf.Abs(aiPath.velocity.x) > 0.01f)
        {
            spriteRenderer.flipX = aiPath.velocity.x < 0f;
        }
    }
    protected override void Attack()
    {
        base.Attack();
    }

    protected override void Run()
    {
        base.Run();
        animator.SetBool("isRun", aiPath.velocity.magnitude > 0.1f);
    }
}
