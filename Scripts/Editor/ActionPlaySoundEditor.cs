using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionPlaySound))]
    public class ActionPlaySoundEditor : ActionEditor
    {
        SerializedProperty propClip;
        SerializedProperty propVolume;
        SerializedProperty propPitch;

        protected override void OnEnable()
        {
            base.OnEnable();

            propClip = serializedObject.FindProperty("clip");
            propVolume = serializedObject.FindProperty("volume");
            propPitch = serializedObject.FindProperty("pitch");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionPlaySound);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propClip, new GUIContent("Clip", "What is the sound you want to play?"));
                EditorGUILayout.PropertyField(propVolume, new GUIContent("Volume", "What's the volume you want to play the sound?\nOne is the default volume.\nYou might want to specify a range to make the sound seem a bit different each time it plays."));
                EditorGUILayout.PropertyField(propPitch, new GUIContent("Pitch", "What's the pitch/speed you want to play the sound?\nOne is the default pitch/speed.\nYou might want to specify a range to make the sound seem a bit different each time it plays."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}