using UnityEngine;

namespace Game.Tutorial
{
    [CreateAssetMenu(fileName = ("ItemSO"))]
    public class ItemSO : ScriptableObject
    {
        public string id;
        public bool stackable;
        public Sprite image;

        [Space]
        public ItemType type;
        public ActionType action;

        [Header("Stats")]
        public float health;
        [Header("Sat Thuong")]
        public float stVatLy;
        public float stPhep;
        [Header("Giap")]
        public float giapVatLy;
        public float giapPhep;
        [Header("Base")]
        public float tocDoDanh;
        public float tocDoDiChuyen;
        [Header("Chi Mang")]
        public float tiLeChiMang;
        public float stChiMang;
    }
    public enum ItemType
    {
        Tool, Item
    }
    public enum ActionType
    {
        None, Axe, Attack, Mining
    }

}