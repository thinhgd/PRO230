using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorials
{
    [CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy/Stats")]
    public class EnemyStats : ScriptableObject
    {
        public string enemyName; // ten enemy
        public int maxHP; //hp toi da
        public int physicalDamage; //dame vat li
        public int magicDamage; //dame phep
        public int armor; //giap vat li
        public int magicResist; //giap phep
        public float attackSpeed; //tocdo danh
        public float moveSpeed; //toc do di chuyen
        public float lifestealPercent; //hut mau
        public float antiHealPercent; //giam hoi mau
        public float critChance; //tile chi mang
        public float critMultiplier; //sat thuong chi mang
        public float reflectDamagePercent; //phan tram phan sat thuong
        public float dodgeChance; //ti le ne tranh
    }
}
