using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Equipment))]
    public class EquipmentEditor : OkapiBaseEditor
    {
        SerializedProperty availableSlots;
        SerializedProperty linkedInventory;
        SerializedProperty combatTextEnable;
        SerializedProperty combatTextDuration;
        SerializedProperty combatTextEquippedItemColor;
        SerializedProperty combatTextUnequippedItemColor;

        Equipment equipment;

        protected override void OnEnable()
        {
            base.OnEnable();

            equipment = (Equipment)target;

            availableSlots = serializedObject.FindProperty("availableSlots");
            linkedInventory = serializedObject.FindProperty("linkedInventory");
            combatTextEnable = serializedObject.FindProperty("combatTextEnable");
            combatTextDuration = serializedObject.FindProperty("combatTextDuration");
            combatTextEquippedItemColor = serializedObject.FindProperty("combatTextEquippedItemColor");
            combatTextUnequippedItemColor = serializedObject.FindProperty("combatTextUnequippedItemColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(linkedInventory);
                EditorGUILayout.PropertyField(availableSlots);

                EditorGUILayout.PropertyField(combatTextEnable, new GUIContent("Combat Text Enable", "Should combat text spawn when a new item is equipped/unequipped?"));
                if (combatTextEnable.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(combatTextDuration, new GUIContent("Duration", "Combat text duration"));
                    EditorGUILayout.PropertyField(combatTextEquippedItemColor, new GUIContent("Received Item Color", "Color to display text when an item is equipped"));
                    EditorGUILayout.PropertyField(combatTextUnequippedItemColor, new GUIContent("Lost Item Color", "Color to display text when an item is unequipped"));
                    EditorGUI.indentLevel--;
                }

                serializedObject.ApplyModifiedProperties();

                if (Application.isPlaying)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Equipped Items", EditorStyles.boldLabel);

                    foreach (var slot in equipment.GetAvailableSlots())
                    {
                        var item = equipment.GetItem(slot);
                        EditorGUILayout.BeginHorizontal();

                        // Slot name
                        EditorGUILayout.LabelField(slot.name, GUILayout.Width(100));

                        if (item != null)
                        {
                            Sprite sprite = item.displaySprite;

                            if (sprite != null)
                            {
                                Texture2D tex = AssetPreview.GetAssetPreview(sprite);
                                if (tex != null)
                                    GUILayout.Label(tex, GUILayout.Width(32), GUILayout.Height(32));
                                else
                                    GUILayout.Label("", GUILayout.Width(32), GUILayout.Height(32)); // fallback placeholder
                            }
                            else
                            {
                                GUILayout.Label("", GUILayout.Width(32), GUILayout.Height(32)); // placeholder
                            }

                            EditorGUILayout.LabelField(item.displayName ?? item.name);
                        }
                        else
                        {
                            GUILayout.Space(32); // maintain layout for empty slot
                            EditorGUILayout.LabelField("(empty)");
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        // OkapiBaseEditor requirements

        protected override string GetTitle()
        {
            return "Equipment";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Equipment"); // fallback to named icon, or replace with custom sprite
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
