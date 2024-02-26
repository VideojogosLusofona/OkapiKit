using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System.ComponentModel;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/UI/Display Text (Multi)")]
    public class MultiValueDisplayText : MultiValueDisplay
    {
        private string baseText;

        TextMeshProUGUI textGUI;
        TextMeshPro     textScene;

        void Start()
        {
            GetTarget();
        }

        bool GetTarget()
        {
            if ((textGUI == null) && (textScene == null))
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

            return (textGUI != null) || (textScene != null);
        }

        void Update()
        {
            var v = GetVariables();
            if (v == null) return;
            if ((textGUI == null) && (textScene == null)) return;

            var txt = "";
            try
            {
                txt = string.Format(baseText, v);
            }
            catch
            {
                txt = $"[Invalid Formatter - {baseText}]";
            }
            if (textGUI) textGUI.text = txt;
            if (textScene) textScene.text = txt;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string ret = $"This component displays the variables {GetVariableNames()} as text.";
            if (!GetTarget())
            {
                ret += "\nThere has to be a text component on this object with the C# text formatter string set.";
            }
            return ret;
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
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Need to have a TextMeshPro or TextMeshProUGUI component on this object!", "To display some text, we need to have a TextMeshPro component (either TextMeshPro or TextMeshProUGUI, depending if it's part of a UI/canvas or the scene itself)."));
                }
                else
                {
                    if ((textScene.text == "") || (textScene.text.IndexOf('{') == -1))
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Warning, "Need to set the text of the component to a C# text formatter (like {0} or {0:D4})", "The text can incorporate other parts, only the part between { } will be replaced by the value itself and formatted according to the C# rules."));
                    }
                }
            }
            else
            {
                if ((textGUI.text == "") || (textGUI.text.IndexOf('{') == -1))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Need to set the text of the component to a C# text formatter (like {0} or {0:D4})", "The text can incorporate other parts, only the part between { } will be replaced by the value itself and formatted according to the C# rules."));
                }
            }
        }

    }
}