using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Resource))]
    public class ResourceEditor : OkapiBaseEditor
    {
        SerializedProperty typeProperty;
        SerializedProperty startValueProperty;
        SerializedProperty flags;
        SerializedProperty globalCooldown;
        SerializedProperty cooldownPerSource;

        Resource resource;

        protected override void OnEnable()
        {
            base.OnEnable();

            resource = (Resource)target;

            typeProperty = serializedObject.FindProperty("type");
            startValueProperty = serializedObject.FindProperty("startValue");
            flags = serializedObject.FindProperty("flags");
            globalCooldown = serializedObject.FindProperty("globalCooldown");
            cooldownPerSource = serializedObject.FindProperty("cooldownPerSource");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(typeProperty, new GUIContent("Type", "Resource type"));
                EditorGUILayout.PropertyField(flags, new GUIContent("Flags", "Options for this resource handler"));

                Resource.Flags f = (Resource.Flags)flags.enumValueFlag;

                if ((f & Resource.Flags.UseCooldownOnChanges) != 0)
                {
                    EditorGUILayout.PropertyField(globalCooldown, new GUIContent("Global cooldown", "Changes can't happen faster than this time."));
                }
                if ((f & Resource.Flags.UseCooldownPerSource) != 0)
                {
                    EditorGUILayout.PropertyField(cooldownPerSource, new GUIContent("Per source cooldown", "Changes from a single source can't happen faster than this time."));
                }

                if ((f & Resource.Flags.OverrideStartValue) != 0)
                { 
                    EditorGUILayout.PropertyField(startValueProperty, new GUIContent("Start value", "Instead of using the default value, use this."));
                }

                if (resource != null && resource.type != null)
                {
                    EditorGUILayout.Space(10);

                    float newValue = EditorGUILayout.Slider(resource.type.displayName, resource.resource, 0, resource.type.maxValue);
                    if (!Mathf.Approximately(newValue, resource.resource))
                    {
                        Undo.RecordObject(resource, "Change Resource Value");
                        resource.SetResource(newValue);
                        EditorUtility.SetDirty(resource);
                    }

                    DrawProgressBar(resource);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProgressBar(Resource resource)
        {
            float normalizedValue = resource.normalizedResource;
            string progressText = $"{resource.resource:0.##}/{resource.type.maxValue:0.##}";
            Color barColor = resource.type.displayBarColor;

            Rect rect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1.0f)); // Background

            Rect fill = new Rect(rect.x, rect.y, rect.width * normalizedValue, rect.height);
            EditorGUI.DrawRect(fill, barColor);

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            EditorGUI.LabelField(rect, progressText, style);
        }

        protected override string GetTitle()
        {
            return (resource.type != null) ? resource.type.displayName : "Resource";
        }

        protected override Texture2D GetIcon()
        {
            Texture2D ret = null;

            if ((resource.type != null) && (resource.type.displaySprite != null))
            {
                ret = AssetPreview.GetAssetPreview(resource.type.displaySprite);
            }
            else ret = GUIUtils.GetTexture("Resource");

            return ret;
        }

        protected override (Color, Color, Color) GetColors()
        {
            Color c = GUIUtils.ColorFromHex("#D0FFFF");
            if (resource.type != null)
                c = resource.type.displayBarColor;

            return (c, GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#86CBFF"));
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }
    }
}
