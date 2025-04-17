using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.Tutorial
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerCtrl : PlayerFinalStatMachine
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 10f;

        [Header("References")]
        [SerializeField] private PlayerStats playerStats;

        [Header("Roll")]
        [SerializeField] private float rollDuration = 0.4f;
        private float rollTimer;
        private bool isRoll;

        private Vector2 moveInput;
        private PlayerInput playerInput;

        private bool isDie = false;
        private bool isInWater = false;

        protected override void Init()
        {
            playerInput = GetComponent<PlayerInput>();
            RegisterInputCallbacks();
            SetDefautlState();
        }

        private void RegisterInputCallbacks()
        {
            var actions = playerInput.actions;

            actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            actions["Move"].canceled += ctx => moveInput = Vector2.zero;

            actions["Attack"].performed += ctx =>
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                if (!isDie && !isInWater && currentState != State.Attack)
                    ChangeState(State.Attack);
            };

            actions["Roll"].performed += ctx =>
            {
                if (!isDie && !isInWater && currentState != State.Roll && moveInput.sqrMagnitude > 0.01f)
                    ChangeState(State.Roll);
            };
        }

        protected override void FSMUpdate()
        {
            if (playerStats.CurrentHealth <= 0 && !isDie)
                ChangeState(State.Die);
        }

        protected override void FSMFixedUpdate()
        {
            switch (currentState)
            {
                case State.Idle:
                    IdleState();
                    break;
                case State.Run:
                    RunState();
                    break;
                case State.Attack:
                    UseToolState();
                    break;
                case State.Roll:
                    RollState();
                    break;
                case State.Swimming:
                    SwimmingState();
                    break;
                case State.Die:
                    DieState();
                    break;
            }
        }

        private void IdleState()
        {
            if (isDie) return;

            PlayAnimation(Tag.IDLE);

            if (moveInput.sqrMagnitude > 0.01f)
                ChangeState(State.Run);
        }

        private void RunState()
        {
            if (isDie) return;

            PlayAnimation(Tag.RUN);
            SetVelocity(moveInput.x, moveInput.y, speed);

            if (moveInput.sqrMagnitude < 0.01f)
                ChangeState(State.Idle);
        }

        private void RollState()
        {
            if (isDie || isInWater) return;

            if (rollTimer <= 0f)
            {
                PlayAnimation(Tag.ROLL);
                rollTimer = rollDuration;
            }

            SetVelocity(moveInput.normalized.x, moveInput.normalized.y, speed * 1.2f);
            rollTimer -= Time.fixedDeltaTime;

            if (rollTimer <= 0f)
                ChangeState(State.Idle);
        }

        private void SwimmingState()
        {
            if (isDie) return;

            PlayAnimation(Tag.SWIMMING);
            SetVelocity(moveInput.x, moveInput.y, speed);

            if (!isInWater)
                ChangeState(moveInput.sqrMagnitude > 0.01f ? State.Run : State.Idle);
        }

        private void UseToolState()
        {
            if (isDie || isInWater) return;

            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            ItemSO selectedItem = HotBarManager.instance.GetSelectedItem();
            if (selectedItem == null)
            {
                ChangeState(State.Idle);
                return;
            }

            string newTool = selectedItem.action.ToString();

            if (!stateInfo.IsName(newTool))
            {
                PlayAnimation(newTool);
                SetZeroVelocity();
            }

            if (stateInfo.normalizedTime >= 1f && !stateInfo.loop)
            {
                ChangeState(State.Idle);
                SetVelocity(moveInput.x, moveInput.y, speed);
            }
        }

        private void DieState()
        {
            if (!isDie)
                StartCoroutine(ReSpawner());
        }

        IEnumerator ReSpawner()
        {
            isDie = true;
            PlayAnimation(Tag.DIE);
            yield return new WaitForSeconds(1);
            playerStats.AddHealth(30);
            yield return new WaitForSeconds(1);
            playerStats.AddHealth(30);
            PlayAnimation(Tag.DESPAWN);
            yield return new WaitForSeconds(1);
            playerStats.AddHealth(40);
            ChangeState(State.Idle);
            isDie = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tag.WATER))
            {
                isInWater = true;

                if (!isDie && currentState != State.Swimming)
                    ChangeState(State.Swimming);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(Tag.WATER))
            {
                isInWater = false;

                if (!isDie && currentState == State.Swimming)
                    ChangeState(moveInput.sqrMagnitude > 0.01f ? State.Run : State.Idle);
            }
        }

        private void OnDisable()
        {
            if (playerInput != null)
            {
                var actions = playerInput.actions;
                actions["Move"].performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
                actions["Move"].canceled -= ctx => moveInput = Vector2.zero;
                actions["Attack"].performed -= ctx => ChangeState(State.Attack);
            }
        }
    }
}
