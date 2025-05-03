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
                EditorGUILayout.PropertyField(propHasMaxSpeed, new GUIContent("Has Max Speed", "Is there a limit to the rotation speed?\nIf not, this object will be always pointing towards the target."));
                if (propHasMaxSpeed.boolValue)
                {
                    EditorGUILayout.PropertyField(propSpeed, new GUIContent("Max Speed", "What's the maximum angular speed, in degrees per second?"));
                }
                EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align", "Which of the axis you want to align to the target?"));

                if (propTargetTag.objectReferenceValue == null)
                {
                    if (propTargetObject.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "What's the target's tag?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the target object?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the target object?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "What's the target's tag?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}