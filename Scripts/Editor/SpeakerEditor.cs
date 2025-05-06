using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Speaker))]
    public class SpeakerEditor : OkapiBaseEditor
    {
        SerializedProperty displayName;
        SerializedProperty nameAlias;
        SerializedProperty nameColor;
        SerializedProperty displaySprite;
        SerializedProperty displaySpriteColor;
        SerializedProperty textColor;
        SerializedProperty characterSnd;
        SerializedProperty characterSndVolume;
        SerializedProperty characterSndPitch;

        Speaker speaker;

        protected override void OnEnable()
        {
            base.OnEnable();

            speaker = (Speaker)target;

            displayName = serializedObject.FindProperty("displayName");
            nameAlias = serializedObject.FindProperty("nameAlias");
            nameColor = serializedObject.FindProperty("nameColor");
            displaySprite = serializedObject.FindProperty("displaySprite");
            displaySpriteColor = serializedObject.FindProperty("displaySpriteColor");
            textColor = serializedObject.FindProperty("textColor");
            characterSnd = serializedObject.FindProperty("characterSnd");
            characterSndVolume = serializedObject.FindProperty("characterSndVolume");
            characterSndPitch = serializedObject.FindProperty("characterSndPitch");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display Title and Explanation
            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(displayName, new GUIContent("Display Name", "The visible name shown for this speaker."));
                EditorGUILayout.PropertyField(nameAlias, new GUIContent("Name Aliases", "Alternative names recognized for this speaker."), true);
                EditorGUILayout.PropertyField(nameColor, new GUIContent("Name Color", "Color of the displayed name text."));

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(displaySprite, new GUIContent("Display Sprite", "The sprite representing the speaker."));
                if (displaySprite.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(displaySpriteColor, new GUIContent("Sprite Color", "Tint color for the speaker's sprite."));
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(textColor, new GUIContent("Text Color", "Color of the speaker's dialogue text."));

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(characterSnd, new GUIContent("Character Sound", "Audio clip played for each character typed."));

                if (characterSnd.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(characterSndVolume, new GUIContent("Sound Volume", "Volume range for the character sound."));
                    EditorGUILayout.PropertyField(characterSndPitch, new GUIContent("Sound Pitch", "Pitch range for the character sound."));
                }

                serializedObject.ApplyModifiedProperties();
            }

            (target as OkapiElement)?.UpdateExplanation();
        }

        // OkapiBaseEditor requirements:

        protected override string GetTitle()
        {
            return string.IsNullOrEmpty(speaker.displayName) ? "Speaker" : $"Speaker {speaker.displayName}";
        }

        protected override Color GetIconBackgroundColor()
        {
            return (speaker.displaySprite) ? Color.black : base.GetIconBackgroundColor();
        }

        protected override Texture2D GetIcon()
        {
            return (speaker.displaySprite) ? AssetPreview.GetAssetPreview(speaker.displaySprite) : GUIUtils.GetTexture("SpeakerIcon");
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
                GUIUtils.ColorFromHex("#C7D8D8"), GUIUtils.ColorFromHex("#4F4F8C"), GUIUtils.ColorFromHex("#8080D0")
            );
        }
    }
}
