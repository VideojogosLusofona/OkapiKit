using UnityEditor;
using UnityEngine;
using System.Linq;

namespace OkapiKit.Editor
{
    public abstract class OkapiBaseEditor : UnityEditor.Editor
    {
        protected SerializedProperty propShowInfo;
        protected SerializedProperty propDescription;
        protected SerializedProperty propExplanation;

        protected abstract GUIStyle GetTitleSyle();
        protected abstract GUIStyle GetExplanationStyle();

        protected string GetExplanation()
        {
            return propExplanation.stringValue;
        }

        protected abstract string GetTitle();

        protected abstract Texture2D GetIcon();

        protected virtual bool showInfo
        {
            get { return propShowInfo.boolValue; }
            set { propShowInfo.boolValue = value; }
        }

        protected abstract (Color, Color, Color) GetColors();

        protected virtual void OnEnable()
        {
            var okapiElement = target as OkapiElement;
            if (okapiElement != null)
            {
                okapiElement.UpdateExplanation();
            }

            propShowInfo = serializedObject.FindProperty("_showInfo");
            propDescription = serializedObject.FindProperty("description");
            propExplanation = serializedObject.FindProperty("_explanation");
        }

        protected Rect titleRect;

        protected virtual bool WriteTitle()
        {
            var okapiElement = target as OkapiElement;
            if (okapiElement == null) 
            {
                return false;
            }

            GUIStyle styleTitle = GetTitleSyle();
            GUIStyle explanationStyle = GetExplanationStyle();

            (var backgroundColor, var textColor, var separatorColor) = GetColors();

            // Compute explanation text height
            string explanation = GetExplanation();
            int explanationLines = explanation.Count((c) => c == '\n') + 1;
            int explanationTextHeight = explanationLines * (explanationStyle.fontSize + 2) + 6;

            // Background and title
            float inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
            titleRect = EditorGUILayout.BeginVertical("box");
            Rect rect = new Rect(titleRect.x, titleRect.y, inspectorWidth - titleRect.x, styleTitle.fontSize + 14);
            Rect fullRect = rect;
            if (explanation != "")
            {
                fullRect.height = rect.height + 6 + explanationTextHeight;
            }
            Color barColor = backgroundColor;
            if (OkapiConfig.IsPinged(okapiElement))
            {
                barColor = Color.yellow;
            }
            EditorGUI.DrawRect(fullRect, barColor);
            var prevColor = styleTitle.normal.textColor;
            styleTitle.normal.textColor = textColor;
            GUI.DrawTexture(new Rect(titleRect.x + 10, titleRect.y + 4, 32, 32), GetIcon(), ScaleMode.ScaleToFit, true, 1.0f);
            EditorGUI.LabelField(new Rect(titleRect.x + 50, titleRect.y + 6, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), GetTitle(), styleTitle);
            styleTitle.normal.textColor = prevColor;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(fullRect.height);

            if (explanation != "")
            {
                // Separator
                Rect separatorRect = new Rect(titleRect.x + 4, titleRect.y + rect.height, inspectorWidth - 20 - 8, 4);
                EditorGUI.DrawRect(separatorRect, separatorColor);

                // Explanation
                EditorGUI.LabelField(new Rect(titleRect.x + 10, separatorRect.y + separatorRect.height + 4, inspectorWidth - 20 - titleRect.x - 4, explanationTextHeight), explanation, explanationStyle);
            }

            bool toggle = false;
            bool refreshExplanation = false;
            if (showInfo)
            {
                toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeClose"));
            }
            else
            {
                toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeOpen"));
            }
            refreshExplanation = GUI.Button(new Rect(rect.x + rect.width - 26, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("Refresh"));
            if (toggle)
            {
                refreshExplanation = true;
                showInfo = !showInfo;

                Event e = Event.current;
                if (e.shift)
                {
                    // Affect all the triggers in this object
                    var allOkapi = (target as Component).GetComponents<OkapiElement>();
                    foreach (var t in allOkapi)
                    {
                        t.showInfo = showInfo;
                        t.UpdateExplanation();
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
            if (refreshExplanation)
            {
                propExplanation.stringValue = okapiElement.UpdateExplanation();
                serializedObject.ApplyModifiedProperties();
            }

            DisplayLogs();

            return propShowInfo.boolValue;
        }

        protected void DisplayLogs()
        {
            var okapiElement = target as OkapiElement;

            var logs = okapiElement.logs;
            if (logs.Count > 0)
            {
                var rect = EditorGUILayout.BeginVertical("box");
                
                float       inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
                GUIStyle    explanationStyle = GUIUtils.GetLogStyle();

                int index = 0;
                foreach (var log in logs)
                {
                    Color color = Color.white;
                    switch (log.type)
                    {
                        case LogEntry.Type.Debug:
                            color = (index % 2 == 0) ? (GUIUtils.ColorFromHex("#5edb64")) : (GUIUtils.ColorFromHex("#5ea064"));
                            break;
                        case LogEntry.Type.Warning:
                            color = (index % 2 == 0) ? (GUIUtils.ColorFromHex("#dbcb5e")) : (GUIUtils.ColorFromHex("#bbbb5e"));
                            break;
                        case LogEntry.Type.Error:
                            color = (index % 2 == 0) ? (GUIUtils.ColorFromHex("#ee5a5a")) : (GUIUtils.ColorFromHex("#bb5a5a"));
                            break;
                        default:
                            color = (index % 2 == 0) ? (GUIUtils.ColorFromHex("#bfbfbf")) : (GUIUtils.ColorFromHex("#909090"));
                            break;
                    }
                    index++;

                    int textLines = log.text.Count((c) => c == '\n');
                    int height = 18 + textLines * 16;

                    Rect logErrorRect = new Rect(titleRect.x, rect.y, inspectorWidth - 10 - titleRect.x - 4, height);
                    EditorGUI.DrawRect(logErrorRect, color);
                    if (log.tooltip != "")
                        EditorGUI.LabelField(logErrorRect, new GUIContent("", log.tooltip));

                    logErrorRect.x += 10;
                    logErrorRect.width += 10;

                    EditorGUI.LabelField(logErrorRect, log.text, explanationStyle);
                    rect.y += height;
                    EditorGUILayout.Space(height);
                }

                EditorGUILayout.EndVertical();
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return (OkapiConfig.IsPinged(target as OkapiElement));
        }
    }
}