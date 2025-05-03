using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionQuestGive))]
    public class ActionQuestGiveEditor : ActionEditor
    {
        SerializedProperty questManager;
        SerializedProperty quest;

        protected override void OnEnable()
        {
            base.OnEnable();

            questManager = serializedObject.FindProperty("questManager");
            quest = serializedObject.FindProperty("quest");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionQuestGive);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(questManager, new GUIContent("Quest Manager", "Target quest manager - who receives this quest?"));
                EditorGUILayout.PropertyField(quest, new GUIContent("Quest", "Quest to give to player"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}