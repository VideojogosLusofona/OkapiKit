using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{

    [CustomEditor(typeof(ActionChangeObjectState))]
    public class ActionChangeObjectStateEditor : ActionEditor
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

                var action = (target as ActionChangeObjectState);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the game object to turn on/off/toggle?"));
                EditorGUILayout.PropertyField(propState, new GUIContent("State", "What is the state of the target component after this action runs?\nOn - Activates the component\nOff - Disables the component\nToggle - If it is on, turn off. If it is off, turn it on."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}