using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeCollider))]
    public class ActionChangeColliderEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propState;
        SerializedProperty propChangeState;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propState = serializedObject.FindProperty("state");
            propChangeState = serializedObject.FindProperty("changeState");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeCollider);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the collider to change?"));
                EditorGUILayout.PropertyField(propState, new GUIContent("State", "What is the property of the collider to change?\nIsTrigger - Set if state of the trigger flag of the collider"));

                var state = (ActionChangeCollider.State)propState.enumValueIndex;
                if (state == ActionChangeCollider.State.IsTrigger)
                {
                    EditorGUILayout.PropertyField(propChangeState, new GUIContent("Change State", "What is the new state of the trigger flag after this action runs?\nOn - Collider is now a trigger\nOff - Collider is not a trigger\nToggle - If it is trigger, stop being one, and vice-versa."));
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