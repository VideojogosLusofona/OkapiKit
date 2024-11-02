using UnityEngine;
using UnityEditor;
using System.Linq;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(TileSet))]
    public class TileSetEditor : OkapiBaseEditor
    {
        SerializedProperty propTiles;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTiles = serializedObject.FindProperty("tiles");
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

            EditorGUILayout.PropertyField(propTiles, new GUIContent("Tiles", "All tiles that belong to this set."), true);

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
            return $"Tileset {target.name}";
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Tiles");

            return varTexture;
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#fffaa7"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ffdf6e"));

    }
}