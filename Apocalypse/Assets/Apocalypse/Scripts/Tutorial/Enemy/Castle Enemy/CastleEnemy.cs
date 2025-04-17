using UnityEngine;
using Pathfinding;

namespace Game.Tutorials
{
    [RequireComponent(typeof(Seeker), typeof(AIPath), typeof(AIDestinationSetter))]
    public class CastleEnemy : BaseEnemy
    {
        [Header("A* Settings")]
        public Transform targetCastle;
        public float stopDistance = 0.2f;

        private AIPath aiPath;
        private AIDestinationSetter destinationSetter;

        [Header("Fix Vi Tri Enemy")]
        private Vector3 lastPosition;
        private float stuckTimer = 0f;
        private float stuckCheckDelay = 1f;
        private Transform offsetTarget;
        private EnemyGroupManager groupManager;
        private Vector3 offsetPosition;

        [Header("Delay truoc khi di chuyen")]
        public float minMoveDelay = 0.2f;
        public float maxMoveDelay = 1.0f;
        protected override void Start()
        {
            base.Start();
            lastPosition = transform.position;
            aiPath = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();

            if (targetCastle == null)
            {
                FindNearestCastle();
            }

            SetTargetWithOffset(targetCastle);
        }

        protected override void Update()
        {
            base.Update();

            if (currentState == EnemyState.Die) return;

            if (targetCastle == null || !targetCastle.gameObject.activeInHierarchy)
            {
                FindNearestCastle();

                if (targetCastle != null && destinationSetter != null)
                {
                    SetTargetWithOffset(targetCastle);
                }

                return;
            }
            float distanceToOffset = Vector2.Distance(transform.position, offsetPosition);
            float distanceToCastle = Vector2.Distance(transform.position, targetCastle.position);

            if (distanceToCastle <= stopDistance)
            {
                aiPath.canMove = false;
                rb.velocity = Vector2.zero;
                animator.SetBool("isRun", false);
                SetState(EnemyState.Attack);
            }
            else if (distanceToOffset > 0.2f) // chưa đến đúng chỗ đứng
            {
                aiPath.canMove = true;
                SetState(EnemyState.Run);
            }
            else
            {
                float speed = aiPath.velocity.magnitude;
                if (Vector3.Distance(transform.position, lastPosition) < 0.05f)
                {
                    stuckTimer += Time.deltaTime;

                    if (stuckTimer >= stuckCheckDelay)
                    {
                        float distToTarget = Vector2.Distance(transform.position, targetCastle.position);

                        if (distToTarget < 1.5f)
                        {
                            Transform oldTarget = targetCastle;
                            FindNearestCastle(exclude: oldTarget);

                            if (targetCastle != null && targetCastle != oldTarget)
                            {
                                SetTargetWithOffset(targetCastle);
                                aiPath.canMove = true;
                                SetState(EnemyState.Run);
                                stuckTimer = 0f;
                                return;
                            }
                        }
                        animator.SetBool("isRun", false);
                        SetState(EnemyState.Idle);
                        aiPath.canMove = false;
                    }
                }
                else
                {
                    if (!aiPath.canMove)
                    {
                        aiPath.canMove = true;
                        SetState(EnemyState.Run);
                    }

                    stuckTimer = 0f;
                }

                lastPosition = transform.position;
            }
            if (currentState == EnemyState.Idle)
            {
                float dist = Vector2.Distance(transform.position, targetCastle.position);
                if (dist > stopDistance + 0.1f)
                {
                    aiPath.canMove = true;
                    SetState(EnemyState.Run);
                }
            }
            UpdateSpriteDirection();
        }

        private void FindNearestCastle(Transform exclude = null)
        {
            float shortestDistance = Mathf.Infinity;
            GameObject[] castles = GameObject.FindGameObjectsWithTag("Town");
            Transform bestTarget = null;

            foreach (GameObject castle in castles)
            {
                if (castle.transform == exclude) continue;

                float distance = Vector2.Distance(transform.position, castle.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestTarget = castle.transform;
                }
            }

            if (bestTarget != null)
            {
                targetCastle = bestTarget;
            }
        }

        protected override void Run()
        {
            base.Run();
            if (aiPath != null)
            {
                float speed = aiPath.velocity.magnitude;
                animator.SetBool("isRun", speed > 0.1f);
                float x = aiPath.desiredVelocity.x;

                if (Mathf.Abs(x) > 0.01f)
                {
                    spriteRenderer.flipX = x < 0f;
                }
            }
        }

        protected override void Attack()
        {
            if (Vector2.Distance(transform.position, targetCastle.position) > stopDistance)
            {
                SetState(EnemyState.Run);
                return;
            }

            base.Attack();

            // Gây sát thương cho castle tại đây nếu cần
            // Castle castle = targetCastle.GetComponent<Castle>();
            // if (castle != null && attackCooldown <= 0f)
            // {
            //     castle.TakeDamage(stats.physicalDamage);
            // }
        }
        private void UpdateSpriteDirection()
        {
            if (targetCastle != null)
            {
                float directionToCastle = targetCastle.position.x - transform.position.x;

                if (Mathf.Abs(directionToCastle) > 0.01f)
                {
                    spriteRenderer.flipX = directionToCastle < 0f;
                }
            }
        }
        private void SetTargetWithOffset(Transform realTarget)
        {

            if (groupManager != null)
            {
                groupManager.UnregisterEnemy(this);
                groupManager = null;
            }

            targetCastle = realTarget;

            groupManager = realTarget.GetComponent<EnemyGroupManager>();
            if (groupManager == null)
                groupManager = realTarget.gameObject.AddComponent<EnemyGroupManager>();

            groupManager.RegisterEnemy(this);
        }
        public void SetOffsetPosition(Vector3 pos)
        {
            offsetPosition = pos;

            if (aiPath != null)
            {
                aiPath.destination = offsetPosition;
            }
        }
    }
}
