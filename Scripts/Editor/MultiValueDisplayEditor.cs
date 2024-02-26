using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MultiValueDisplay))]
    public class MultiValueDisplayEditor : OkapiBaseEditor
    {
        protected SerializedProperty propValues;

        protected virtual string typeOfDisplay { get; }

        protected override void OnEnable()
        {
            base.OnEnable();

            propValues = serializedObject.FindProperty("values");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);
            }
        }

        protected virtual void StdEditor(bool useOriginalEditor = true, bool isFinal = true)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propValues, new GUIContent("Values", "Values to display.\nFirst of these will replace {0} on the format string, the second will replace {1} and so forth."), true);

            if (isFinal)
            {
                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."), true);

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                // Draw old editor, need it for now
                if (useOriginalEditor)
                {
                    base.OnInspectorGUI();
                }
            }
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override string GetTitle()
        {
            string varNames = "[UNDEFINED]";

            // Get all var names
            var t = target as MultiValueDisplay;
            if (t != null)
            {
                varNames = t.GetVariableNames();
            }

            return $"Display {varNames} as {typeOfDisplay}";
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("VarDisplay");

            return varTexture;
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#fffaa7"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ffdf6e"));

    }
}