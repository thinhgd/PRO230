using System.Collections;
using UnityEngine;

namespace Game.Tutorials
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class BaseEnemy : MonoBehaviour
    {
        public EnemyStats stats;
        public EnemyState currentState = EnemyState.Idle;

        protected int currentHp;
        protected Rigidbody2D rb;
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;
        protected float attackCooldown = 1f;
        protected Vector2 lastMoveDirection = Vector2.zero;

        private EnemyGroupManager groupManager;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            if (stats == null)
            {
                //Debug.LogError("Thiếu EnemyStats trên " + gameObject.name);
                return;
            }

            currentHp = stats.maxHP;
        }

        protected virtual void Update()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Run:
                    Run();
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.Die:
                    Die();
                    break;
            }

            if (attackCooldown > 0)
                attackCooldown -= Time.deltaTime;
        }

        protected virtual void Idle()
        {
            PlayAnimation(Tag.IDLE);
            rb.velocity = Vector2.zero;
        }

        protected virtual void Run()
        {
            PlayAnimation(Tag.RUN);

            if (rb.velocity.sqrMagnitude > 0.01f)
            {
                lastMoveDirection = rb.velocity.normalized;

                if (lastMoveDirection.x > 0.05f)
                {
                    spriteRenderer.flipX = false;
                }
                else if (lastMoveDirection.x < -0.05f)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }

        protected virtual void Attack()
        {
            if (attackCooldown <= 0f)
            {
                Debug.Log($"{stats.enemyName} tấn công!");
                attackCooldown = 1f / stats.attackSpeed;
                animator.SetTrigger(Tag.ATTACK);
            }
        }

        public virtual void TakeDamage(int physicalDmg, int magicDmg, float critChance, float critMultiplier, float dodgeChance)
        {
            DamageCalculator.DamageResult result = DamageCalculator.CalculateDamage(
                physicalDmg,
                magicDmg,
                critChance,
                critMultiplier,
                stats.armor,
                stats.magicResist,
                dodgeChance,
                stats.reflectDamagePercent);
            if (result.isDodged)
            {
                Debug.Log($"{stats.enemyName} né tránh sát thương!");
                return;
            }

            currentHp -= result.totalDamage;
            Debug.Log($"{stats.enemyName} nhận {result.totalDamage} sát thương (Chí mạng: {result.isCrit}). HP còn: {currentHp}");

            if (result.reflectedDamage > 0)
            {
                Debug.Log($"{stats.enemyName} nhận {result.reflectedDamage} sát thương phản hồi!");
                currentHp -= result.reflectedDamage;
            }

            if (currentHp <= 0 && currentState != EnemyState.Die)
            {
                SetState(EnemyState.Die);
            }
        }





        public virtual void ApplyLifesteal(int damage, float lifestealPercent)
        {
            float lifestealAmount = DamageCalculator.CalculateLifesteal(damage, lifestealPercent);
            currentHp += Mathf.FloorToInt(lifestealAmount);
            Debug.Log($"{stats.enemyName} hồi {lifestealAmount} máu từ hút máu. HP hiện tại: {currentHp}");
        }

        protected virtual void Die()
        {
            Debug.Log($"{stats.enemyName} đã chết.");
            PlayAnimation(Tag.DIE);
            Destroy(gameObject, 1f);
        }

        public virtual void SetState(EnemyState newState)
        {
            if (currentState == EnemyState.Die || currentState == newState) return;
            currentState = newState;
        }

        protected void PlayAnimation(string animationName)
        {
            animator.Play(animationName);
        }

        public void MoveTo(Vector2 targetPos)
        {
            if (currentState == EnemyState.Die) return;

            float speed = stats.moveSpeed;
            Vector2 newPos = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
    }
}
