using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Movement))]
    public class MovementEditor : OkapiBaseEditor
    {
        protected SerializedProperty propHasConditions;
        protected SerializedProperty propConditions;

        protected override void OnEnable()
        {
            base.OnEnable();

            propHasConditions = serializedObject.FindProperty("hasConditions");
            propConditions = serializedObject.FindProperty("conditions");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                DefaultMovementEditor();

                StdEditor(false);
            }
        }

        protected void DefaultMovementEditor()
        {
            EditorGUILayout.PropertyField(propHasConditions, new GUIContent("Conditional Movement", "If active, this movement will only can affect the object if the conditions are met"));
            if (propHasConditions.boolValue)
            {
                // Display tags
                EditorGUILayout.PropertyField(propConditions, new GUIContent("Conditions", "These conditions have to be met for this movement to affect the object."), true);
            }
        }

        protected void StdEditor(bool useOriginalEditor = true)
        {
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
            return (target as Movement).GetTitle();
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Movement");
            return varTexture;
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffcaca"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff6060"));
    }
}