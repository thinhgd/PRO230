using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace Game.Tutorial
{
    public class HotBarManager : MonoBehaviour
    {
        public static HotBarManager instance;

        public HotBarSlot[] hotBarSlots;
        public GameObject hotBarItemPrefab;

        private int selectedSlot = -1;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (Input.inputString != null)
            {
                bool isNumber = int.TryParse(Input.inputString, out int number);
                if (isNumber && number > 0 && number < 8)
                    ChangeSelectedSlotInPC(number - 1);
            }
        }

        public bool AddItem(ItemSO item)
        {
            foreach (var slot in hotBarSlots)
            {
                HotBarItem itemSlot = slot.GetComponentInChildren<HotBarItem>();

                if (itemSlot == null)
                {
                    SpawnItem(item, slot);
                    return true;
                }
                else if (itemSlot.itemSO == item && itemSlot.count < 99 && itemSlot.itemSO.stackable)
                {
                    itemSlot.count++;
                    itemSlot.RefreshCount();
                    return true;
                }
            }
            return false;
        }
        private void RemoveItem(ItemSO item, int amount)
        {
            foreach (var slot in hotBarSlots)
            {
                if (amount <= 0)
                    break;

                HotBarItem itemSlot = slot.GetComponentInChildren<HotBarItem>();
                if (itemSlot != null && itemSlot.itemSO == item)
                {
                    int consume = Mathf.Min(itemSlot.count, amount);
                    itemSlot.count -= consume;
                    amount -= consume;

                    if (itemSlot.count <= 0)
                        Destroy(itemSlot.gameObject);
                    else
                        itemSlot.RefreshCount();
                }
            }
        }
        #region SelectSlot
        private void ChangeSelectedSlotInPC(int newValue)
        {
            if (selectedSlot >= 0)
                hotBarSlots[selectedSlot].DeSelect();

            hotBarSlots[newValue].Select();
            selectedSlot = newValue;
        }

        public void ChangeSelectedSlotInMobile(int newValue)
        {
            if(newValue > 8)
                return;

            ChangeSelectedSlotInPC(newValue);
        }
        public ItemSO GetSelectedItem()
        {
            if (selectedSlot < 0 || selectedSlot >= hotBarSlots.Length)
                return null;

            HotBarSlot slot = hotBarSlots[selectedSlot];
            HotBarItem itemInSlot = slot.GetComponentInChildren<HotBarItem>();

            if (itemInSlot != null)
                return itemInSlot.itemSO;

            return null;
        }
        #endregion
        private void SpawnItem(ItemSO item, HotBarSlot slot)
        {
            GameObject newObj = Instantiate(hotBarItemPrefab, slot.transform);
            newObj.GetComponent<HotBarItem>().BeginItem(item);
        }

        // Lay Item 
        public int GetItemCount(ItemSO item)
        {
            int total = 0;
            foreach (var slot in hotBarSlots)
            {
                HotBarItem itemSlot = slot.GetComponentInChildren<HotBarItem>();

                if (itemSlot != null && itemSlot.itemSO == item)
                    total += itemSlot.count;
            }

            return total;
        }

        // Lay Item neu True : du nguyen lieu con False : khong du nguyen lieu
        public bool TakeItem(Dictionary<ItemSO, int> requiredItems)
        {
            foreach (var item in requiredItems)
            {
                if (GetItemCount(item.Key) < item.Value)
                    return false;
            }

            foreach (var item in requiredItems)
            {
                RemoveItem(item.Key, item.Value);
            }

            return true;
        }
    }

}