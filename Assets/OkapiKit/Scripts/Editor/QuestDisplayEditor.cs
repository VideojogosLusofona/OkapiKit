using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(QuestDisplay))]
    public class QuestDisplayEditor : OkapiBaseEditor
    {
        SerializedProperty      sourceQuestManager;
        SerializedProperty      questIndex;
        SerializedProperty      titleText;
        SerializedProperty      objectiveText;
        SerializedProperty      normalObjectiveColor;
        SerializedProperty      completedObjectiveColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            sourceQuestManager = serializedObject.FindProperty("sourceQuestManager");
            questIndex = serializedObject.FindProperty("questIndex");
            titleText = serializedObject.FindProperty("titleText");
            objectiveText = serializedObject.FindProperty("objectiveText");
            normalObjectiveColor = serializedObject.FindProperty("normalObjectiveColor");
            completedObjectiveColor = serializedObject.FindProperty("completedObjectiveColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(sourceQuestManager, new GUIContent("Quest Manager", "From where to fetch quest information"));
                EditorGUILayout.PropertyField(questIndex, new GUIContent("Index", "Index of the quest to use"));
                EditorGUILayout.PropertyField(titleText, new GUIContent("Title Text", "Element where to place the name of the quest"));
                EditorGUILayout.PropertyField(objectiveText, new GUIContent("Objectives", "Text elements to set with the objective text"));
                EditorGUILayout.PropertyField(normalObjectiveColor, new GUIContent("Normal Objective Color", "Color to use when the objective is ongoing"));
                EditorGUILayout.PropertyField(completedObjectiveColor, new GUIContent("Completed Objective Color", "Color to use when the objective is complete"));

                if (serializedObject.ApplyModifiedProperties())
                {
                    (target as OkapiElement).UpdateExplanation();
                }
            }
        }

        protected override string GetTitle()
        {
            return "Quest Display";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("VarDisplay"); 
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#fffaa7"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ffdf6e"));
    }
}
