using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/UI/Display Progress")]
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

        protected override void CheckErrors()
        {
            base.CheckErrors();

            var v = GetVariable();
            if (v != null)
            {
                if (!v.hasLimits)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "This type of display only supports values with limits!", "To display a variable as a progress bar, we need to know the limits of the value, so the variable needs to be bound with a lower and upper value."));
                }
            }
            if (fill == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Need to set fill to a UI object that gets scaled with the given value!", "This works by scaling a component depending on the current ercentage of the value, so we need to define what object we want to scale (usually an UI image component)."));
            }
            else if (setColor)
            {
                if (fill.GetComponent<Image>() == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "To set color, fill needs to have an Image component!", "If we want to change the color, we need to define an Image component, since it has the property we need to change."));
                }
            }
        }
    }
}