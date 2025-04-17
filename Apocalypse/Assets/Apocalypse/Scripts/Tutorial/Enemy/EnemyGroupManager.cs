using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorials
{
    public class EnemyGroupManager : MonoBehaviour
    {
        public float radius = 1.5f;
        private List<CastleEnemy> enemies = new List<CastleEnemy>();

        public void RegisterEnemy(CastleEnemy enemy)
        {
            if (!enemies.Contains(enemy))
                enemies.Add(enemy);
        }

        public void UnregisterEnemy(CastleEnemy enemy)
        {
            enemies.Remove(enemy);
        }

        private void Update()
        {
            float angleStep = 360f / Mathf.Max(1, enemies.Count);

            for (int i = 0; i < enemies.Count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                Vector2 desiredPos = (Vector2)transform.position + offset;
                enemies[i].SetOffsetPosition(desiredPos);
            }
        }
    }
}
