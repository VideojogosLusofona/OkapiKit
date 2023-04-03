using UnityEngine;
using UnityEditor;
using System.Linq;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(HypertaggedObject))]
    [CanEditMultipleObjects]
    public class HypertaggedObjectEditor : OkapiBaseEditor
    {
        SerializedProperty propHypertags;

        protected override void OnEnable()
        {
            base.OnEnable();

            propHypertags = serializedObject.FindProperty("hypertags");
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

            EditorGUILayout.PropertyField(propHypertags, new GUIContent("Tags"), true);
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"), true);

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();

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
            string ret = (target as HypertaggedObject).GetTagString();

            if (ret == "") return "Hypertag";

            return ret;
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Tag");

            return varTexture;
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#fdd0f6"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#fea0f0"));

    }
}