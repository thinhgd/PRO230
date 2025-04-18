using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tutorial
{
    public class Stats : MonoBehaviour
    {
        [SerializeField] protected Image fill;
        //[SerializeField] private GameObject damageTextPrefab;
        //[SerializeField] private Transform damageTextSpawnPoint;

        [Header("Stats")]
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float currentHealth;
        [Header("Sat Thuong")]
        public float stVatLy;
        public float stPhep;
        public float hutHp;
        [Header("Giap")]
        public float giapVatLy;
        public float giapPhep;
        [Header("Base")]
        public float tocDoDanh;
        public float tocDoDiChuyen;
        [Header("Chi Mang")]
        public float tiLeChiMang;
        public float stChiMang;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        protected virtual void Start()
        {
            currentHealth = maxHealth;
            UpdateHealthBar();
        }

        public void TakeDamage(Stats attacker)
        {
            if (currentHealth <= 0) return;

            // giap vl
            float stVl = attacker.stVatLy * (100 / (100 + giapVatLy));

            // giap Ap
            float stAp = attacker.stPhep * (100 / (100 + giapPhep));

            //Ti le chi mang
            bool isCrit = Random.value < attacker.tiLeChiMang;
            float critDamage = isCrit ? attacker.stChiMang : 0;

            float totalDamage = stVl + stAp + critDamage;
            
            currentHealth = Mathf.Max(0, currentHealth - totalDamage);
            UpdateHealthBar();
            
            //Hut HP
            float hutHP = totalDamage * this.hutHp;
            attacker.AddHealth(hutHP);       
        }

        public void AddHealth(float health)
        {
            if (currentHealth >= maxHealth) return;
            currentHealth = Mathf.Min(maxHealth, currentHealth + health);
            UpdateHealthBar();
        }

        #region GUI
        protected void UpdateHealthBar()
        {
            float healthRatio = currentHealth / maxHealth;
            StartCoroutine(SmoothHealthBar(healthRatio));

            fill.color = (healthRatio < 0.3f)
                ? Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 2, 1))
                : Color.green;
        }

        protected IEnumerator SmoothHealthBar(float targetFill)
        {
            float startFill = fill.fillAmount;
            float elapsedTime = 0f;
            float duration = 0.5f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                fill.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / duration);
                yield return null;
            }

            fill.fillAmount = targetFill;
        }
        //void ShowDamageText(int damage)
        //{
        //    Vector3 spawnPosition = damageTextSpawnPoint.position;
        //    photonView.RPC("ShowDamageTextRPC", RpcTarget.All, damage, spawnPosition);
        //}
        //[PunRPC]
        //void ShowDamageTextRPC(int damage, Vector3 position)
        //{
        //    if (damageTextPrefab == null) return;

        //    GameObject dmgTextObj = Instantiate(damageTextPrefab, position, Quaternion.identity, damageTextSpawnPoint.parent);
        //    DamageText dmgText = dmgTextObj.GetComponent<DamageText>();
        //    if (dmgText != null)
        //    {
        //        dmgText.ShowDamage(damage);
        //    }
        //}
        #endregion
    }

}