using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValueDisplay_Text : ValueDisplay
{
    private string baseText;

    TextMeshProUGUI textGUI;
    TextMeshPro     textScene;

    void Start()
    {
        textGUI = GetComponent<TextMeshProUGUI>();
        if (textGUI)
        {
            baseText = textGUI.text;
        }
        else
        {
            textScene = GetComponent<TextMeshPro>();
            if (textScene) 
            { 
                baseText = textScene.text;
            }
        }
    }

    void Update()
    {
        var v = GetVariable();
        if (v == null) return;
        if ((textGUI == null) && (textScene == null)) return;

        var txt = string.Format(baseText, v.currentValue);
        if (textGUI) textGUI.text = txt;
        if (textScene) textScene.text = txt;
    }
}
