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
            propShowInfo = serializedObject.FindProperty("_showInfo");
            propDescription = serializedObject.FindProperty("description");
            propExplanation = serializedObject.FindProperty("_explanation");
        }

        protected Rect titleRect;

        protected virtual bool WriteTitle()
        {
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
            EditorGUI.DrawRect(fullRect, backgroundColor);
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
                propExplanation.stringValue = (target as OkapiElement).UpdateExplanation();
                serializedObject.ApplyModifiedProperties();
            }

            return propShowInfo.boolValue;
        }

    }
}