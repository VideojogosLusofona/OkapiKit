using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ResourceType))]
    public class ResourceTypeEditor : OkapiBaseEditor
    {
        SerializedProperty displayNameProp;
        SerializedProperty displaySpriteColorProp;
        SerializedProperty displaySpriteProp;
        SerializedProperty displayTextColorProp;
        SerializedProperty displayBarColorProp;
        SerializedProperty defaultValueProp;
        SerializedProperty maxValueProp;

        ResourceType resourceType;

        protected override void OnEnable()
        {
            base.OnEnable();

            resourceType = (ResourceType)target;

            displayNameProp = serializedObject.FindProperty("displayName");
            displaySpriteColorProp = serializedObject.FindProperty("displaySpriteColor");
            displaySpriteProp = serializedObject.FindProperty("displaySprite");
            displayTextColorProp = serializedObject.FindProperty("displayTextColor");
            displayBarColorProp = serializedObject.FindProperty("displayBarColor");
            defaultValueProp = serializedObject.FindProperty("defaultValue");
            maxValueProp = serializedObject.FindProperty("maxValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(displayNameProp);
                EditorGUILayout.PropertyField(displaySpriteProp);

                if (displaySpriteProp.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(displaySpriteColorProp);
                }

                EditorGUILayout.PropertyField(displayTextColorProp);
                EditorGUILayout.PropertyField(displayBarColorProp);
                EditorGUILayout.PropertyField(defaultValueProp);
                EditorGUILayout.PropertyField(maxValueProp);

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override string GetTitle()
        {
            return (!string.IsNullOrEmpty(resourceType.displayName)) ? resourceType.displayName : resourceType.name;
        }

        protected override Color GetIconBackgroundColor()
        {
            return (resourceType.displaySprite) ? Color.black : base.GetIconBackgroundColor();
        }

        protected override Texture2D GetIcon()
        {
            return (resourceType.displaySprite) ? AssetPreview.GetAssetPreview(resourceType.displaySprite) : GUIUtils.GetTexture("Resource");
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
            return (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#969721"));
        }
    }
}
