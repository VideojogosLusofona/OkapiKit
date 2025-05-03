using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionFlash))]
    public class ActionFlashEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propColor;
        SerializedProperty propDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propColor = serializedObject.FindProperty("color");
            propDuration = serializedObject.FindProperty("duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionFlash);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the target renderer to flash?\nNote that only renderers that have a material will be able to flash, like sprite renderers. Others might work, but the behaviour might be unexpected."));
                EditorGUILayout.PropertyField(propColor, new GUIContent("Color", "Color to flash the object."));
                EditorGUILayout.PropertyField(propDuration, new GUIContent("Duration", "For how long to flash the object, in seconds."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}