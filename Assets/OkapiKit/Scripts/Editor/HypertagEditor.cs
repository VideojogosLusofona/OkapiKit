using UnityEngine;
using UnityEditor;
using System.Linq;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Hypertag))]
    public class HypertagEditor : OkapiBaseEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);
            }
        }

        protected void StdEditor(bool useOriginalEditor = true)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."), true);

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiScriptableObject).UpdateExplanation();

            // Draw old editor, need it for now
            if (useOriginalEditor)
            {
                base.OnInspectorGUI();
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
            OkapiScriptableObject varInstance = target as Hypertag;

            return $"{varInstance.name}";
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Variable");

            return varTexture;
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#fdd0f6"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#fea0f0"));
    }
}