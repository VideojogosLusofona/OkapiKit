using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(DialogueDisplayJRPG))]
    public class DialogueDisplayJRPGEditor : OkapiBaseEditor
    {
        SerializedProperty fadeTimeProp;
        SerializedProperty speakerContainerProp;
        SerializedProperty speakerPortraitProp;
        SerializedProperty speakerNameProp;
        SerializedProperty dialogueTextProp;
        SerializedProperty dialogueDefaultColorProp;
        SerializedProperty appearMethodProp;
        SerializedProperty timePerCharacterProp;
        SerializedProperty timePerCharacterSkipProp;
        SerializedProperty optionSeparatorProp;
        SerializedProperty optionsProp;
        SerializedProperty optionCooldownProp;
        SerializedProperty continueStatusObjectProp;
        SerializedProperty skipStatusObjectProp;
        SerializedProperty doneStatusObjectProp;
        SerializedProperty changeSpeakerSndProp;
        SerializedProperty endSndProp;
        SerializedProperty skipSndProp;
        SerializedProperty optionSndProp;
        SerializedProperty skipInputProp;
        SerializedProperty optionSelectionInputProp;

        DialogueDisplayJRPG dialogueDisplayJRPG;

        protected override void OnEnable()
        {
            base.OnEnable();

            dialogueDisplayJRPG = (DialogueDisplayJRPG)target;

            fadeTimeProp = serializedObject.FindProperty("fadeTime");
            speakerContainerProp = serializedObject.FindProperty("speakerContainer");
            speakerPortraitProp = serializedObject.FindProperty("speakerPortrait");
            speakerNameProp = serializedObject.FindProperty("speakerName");
            dialogueTextProp = serializedObject.FindProperty("dialogueText");
            dialogueDefaultColorProp = serializedObject.FindProperty("dialogueDefaultColor");
            appearMethodProp = serializedObject.FindProperty("appearMethod");
            timePerCharacterProp = serializedObject.FindProperty("timePerCharacter");
            timePerCharacterSkipProp = serializedObject.FindProperty("timePerCharacterSkip");
            optionSeparatorProp = serializedObject.FindProperty("optionSeparator");
            optionsProp = serializedObject.FindProperty("options");
            optionCooldownProp = serializedObject.FindProperty("optionCooldown");
            continueStatusObjectProp = serializedObject.FindProperty("continueStatusObject");
            skipStatusObjectProp = serializedObject.FindProperty("skipStatusObject");
            doneStatusObjectProp = serializedObject.FindProperty("doneStatusObject");
            changeSpeakerSndProp = serializedObject.FindProperty("changeSpeakerSnd");
            endSndProp = serializedObject.FindProperty("endSnd");
            skipSndProp = serializedObject.FindProperty("skipSnd");
            optionSndProp = serializedObject.FindProperty("optionSnd");
            skipInputProp = serializedObject.FindProperty("skipInput");
            optionSelectionInputProp = serializedObject.FindProperty("optionSelectionInput");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display Title and Explanation
            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(fadeTimeProp, new GUIContent("Fade Time", "The duration for fading in/out the dialogue."));
                EditorGUILayout.PropertyField(speakerContainerProp, new GUIContent("Speaker Container", "The container holding the speaker's portrait and name."));
                EditorGUILayout.PropertyField(speakerPortraitProp, new GUIContent("Speaker Portrait", "The image of the speaker's portrait."));
                EditorGUILayout.PropertyField(speakerNameProp, new GUIContent("Speaker Name", "Text element for the name of the speaker."));
                EditorGUILayout.PropertyField(dialogueTextProp, new GUIContent("Dialogue Text", "Text element for the dialogue."));
                EditorGUILayout.PropertyField(dialogueDefaultColorProp, new GUIContent("Default Dialogue Color", "The default color for dialogue text, when there's no speaker defined."));
                EditorGUILayout.PropertyField(appearMethodProp, new GUIContent("Appearance Method", "How the dialogue text appears: all at once or per character."));
                var appearMethod = (DialogueDisplayJRPG.AppearMethod)appearMethodProp.enumValueIndex;
                if (appearMethod == DialogueDisplayJRPG.AppearMethod.PerChar)
                {
                    EditorGUILayout.PropertyField(timePerCharacterProp, new GUIContent("Time Per Character", "Time to display each character."), true);
                    EditorGUILayout.PropertyField(timePerCharacterSkipProp, new GUIContent("Time Per Character (Skip)", "Time to display each character when skipping text."), true);
                }
                EditorGUILayout.PropertyField(optionSeparatorProp, new GUIContent("Option Separator", "The UI element separating the dialogue from the options."));
                EditorGUILayout.PropertyField(optionsProp, new GUIContent("Options", "List of options available during the dialogue. The size of this array define the maximum number of options available."), true);
                EditorGUILayout.PropertyField(optionCooldownProp, new GUIContent("Option Cooldown", "Cooldown time between option selections."));
                EditorGUILayout.PropertyField(continueStatusObjectProp, new GUIContent("Continue Status", "Object showing when the player can continue the dialogue. This object will be enabled if there is more text to display."));
                EditorGUILayout.PropertyField(skipStatusObjectProp, new GUIContent("Skip Status", "Object showing when the player can skip the dialogue."));
                EditorGUILayout.PropertyField(doneStatusObjectProp, new GUIContent("Done Status", "Object showing when the dialogue is completed. This object will be enabled if the dialogue is on the final 'page'."));
                EditorGUILayout.PropertyField(changeSpeakerSndProp, new GUIContent("Change Speaker Sound", "Sound to play when the speaker changes."));
                EditorGUILayout.PropertyField(endSndProp, new GUIContent("End Sound", "Sound to play when the dialogue ends."));
                EditorGUILayout.PropertyField(skipSndProp, new GUIContent("Skip Sound", "Sound to play when the dialogue is skipped."));
                EditorGUILayout.PropertyField(optionSndProp, new GUIContent("Option Sound", "Sound to play when an option is selected."));
                EditorGUILayout.PropertyField(skipInputProp, new GUIContent("Skip Input", "The input control for skipping dialogue."));
                EditorGUILayout.PropertyField(optionSelectionInputProp, new GUIContent("Option Selection Input", "The input control for selecting options."));

                serializedObject.ApplyModifiedProperties();
            }

            (target as OkapiElement)?.UpdateExplanation();
        }

        // OkapiBaseEditor requirements:

        protected override string GetTitle()
        {
            return "JRPG Dialogue Display";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Talk"); // Replace with actual texture for the DialogueDisplayJRPG
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
