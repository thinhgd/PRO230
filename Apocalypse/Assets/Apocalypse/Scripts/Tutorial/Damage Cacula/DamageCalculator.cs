using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorials
{
    public static class DamageCalculator
    {
        public struct DamageResult
        {
            public int totalDamage;
            public bool isCrit;
            public bool isDodged;
            public int reflectedDamage;
        }

        public static int CalculateDamage(int physicalDmg, int magicDmg, float critChance, float critMultiplier, int armor, int magicResist, float dodgeChance)
        {
            //kiem tra ne
            if (Random.value < dodgeChance)
            {
                Debug.Log("Ne thanh cong.");
                return 0;
            }

            // kiem tra chi mang
            bool isCrit = Random.value < critChance;
            float critFactor = isCrit ? critMultiplier : 1f;

            // tinh sat thuong vat li + voi sat thuong chi mang
            int finalPhysical = Mathf.Max(Mathf.FloorToInt(physicalDmg * critFactor) - armor, 1);

            // tinh sat thuong phep
            int finalMagic = Mathf.Max(magicDmg - magicResist, 1);

            // tong sat thuong
            return finalPhysical + finalMagic;
        }

        public static DamageResult CalculateDamage(
            int physicalDmg, int magicDmg,
            float critChance, float critMultiplier,
            int armor, int magicResist,
            float dodgeChance, float reflectDamagePercent)
        {
            DamageResult result = new DamageResult();

            // kiem tra ne tranh
            result.isDodged = Random.value < dodgeChance;
            if (result.isDodged)
            {
                result.totalDamage = 0;
                result.isCrit = false;
                result.reflectedDamage = 0;
                return result;
            }

            // kiem tra chi mang
            result.isCrit = Random.value < critChance;
            float critFactor = result.isCrit ? critMultiplier : 1f;

            // Tinh sat thuong vat li + sat thuong chi mang
            int finalPhysical = Mathf.Max(Mathf.FloorToInt(physicalDmg * critFactor) - armor, 1);

            // Tinh sat thuong phep
            int finalMagic = Mathf.Max(magicDmg - magicResist, 1);

            // Tong sat thuong
            result.totalDamage = finalPhysical + finalMagic;

            // Phan sat thuong
            result.reflectedDamage = Mathf.FloorToInt(result.totalDamage * reflectDamagePercent);

            return result;
        }

        // hut mau
        public static float CalculateLifesteal(int damage, float lifestealPercent)
        {
            return damage * lifestealPercent;
        }

        // giam hoi mau
        public static float CalculateAntiHeal(float lifesteal, float antiHealPercent)
        {
            return lifesteal * (1 - antiHealPercent);
        }
    }
}
