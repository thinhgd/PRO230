using UnityEngine;
using Pathfinding;

namespace Game.Tutorials
{
    [RequireComponent(typeof(Seeker), typeof(AIPath), typeof(AIDestinationSetter))]
    public class BowEnemy : BaseEnemy
    {
        [Header("A* Settings")]
        public Transform targetCastle;
        public float attackRange = 10f;
        public GameObject arrowPrefab;
        public Transform firePoint;

        private AIPath aiPath;
        private AIDestinationSetter destinationSetter;

        protected override void Start()
        {
            base.Start();

            aiPath = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();

            if (targetCastle == null)
            {
                FindNearestCastle();
                if (targetCastle == null)
                {
                    Debug.LogError("Không tìm thấy castle gần nhất!");
                    return;
                }
            }

            if (destinationSetter != null)
            {
                destinationSetter.target = targetCastle;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (currentState == EnemyState.Die || targetCastle == null) return;

            float distanceToTarget = Vector2.Distance(transform.position, targetCastle.position);

            if (distanceToTarget <= attackRange)
            {
                aiPath.canMove = false;
                animator.SetBool("isRun", false);
                rb.velocity = Vector2.zero;
                SetState(EnemyState.Attack);
            }
            else
            {
                aiPath.canMove = true;
                SetState(EnemyState.Run);
            }

            UpdateSpriteDirection();
        }

        private void FindNearestCastle()
        {
            float shortestDistance = Mathf.Infinity;
            GameObject[] castles = GameObject.FindGameObjectsWithTag("Town");

            foreach (GameObject castle in castles)
            {
                float distance = Vector2.Distance(transform.position, castle.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    targetCastle = castle.transform;
                }
            }
        }

        protected override void Run()
        {
            base.Run();

            // Flip sprite dựa trên hướng di chuyển của AIPath
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
            if (targetCastle == null || Vector2.Distance(transform.position, targetCastle.position) > attackRange)
            {
                SetState(EnemyState.Run);
                return;
            }

            base.Attack();

            if (attackCooldown <= 0f)
            {
                ShootArrow();
                attackCooldown = 1f / stats.attackSpeed;
            }
        }

        void ShootArrow()
        {
            if (arrowPrefab != null && firePoint != null && targetCastle != null)
            {
                GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
                Vector2 direction = (targetCastle.position - firePoint.position).normalized;
                arrow.GetComponent<Rigidbody2D>().velocity = direction * 10f;
                // TODO: Truyền stats nếu cần
            }
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
