using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ValueDisplayProgress : ValueDisplay
    {
        [SerializeField]
        private RectTransform fill;
        [SerializeField]
        private bool setColor;
        [SerializeField]
        private Gradient color;

        private Image fillImage;

        private void Start()
        {
            fillImage = fill.GetComponent<Image>();
        }

        void Update()
        {
            var v = GetVariable();
            if (v == null) return;

            float t = (v.currentValue - v.minValue) / (v.maxValue - v.minValue);

            fill.localScale = new Vector2(t, 1.0f);

            if ((setColor) && (fillImage != null) && (color != null))
            {
                fillImage.color = color.Evaluate(t);
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            var desc = "This component displays the value as a progress bar.";

            if (setColor)
            {
                desc += "\nIt also sets the color of the fill rectangle according to the gradient.";
            }

            return desc;
        }

    }
}