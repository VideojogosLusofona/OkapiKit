using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : OkapiBaseEditor
    {
        SerializedProperty propDefaultMixerOutput;

        protected override void OnEnable()
        {
            base.OnEnable();

            propDefaultMixerOutput = serializedObject.FindProperty("defaultMixerOutput");
        }

        public override void OnInspectorGUI()
        {
            if (WriteTitle())
            {
                StdEditor(false);
            }
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("SoundManager");
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
            return "Sound Manager";
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffcaca"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff6060"));

        protected void StdEditor(bool useOriginalEditor = true)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propDefaultMixerOutput, new GUIContent("Default Mixer Output"));
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();
            (target as SoundManager).UpdateExplanation();

            // Draw old editor, need it for now
            if (useOriginalEditor)
            {
                base.OnInspectorGUI();
            }

        }

    }
}