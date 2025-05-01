using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(OkapiConfig))]
    public class OkapiConfigEditor : UnityEditor.Editor
    {
        protected Rect                                      titleRect;
        protected Dictionary<string, SerializedProperty>    props;

        protected virtual void OnEnable()
        {
            var config = (target as OkapiConfig);
            config.ForceCheckErrors();

            AddProp("maxPingTime");
            AddProp("displayHypertags");
            AddProp("displayConditions");
            AddProp("orderMode");
            AddProp("orderScaleY");
            AddProp("orderMin");
            AddProp("orderMax");
            AddProp("orderMinZ");
            AddProp("orderMaxZ");
        }

        void AddProp(string name)
        {
            if (props == null) props = new Dictionary<string, SerializedProperty>();

            props.Add(name, serializedObject.FindProperty(name));
        }

        protected GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected string GetTitle() => "Okapi Config";

        protected Texture2D GetIcon() => GUIUtils.GetTexture("Action");

        protected (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#D7D7D7"), GUIUtils.ColorFromHex("#585858"), GUIUtils.ColorFromHex("#CBCBCB"));

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                var config = (target as OkapiConfig);
                if (config == null) return;
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(props["maxPingTime"], new GUIContent("Max. Ping Time", "When pinging an action, how long does it stays lit up."));
                EditorGUILayout.PropertyField(props["displayHypertags"], new GUIContent("Display hypertags in scene view?", "Should the hypertags be visible in the scene view?"));
                EditorGUILayout.PropertyField(props["displayConditions"], new GUIContent("Display conditions in scene view?", "Some conditions can be visualized in the scene view, should they?"));
                EditorGUILayout.PropertyField(props["displayConditions"], new GUIContent("Display conditions in scene view?", "Some conditions can be visualized in the scene view, should they?"));

                EditorGUILayout.PropertyField(props["orderMode"], new GUIContent("Order mode", "Should the order in layer or the Z position changed when ordering by Y?"));

                if (props["orderMode"].enumValueIndex == (int)OrderMode.Order)
                {
                    EditorGUILayout.PropertyField(props["orderScaleY"], new GUIContent("Y Scale", "Y position will be scaled by this factor to figure out the order in layer"));
                    EditorGUILayout.PropertyField(props["orderMin"], new GUIContent("Min. order in layer", "Minimum order in layer"));
                    EditorGUILayout.PropertyField(props["orderMax"], new GUIContent("Max. order in layer", "Maximum order in layer"));
                }
                else
                {
                    EditorGUILayout.PropertyField(props["orderScaleY"], new GUIContent("Y Scale", "Y position will be scaled by this factor to figure out the Z position"));
                    EditorGUILayout.PropertyField(props["orderMinZ"], new GUIContent("Min Z", "Minimum z value"));
                    EditorGUILayout.PropertyField(props["orderMaxZ"], new GUIContent("Max Z", "Maximum z value"));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        protected virtual bool WriteTitle()
        {
            GUIStyle styleTitle = GetTitleSyle();

            (var backgroundColor, var textColor, var separatorColor) = GetColors();

            // Background and title
            float inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
            titleRect = EditorGUILayout.BeginVertical("box");
            Rect rect = new Rect(titleRect.x, titleRect.y, inspectorWidth - titleRect.x, styleTitle.fontSize + 14);
            Rect fullRect = rect;
            EditorGUI.DrawRect(fullRect, backgroundColor);
            var prevColor = styleTitle.normal.textColor;
            styleTitle.normal.textColor = textColor;
            GUI.DrawTexture(new Rect(titleRect.x + 10, titleRect.y + 4, 32, 32), GetIcon(), ScaleMode.ScaleToFit, true, 1.0f);
            EditorGUI.LabelField(new Rect(titleRect.x + 50, titleRect.y + 6, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), GetTitle(), styleTitle);
            styleTitle.normal.textColor = prevColor;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(fullRect.height);

            DisplayLogs();

            return true;
        }

        protected void DisplayLogs()
        {
            var okapiConfig = target as OkapiConfig;

            var logs = okapiConfig.logs;
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

                    logErrorRect.x += 10;
                    logErrorRect.width += 10;

                    EditorGUI.LabelField(logErrorRect, log.text, explanationStyle);
                    rect.y += height;
                    EditorGUILayout.Space(height);
                }

                EditorGUILayout.EndVertical();
            }
        }

    }
}