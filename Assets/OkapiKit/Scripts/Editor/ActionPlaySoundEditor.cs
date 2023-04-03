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

                EditorGUILayout.PropertyField(propClip, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(propVolume, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(propPitch, new GUIContent("Pitch"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}