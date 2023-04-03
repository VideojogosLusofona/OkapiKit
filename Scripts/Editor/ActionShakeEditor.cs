using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionShake))]
    public class ActionShakeEditor : ActionEditor
    {
        private SerializedProperty propTargetTag;
        private SerializedProperty propTargetObject;
        private SerializedProperty propStrength;
        private SerializedProperty propDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTargetTag = serializedObject.FindProperty("targetTag");
            propTargetObject = serializedObject.FindProperty("targetObject");
            propStrength = serializedObject.FindProperty("strength");
            propDuration = serializedObject.FindProperty("duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionShake);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                if (propTargetTag.objectReferenceValue == null)
                {
                    if (propTargetTag.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
                }
                EditorGUILayout.PropertyField(propStrength, new GUIContent("Strength"));
                EditorGUILayout.PropertyField(propDuration, new GUIContent("Duration"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}