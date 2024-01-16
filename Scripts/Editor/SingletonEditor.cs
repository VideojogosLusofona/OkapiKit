using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Singleton))]
    public class SingletonEditor : OkapiBaseEditor
    {
        SerializedProperty propTag;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTag = serializedObject.FindProperty("singletonTag");
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
            return GUIUtils.GetTexture("Singleton");
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
            return "Singleton";
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffcaca"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff6060"));

        protected void StdEditor(bool useOriginalEditor = true)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propTag, new GUIContent("Singleton Tag", "Selects the tag that identifies this singleton.\nOnly one object with this tag will be able to exist on scene start. If another object with this tag comes from another scene (being a singleton), the more recent object will be deleted."));
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

            EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();
            (target as Singleton).UpdateExplanation();

            // Draw old editor, need it for now
            if (useOriginalEditor)
            {
                base.OnInspectorGUI();
            }

        }

    }
}