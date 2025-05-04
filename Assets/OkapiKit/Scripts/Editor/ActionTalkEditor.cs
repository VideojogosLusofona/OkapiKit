
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionTalk))]
    public class ActionTalkEditor : ActionEditor
    {
        SerializedProperty dialogueKey;

        protected override void OnEnable()
        {
            base.OnEnable();

            dialogueKey = serializedObject.FindProperty("dialogueKey");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = target as Action;
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(dialogueKey, new GUIContent("Dialogue", "Dialogue to trigger."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}
