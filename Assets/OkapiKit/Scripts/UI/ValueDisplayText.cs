using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/UI/Display Text")]
    public class ValueDisplayText : ValueDisplay
    {
        private string baseText;

        TextMeshProUGUI textGUI;
        TextMeshPro textScene;

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
            if (baseText == "") baseText = "{0}";
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

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "This component displays the variable as text.\nThere has to be a text component on this object with the C# text formatter string set.";
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            var textGUI = GetComponent<TextMeshProUGUI>();
            if (textGUI == null)
            {
                var textScene = GetComponent<TextMeshPro>();
                if (textScene == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Need to have a TextMeshPro or TextMeshProUGUI component on this object!"));
                }
                else
                {
                    if (textScene.text == "")
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Warning, "Need to set the text of the component to a C# text formatter (like {0})"));
                    }
                }
            }
            else
            {
                if (textGUI.text == "")
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Need to set the text of the component to a C# text formatter (like {0})"));
                }
            }
        }

    }
}