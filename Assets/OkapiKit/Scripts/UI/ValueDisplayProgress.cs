using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class ValueDisplayProgress : ValueDisplay
{
    [SerializeField] 
    private RectTransform  fill;
    [SerializeField] 
    private bool           setColor;
    [SerializeField, ShowIf("setColor")] 
    private Gradient       color;

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
}
