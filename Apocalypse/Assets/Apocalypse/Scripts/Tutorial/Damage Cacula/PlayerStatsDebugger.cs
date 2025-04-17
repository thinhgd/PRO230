using UnityEngine;

namespace Game.Tutorials
{
    public class PlayerStatsDebugger : MonoBehaviour
    {
        [Header("Player Stats")]
        public int maxHP = 1000;
        public int currentHP;
        public int armor = 50;         // Giáp vật lý
        public int magicResist = 30;   // Kháng phép
        public float dodgeChance = 0.2f; // Tỉ lệ né tránh
        public float reflectDamagePercent = 0.1f; // Tỉ lệ phản sát thương

        private void Start()
        {
            currentHP = maxHP;
        }

        /// <summary>
        /// Hàm được gọi khi bị enemy tấn công
        /// </summary>
        public void ReceiveAttackFromEnemy(EnemyStats enemyStats)
        {
            Debug.Log($"===> {enemyStats.enemyName} Tấn Công Player <===");

            // Tính sát thương với các tham số đã cập nhật
            var result = DamageCalculator.CalculateDamage(
                enemyStats.physicalDamage, enemyStats.magicDamage,
                enemyStats.critChance, enemyStats.critMultiplier,
                armor, magicResist,
                dodgeChance, reflectDamagePercent);

            // Nếu bị né tránh
            if (result.isDodged)
            {
                Debug.Log("[Player] Đã né tránh được sát thương!");
                return; // Không trừ máu nếu né tránh
            }

            // Nếu không né tránh, tính sát thương thực tế
            currentHP -= result.totalDamage;
            currentHP = Mathf.Max(currentHP, 0); // Đảm bảo không bị âm máu

            // Hút máu từ enemy
            float lifestealAmount = DamageCalculator.CalculateLifesteal(result.totalDamage, enemyStats.lifestealPercent);

            // Giảm hồi máu của Player do Enemy gây ra
            float reducedHealingPercent = enemyStats.antiHealPercent;
            float finalHealingReceived = DamageCalculator.CalculateAntiHeal(lifestealAmount, reducedHealingPercent);

            Debug.Log($"[Sát thương vật lý] {result.totalDamage} (crit: {(result.isCrit ? enemyStats.critChance * 100 + "%" : "Không chí mạng")})");

            Debug.Log($"[Phản sát thương] {result.reflectedDamage} (phản lại về enemy)");

            Debug.Log($"[Tổng sát thương nhận]: {result.totalDamage}");
            Debug.Log($"[Hút máu Enemy]: {lifestealAmount} HP");
            Debug.Log($"[Giảm hồi máu của Player bởi Enemy]: -{reducedHealingPercent * 100}%");
            Debug.Log($"[Lượng máu Enemy thực sự hồi]: {finalHealingReceived}");

            Debug.Log($"[HP còn lại của Player]: {currentHP}/{maxHP}");
        }

    }
}
