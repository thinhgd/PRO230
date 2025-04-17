using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public class Tree : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private int currentHealth;
        public int maxHealth = 5;

        private bool isDead;

        private void Start()
        {
            anim = GetComponent<Animator>();
            currentHealth = maxHealth;
        }

        public void OnHit()
        {
            if (isDead) return;

            if (currentHealth > 1)
            {
                currentHealth--;
                anim.SetTrigger("Hit");
            }
            else
            {
                isDead = true;
                Instantiate(itemPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}