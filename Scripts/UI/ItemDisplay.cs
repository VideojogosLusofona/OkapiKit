using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OkapiKit
{
    public class ItemDisplay : OkapiElement
    {
        public enum ItemSource { Inventory, Equipment };

        [SerializeField] private ItemSource         itemSource = ItemSource.Inventory; 
        [SerializeField] private TargetInventory    inventory;
        [SerializeField] private TargetEquipment    equipment;
        [SerializeField] private int                inventorySlot;
        [SerializeField] private Hypertag           equipmentSlot;
        [SerializeField] private Image              imageRef;
        [SerializeField] private TextMeshProUGUI    quantityRef;

        private string baseText;

        private void Start()
        {
            baseText = quantityRef?.text ?? "{0}";
            if (baseText.IndexOf("{0}") == -1)
                baseText = "{0}";
        }

        private void Update()
        {
            Item item = null;
            int quantity = 0;
            if (itemSource == ItemSource.Inventory)
            {
                var inv = inventory.GetTarget(gameObject);
                if (inv != null)
                {
                    (item, quantity) = inv.GetSlotContent(inventorySlot);
                }
            }
            else
            {
                var equip = equipment.GetTarget(gameObject);
                if (equip)
                {
                    item = equip.GetItem(equipmentSlot);
                    quantity = (item != null) ? (1) : (0);
                }
            }

            if (imageRef)
            {
                if (item)
                {
                    if (item.displaySprite)
                    {
                        imageRef.sprite = item.displaySprite;
                        imageRef.enabled = true;
                    }
                    else
                    {
                        imageRef.enabled = false;
                    }
                    imageRef.color = item.displaySpriteColor;
                }
                else
                {
                    imageRef.enabled = false;
                }
            }
            if (quantityRef)
            {
                if (quantity > 1)
                {
                    quantityRef.enabled = true;
                    quantityRef.text = string.Format(baseText, quantity);
                    quantityRef.color = item.displayTextColor;
                }
                else
                {
                    quantityRef.enabled = false;
                }
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";

            if ((imageRef == null) && (quantityRef == null))
            {
                return "Displays nothing (no UI references configured).";
            }

            switch (itemSource)
            {
                case ItemSource.Inventory:
                    string inventoryName = inventory.GetRawDescription("inventory", refObject);
                    desc += $"Displays the item from inventory slot {inventorySlot} in {inventoryName}.";
                    break;

                case ItemSource.Equipment:
                    string equipmentName = equipment.GetRawDescription("equipment", refObject);
                    string slotName = equipmentSlot != null ? equipmentSlot.name : "[no slot selected]";
                    desc += $"Displays the item equipped in slot '{slotName}' of {equipmentName}.";
                    break;
            }

            if (imageRef != null)
            {
                desc += " Shows the item's sprite image.";
            }

            if (quantityRef != null)
            {
                desc += " Shows the quantity if greater than 1.";
            }

            return desc;
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";

            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            switch (itemSource)
            {
                case ItemSource.Inventory:
                    inventory.CheckErrors(_logs, "inventory", gameObject);
                    break;
                case ItemSource.Equipment:
                    equipment.CheckErrors(_logs, "equipment", gameObject);
                    if (equipmentSlot == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Slot must be defined!", "Slot must be defined!"));
                    }
                    break;
                default:
                    break;
            }

            if ((imageRef == null) && (quantityRef == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Must define at least one element to display the item properties!", "Must define at least one element to display the item properties!"));
            }

            if ((quantityRef?.text ?? "{0}").IndexOf("{0}") == -1)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Quantity text must have text that include a format specifier like {0}!"));
            }
        }
    }
}
