using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Tutorial;

namespace Game.Tutorials
{
    [CreateAssetMenu(fileName = "Tower", menuName = "TowerSO/Tower")]
    public class TowerSO : ScriptableObject
    {
        public List<TowerLevelData> levels;

        public TowerLevelData GetLevelData(int level)
        {
            if (level < 1 || level > levels.Count) return null;
            return levels[level - 1];
        }
    }

    [Serializable]
    public class TowerLevelData
    {
        public int level;
        public float health;
        public float stVatLy;
        public float stPhep;
        public float giapVatLy;
        public float giapPhep;
        public float tocDoDanh;
        public List<ItemRequirement> requrement;
    }

    [Serializable]
    public class ItemRequirement
    {
        public ItemSO item;
        public int quantity;
    }
}