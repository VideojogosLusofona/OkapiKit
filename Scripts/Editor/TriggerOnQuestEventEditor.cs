using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(TriggerOnQuestEvent))]
    public class TriggerOnQuestEventEditor : TriggerEditor
    {
        SerializedProperty eventType;
        SerializedProperty questManager;
        SerializedProperty quest;

        protected override void OnEnable()
        {
            base.OnEnable();

            eventType = serializedObject.FindProperty("eventType");
            questManager = serializedObject.FindProperty("questManager");
            quest = serializedObject.FindProperty("quest");
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Quest");

            return varTexture;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var trigger = (target as TriggerOnQuestEvent);
                if (trigger == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(eventType, new GUIContent("Event type", "What kind of event we want to detect."));
                if (trigger.GetComponent<QuestManager>() == null)
                {
                    EditorGUILayout.PropertyField(questManager, new GUIContent("Quest Manager", "Location of the quest manager."));
                }
                EditorGUILayout.PropertyField(quest, new GUIContent("Quest", "Which quest to listen for. If left undefined, this is triggered by any quest."));
                EditorGUI.EndChangeCheck();

                ActionPanel();
            }
        }
    }
}