using NaughtyAttributes;
using System;
using UnityEngine;

namespace OkapiKit
{

    [CreateAssetMenu(fileName = "Item", menuName = "Okapi Kit/Item")]
    public class Item : OkapiScriptableObject
    {
        public Item[]       categories;
        public bool         isCategory = false;
        public string       displayName = "Item Display Name";
        public Color        displaySpriteColor = Color.white;
        public Sprite       displaySprite;
        public Color        displayTextColor = Color.white;
        public bool         isStackable = false;
        [ShowIf(nameof(isStackable))]
        public int          maxStack = 1;
        public Hypertag[]   inventorySlots;

        public override string GetRawDescription(string ident, ScriptableObject refObject)
        {
            return "Item";
        }

        public Hypertag GetEquipAutoSlot(Equipment equipManager, bool prioritizeSame)
        {
            float       minTime = float.MaxValue;
            Hypertag    minSlot = null;

            if (prioritizeSame)
            {
                foreach (var slot in inventorySlots)
                {
                    if (slot == null) continue;
                    if (!equipManager.HasSlot(slot)) continue;

                    if (equipManager.GetItem(slot) == this) return slot;
                }
            }

            foreach (var slot in inventorySlots)
            {
                if (slot == null) continue;
                if (!equipManager.HasSlot(slot)) continue;

                float lastChange = equipManager.GetLastChange(slot);
                if (lastChange < minTime)
                {
                    minTime = lastChange;
                    minSlot = slot;
                }
            }

            return minSlot;
        }

        internal bool IsA(Item itemType)
        {
            if (this == itemType) return true;

            if (categories == null) return false;

            foreach (var item in categories)
            {
                if (item == itemType) return true;
            }

            return false;
        }
    }
}
