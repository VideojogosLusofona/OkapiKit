using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeSystemOption))]
    public class ActionChangeSystemOptionEditor : ActionEditor
    {
        SerializedProperty propChangeType;
        SerializedProperty propState;

        protected override void OnEnable()
        {
            base.OnEnable();

            propChangeType = serializedObject.FindProperty("changeType");
            propState = serializedObject.FindProperty("state");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeSystemOption);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of system change we want?\nMouse Cursor Visibility: Allows us to select if the mouse cursor is on/off."));

                if (propChangeType.intValue == (int)ActionChangeSystemOption.ChangeType.MouseCursorVisibility)
                {
                    EditorGUILayout.PropertyField(propState, new GUIContent("Mouse Cursor", "On: Turn on the mouse cursor\nOff: Turn off the mouse cursor\nToggle: If on, turn the mouse cursor off, otherwise turn it on."));
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