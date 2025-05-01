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

        protected virtual Color GetIconBackgroundColor() => new Color(0.0f, 0.0f, 0.0f, 0.0f);
        
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
            var okapiScriptableObject = target as OkapiScriptableObject;
            if ((okapiElement == null) && (okapiScriptableObject == null))
            {
                Debug.LogWarning($"OkapiBaseEditor used an object not derived froM OkapiElement or OkapiScriptableObject ({target.GetType()})!");
                return false;
            }

            GUIStyle styleTitle = GetTitleSyle();
            GUIStyle explanationStyle = GetExplanationStyle();
            explanationStyle.wordWrap = true;

            (var backgroundColor, var textColor, var separatorColor) = GetColors();

            // Compute explanation text height
            float inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
            string explanation = GetExplanation();
            // Background and title
            titleRect = EditorGUILayout.BeginVertical("box");
            Rect rect = new Rect(titleRect.x, titleRect.y, inspectorWidth - titleRect.x, styleTitle.fontSize + 14);
            float textAreaWidth = Mathf.Max(0, inspectorWidth - 20 - titleRect.x - 4);
            float explanationTextHeight = 0.0f;
            Rect fullRect = rect;
            if (!string.IsNullOrEmpty(explanation))
            {
                var content = new GUIContent(explanation);
                Rect explanationRect = GUILayoutUtility.GetRect(content, explanationStyle, GUILayout.Width(textAreaWidth));
                explanationTextHeight = explanationRect.height;
                fullRect.height = rect.height + 6 + 4 + explanationTextHeight;
            }
            Color barColor = backgroundColor;
            if (OkapiConfig.IsPinged(okapiElement))
            {
                barColor = Color.yellow;
            }
            EditorGUI.DrawRect(fullRect, barColor);
            var prevColor = styleTitle.normal.textColor;
            styleTitle.normal.textColor = textColor;

            Rect iconRect = new Rect(titleRect.x + 10, titleRect.y + 4, 32, 32);
            var  iconBackgroundColor = GetIconBackgroundColor();
            if (iconBackgroundColor.a > 0.0f)
            {
                EditorGUI.DrawRect(iconRect, iconBackgroundColor);
            }
            GUI.DrawTexture(iconRect, GetIcon(), ScaleMode.ScaleToFit, true, 1.0f);
            EditorGUI.LabelField(new Rect(titleRect.x + 50, titleRect.y + 6, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), GetTitle(), styleTitle);
            styleTitle.normal.textColor = prevColor;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(fullRect.height);

            if (!string.IsNullOrEmpty(explanation))
            {
                // Separator
                Rect separatorRect = new Rect(titleRect.x + 4, titleRect.y + rect.height, inspectorWidth - 20 - 8, 4);
                EditorGUI.DrawRect(separatorRect, separatorColor);

                // Explanation
                Rect explanationRect = new Rect(titleRect.x + 10, separatorRect.y + separatorRect.height + 4, textAreaWidth, explanationTextHeight);
                EditorGUI.LabelField(explanationRect, explanation, explanationStyle);
            }

            bool toggle = false;
            bool refreshExplanation = false;
            var showInfoButton = new GUIContent("", "This button toggles the element's configuration on/off.\nShift-click to toggle on/off all components on this object!");
            if (showInfo)
            {
                toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), showInfoButton, GUIUtils.GetButtonStyle("EyeClose"));
            }
            else
            {
                toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), showInfoButton, GUIUtils.GetButtonStyle("EyeOpen"));
            }
            refreshExplanation = GUI.Button(new Rect(rect.x + rect.width - 26, rect.y + rect.height * 0.5f - 10, 20, 20), new GUIContent("", "This button refreshes the text explanation of this component"), GUIUtils.GetButtonStyle("Refresh"));
            if (toggle)
            {
                refreshExplanation = true;
                showInfo = !showInfo;

                if (okapiElement)
                {
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
                }

                serializedObject.ApplyModifiedProperties();
            }
            if (refreshExplanation) 
            {
                if (okapiElement)
                {
                    propExplanation.stringValue = okapiElement.UpdateExplanation();
                }
                else if (okapiScriptableObject)
                {
                    propExplanation.stringValue = okapiScriptableObject.UpdateExplanation();
                }
                serializedObject.ApplyModifiedProperties();
            }

            DisplayLogs();

            return propShowInfo.boolValue;
        }

        protected void DisplayLogs()
        {
            var okapiElement = target as OkapiElement;
            var okapiScriptableElement = target as OkapiScriptableObject;

            var logs = (okapiElement) ? (okapiElement.logs) : (okapiScriptableElement.logs);
            if (logs.Count > 0)
            {
                var rect = EditorGUILayout.BeginVertical("box");
                
                float       inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
                GUIStyle    logStyle = GUIUtils.GetLogStyle();

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

                    var content = new GUIContent(log.text);
                    Rect logRect = GUILayoutUtility.GetRect(content, logStyle, GUILayout.Width(inspectorWidth - titleRect.x - 30));
                    float height = logRect.height + 4;

                    Rect logErrorRect = new Rect(titleRect.x, rect.y, inspectorWidth - 10 - titleRect.x - 4, height);
                    EditorGUI.DrawRect(logErrorRect, color);
                    if (log.tooltip != "")
                        EditorGUI.LabelField(logErrorRect, new GUIContent("", log.tooltip));

                    logErrorRect.x += 10;
                    logErrorRect.width += 10;

                    EditorGUI.LabelField(logRect, log.text, logStyle);
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