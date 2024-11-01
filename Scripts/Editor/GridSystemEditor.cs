using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(GridSystem))]
    public class GridSystemEditor : OkapiBaseEditor
    {
        SerializedProperty propGridcolliders;

        protected override void OnEnable()
        {
            base.OnEnable();

            propGridcolliders = serializedObject.FindProperty("gridcolliders");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propGridcolliders, new GUIContent("Grid Colliders", "These colliders will be used when we detect if the children grid objects can move through the environment.\nNormally, this is a list of the Tilemap Colliders or Composite Colliders of the tilemap below."));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();
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
            return "Grid System";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("GridObject");
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#93c08b"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#527d4a"));
    }
}