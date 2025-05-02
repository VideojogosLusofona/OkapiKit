using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(SpriteEffect))]
    public class SpriteEffectEditor : OkapiBaseEditor
    {
        SerializedProperty effectsProp;
        SerializedProperty paletteProp;
        SerializedProperty inverseFactorProp;
        SerializedProperty flashColorProp;
        SerializedProperty outlineWidthProp;
        SerializedProperty outlineColorProp;
        SerializedProperty createMaterialCopyProp;

        SpriteEffect spriteEffect;

        protected override void OnEnable()
        {
            base.OnEnable();

            spriteEffect = (SpriteEffect)target;

            effectsProp = serializedObject.FindProperty("effects");
            paletteProp = serializedObject.FindProperty("palette");
            inverseFactorProp = serializedObject.FindProperty("inverseFactor");
            flashColorProp = serializedObject.FindProperty("flashColor");
            outlineWidthProp = serializedObject.FindProperty("outlineWidth");
            outlineColorProp = serializedObject.FindProperty("outlineColor");
            createMaterialCopyProp = serializedObject.FindProperty("createMaterialCopy");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(effectsProp);
                EditorGUILayout.PropertyField(createMaterialCopyProp);

                if (spriteEffect.colorRemapEnable)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Color Remap", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(paletteProp);
                }

                if (spriteEffect.inverseEnable)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Inverse", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(inverseFactorProp);
                }

                if (spriteEffect.flashEnable)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Flash", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(flashColorProp);
                }

                if (spriteEffect.outlineEnable)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(outlineWidthProp);
                    EditorGUILayout.PropertyField(outlineColorProp);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected override string GetTitle()
        {
            return "Sprite Effect";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Effect"); // fallback to named icon, or replace with custom sprite
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
