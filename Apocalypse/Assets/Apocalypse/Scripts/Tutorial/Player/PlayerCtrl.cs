using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Game.Tutorial
{
    public class PlayerCtrl : PlayerFinalStatMachine
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private PlayerStats playerStats;
        private Vector2 moveInput;

        [SerializeField] private PlayerInput playerInput;

        private bool isAttack = false;
        private bool isDie = false;

        //private List<string> tools = new List<string> {Tag.ATTACK ,Tag.AXE, Tag.WATERING };
        //private int currentToolIndex = 0;
        protected override void Init()
        {
            if(playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            SetDefautlState();
        }

        protected override void FSMUpdate()
        {
            if (playerStats.CurrentHealth <= 0 && !isDie)
                ChangeState(State.Die);

            CheckInput();
        }
        private void CheckInput()
        {
            var input = playerInput.actions["Move"].ReadValue<Vector2>();
            moveInput = new Vector2(input.x, input.y);
            //moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) && !isAttack)
                ChangeState(State.Attack);
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
                case State.Die:
                    DieState();
                    break;
            }
        }

        private void IdleState()
        {
            if (isDie) return;
            PlayAnimation(Tag.IDLE);

            if (Mathf.Abs(moveInput.x) > Mathf.Epsilon || Mathf.Abs(moveInput.y) > Mathf.Epsilon)
                ChangeState(State.Run);
        }

        private void RunState()
        {
            if (isDie) return;
            PlayAnimation(Tag.RUN);
            SetVelocity(moveInput.x, moveInput.y, speed);

            if (Mathf.Abs(moveInput.x) <= Mathf.Epsilon && Mathf.Abs(moveInput.y) <= Mathf.Epsilon)
                ChangeState(State.Idle);

        }
        private void UseToolState()
        {
            if (isDie) return;

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

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (!photonView.IsMine) return;

        //    if (!collision.CompareTag("Item")) return;

        //    var item = collision.gameObject.GetComponent<Item>();
        //    if (item && boxCollider2D)
        //        HotBarManager.instance.AddItem(item.itemSO);
        //}
    }
}
