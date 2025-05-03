using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionQuestFail))]
    public class ActionQuestFailEditor : ActionEditor
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

                var action = (target as ActionQuestFail);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(questManager, new GUIContent("Quest Manager", "Target quest manager - who fails this quest?"));
                EditorGUILayout.PropertyField(quest, new GUIContent("Quest", "Quest to fail"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as ActionEffect).UpdateExplanation();
                }
            }
        }
    }
}