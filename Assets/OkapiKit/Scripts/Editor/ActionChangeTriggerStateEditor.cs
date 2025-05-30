using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeTriggerState))]
    public class ActionChangeTriggerStateEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propState;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propState = serializedObject.FindProperty("state");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeTriggerState);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the target trigger?"));
                EditorGUILayout.PropertyField(propState, new GUIContent("State", "On: Turn on the trigger\nOff: Turn off the trigger\nToggle: If on, turn the trigger off, otherwise turn it on."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}