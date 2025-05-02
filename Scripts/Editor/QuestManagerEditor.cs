using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(QuestManager))]
    public class QuestManagerEditor : OkapiBaseEditor
    {
        SerializedProperty startQuests;
        SerializedProperty targetInventory;
        SerializedProperty targetEquipment;

        QuestManager manager;

        protected override void OnEnable()
        {
            base.OnEnable();

            manager = (QuestManager)target;

            startQuests = serializedObject.FindProperty("startQuests");
            targetInventory = serializedObject.FindProperty("targetInventory");
            targetEquipment = serializedObject.FindProperty("targetEquipment");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(startQuests, new GUIContent("Start Quests", "These quests will be made active when it's prerequisites are fullfilled"), true);

                // Show TargetInventory only if no Inventory on this object
                if (manager.GetComponent<Inventory>() == null)
                {
                    EditorGUILayout.PropertyField(targetInventory, new GUIContent("Target Inventory", "When checking for items, I'll check this inventory."));
                }

                // Show TargetEquipment only if no Equipment on this object
                if (manager.GetComponent<Equipment>() == null)
                {
                    EditorGUILayout.PropertyField(targetEquipment, new GUIContent("Target Equipment", "When checking for items, I'll check this equipment as well."));
                }

                if (Application.isPlaying)
                {
                    DrawQuestList("Pending Quests", manager.PendingQuests, Color.yellow);
                    DrawQuestList("Active Quests", manager.ActiveQuests, Color.cyan, showObjectives: true);
                    DrawQuestList("Failed Quests", manager.FailedQuests, Color.red);
                    DrawQuestList("Completed Quests", manager.CompletedQuests, Color.green);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                manager.UpdateExplanation();
            }
        }

        void DrawQuestList(string title, List<Quest> quests, Color color, bool showObjectives = false)
        {
            if (quests == null || quests.Count == 0) return;

            EditorGUILayout.Space(10);
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = color;
            EditorGUILayout.LabelField(title, style);

            foreach (var quest in quests)
            {
                if (!quest) continue;

                EditorGUILayout.LabelField("• " + quest.displayName, EditorStyles.label);

                if (showObjectives && quest.objectiveCount > 0)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < quest.objectiveCount; i++) 
                    {
                        string desc = quest.GetObjectiveDescription(i, manager);
                        if (!string.IsNullOrEmpty(desc))
                        {
                            GUIStyle miniStyle = new GUIStyle(EditorStyles.miniLabel);
                            if (quest.IsObjectiveCompleted(i, manager))
                                miniStyle.normal.textColor = Color.green;
                            EditorGUILayout.LabelField("   - " + desc, miniStyle);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        protected override string GetTitle()
        {
            return "Quest Manager";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Inventory"); // fallback to named icon, or replace with custom sprite
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override (Color, Color, Color) GetColors()
        {
            return (
                GUIUtils.ColorFromHex("#ecc278"),  // Background
                GUIUtils.ColorFromHex("#2f4858"),  // Text
                GUIUtils.ColorFromHex("#8c5c00")   // Accent
            );
        }
    }
}
