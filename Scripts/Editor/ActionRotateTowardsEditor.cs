using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionRotateTowards))]
    public class ActionRotateTowardsEditor : ActionEditor
    {
        SerializedProperty propHasMaxSpeed;
        SerializedProperty propSpeed;
        SerializedProperty propAxisToAlign;
        SerializedProperty propTargetObject;
        SerializedProperty propTargetTag;

        protected override void OnEnable()
        {
            base.OnEnable();

            propHasMaxSpeed = serializedObject.FindProperty("hasMaxSpeed");
            propSpeed = serializedObject.FindProperty("speed");
            propAxisToAlign = serializedObject.FindProperty("axisToAlign");
            propTargetObject = serializedObject.FindProperty("targetObject");
            propTargetTag = serializedObject.FindProperty("targetTag");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionRotateTowards);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propHasMaxSpeed, new GUIContent("Has Max Speed"));
                if (propHasMaxSpeed.boolValue)
                {
                    EditorGUILayout.PropertyField(propSpeed, new GUIContent("Max Speed"));
                }
                EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align"));

                if (propTargetTag.objectReferenceValue == null)
                {
                    if (propTargetObject.objectReferenceValue == null)
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

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}