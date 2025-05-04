using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(CombatTextManager))]
    public class CombatTextManagerEditor : OkapiBaseEditor
    {
        SerializedProperty textPrefab;
        SerializedProperty defaultTime;
        SerializedProperty movementVector;
        SerializedProperty fadeRate;

        CombatTextManager combatText;

        protected override void OnEnable()
        {
            base.OnEnable();

            combatText = (CombatTextManager)target;

            textPrefab = serializedObject.FindProperty("textPrefab");
            defaultTime = serializedObject.FindProperty("defaultTime");
            movementVector = serializedObject.FindProperty("movementVector");
            fadeRate = serializedObject.FindProperty("fadeRate");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(textPrefab, new GUIContent("Text Prefab", "Element to spawn on each combat text instance."));
                EditorGUILayout.PropertyField(movementVector, new GUIContent("Direction", "Where to move the combat text (units/second)."));
                EditorGUILayout.PropertyField(defaultTime, new GUIContent("Duration", "Default time for the combat text movement."));
                EditorGUILayout.PropertyField(fadeRate, new GUIContent("Fade Duration", "Fade duration for the combat text."));

                serializedObject.ApplyModifiedProperties();
            }
        }

        // OkapiBaseEditor requirements

        protected override string GetTitle()
        {
            return "Combat Text";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("CombatText"); // fallback to named icon, or replace with custom sprite
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override (Color, Color, Color) GetColors()
        {
            return (
                GUIUtils.ColorFromHex("#ecc278"),  // Background
                GUIUtils.ColorFromHex("#2f4858"),  // Text
                GUIUtils.ColorFromHex("#8c5c00")   // Accent
            );
        }
    }
}
