using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDisplay_Progress : ValueDisplay
{
    [SerializeField] private RectTransform  fill;

    void Update()
    {
        var v = GetVariable();
        if (v == null) return;

        float t = (v.currentValue - v.minValue) / (v.maxValue - v.minValue);

        fill.localScale = new Vector2(t, 1.0f);
    }
}
