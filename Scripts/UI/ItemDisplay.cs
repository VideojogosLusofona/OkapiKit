using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OkapiKit
{
    public class ItemDisplay : MonoBehaviour
    {
        [SerializeField] private TargetInventory    inventory;
        [SerializeField] private int                slot;
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
            var inv = inventory.GetTarget(gameObject);
            if (inv != null)
            {
                (var item, var quantity) = inv.GetSlotContent(slot);
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
        }
    }
}
