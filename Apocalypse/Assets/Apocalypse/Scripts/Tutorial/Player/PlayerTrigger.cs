using UnityEngine;

namespace Game.Tutorial
{
    public class PlayerTrigger : MonoBehaviour
    {
        private PlayerCtrl playerCtrl;

        private void Start()
        {
            playerCtrl = GetComponentInParent<PlayerCtrl>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            ItemSO selectedItem = HotBarManager.instance.GetSelectedItem();
            if (selectedItem == null) return;

            if (selectedItem.action == ActionType.Attack && collision.CompareTag(Tag.ENEMY))
            {
                var enemy = collision.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(playerCtrl.playerStats);
                }
            }
            if (selectedItem.action == ActionType.Axe && collision.CompareTag(Tag.TREE))
            {
                var tree = collision.GetComponent<Tree>();
                if (tree != null)
                    tree.OnHit();
            }
        }
    }
}
