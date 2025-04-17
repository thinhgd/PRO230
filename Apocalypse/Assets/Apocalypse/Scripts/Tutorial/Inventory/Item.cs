using UnityEngine;

namespace Game.Tutorial
{
    public class Item : MonoBehaviour
    {
        public ItemSO itemSO;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tag.PLAYER))
                Destroy(gameObject);
        }
    }
}
