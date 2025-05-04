using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : OkapiBaseEditor
    {
        SerializedProperty limitedProp;
        SerializedProperty maxSlotsProp;
        SerializedProperty combatTextEnable;
        SerializedProperty combatTextDuration;
        SerializedProperty combatTextReceivedItemColor;
        SerializedProperty combatTextLostItemColor;

        Inventory inventory;

        protected override void OnEnable()
        {
            base.OnEnable();

            inventory = (Inventory)target;

            limitedProp = serializedObject.FindProperty("limited");
            maxSlotsProp = serializedObject.FindProperty("maxSlots");
            combatTextEnable = serializedObject.FindProperty("combatTextEnable");
            combatTextDuration = serializedObject.FindProperty("combatTextDuration");
            combatTextReceivedItemColor = serializedObject.FindProperty("combatTextReceivedItemColor");
            combatTextLostItemColor = serializedObject.FindProperty("combatTextLostItemColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(limitedProp);

                if (limitedProp.boolValue)
                {
                    EditorGUILayout.PropertyField(maxSlotsProp);
                }
                EditorGUILayout.PropertyField(combatTextEnable, new GUIContent("Combat Text Enable", "Should combat text spawn when a new item is received/lost?"));
                if (combatTextEnable.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(combatTextDuration, new GUIContent("Duration", "Combat text duration"));
                    EditorGUILayout.PropertyField(combatTextReceivedItemColor, new GUIContent("Received Item Color", "Color to display text when an item is received"));
                    EditorGUILayout.PropertyField(combatTextLostItemColor, new GUIContent("Lost Item Color", "Color to display text when an item is lost"));
                    EditorGUI.indentLevel--;
                }

                serializedObject.ApplyModifiedProperties();

                if (Application.isPlaying)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Current State", EditorStyles.boldLabel);

                    foreach (var (slot, item, count) in inventory)
                    {
                        if (item == null || count <= 0) continue;

                        EditorGUILayout.BeginHorizontal();

                        // Show sprite preview if available
                        Texture2D icon = item.displaySprite ? AssetPreview.GetAssetPreview(item.displaySprite) : null;
                        if (icon != null)
                        {
                            GUILayout.Label(icon, GUILayout.Width(32), GUILayout.Height(32));
                        }
                        else
                        {
                            GUILayout.Space(36); // keep alignment
                        }

                        // Show item info
                        GUILayout.Label($"Slot {slot}: {item.displayName} x{count}", GUILayout.ExpandWidth(true));

                        EditorGUILayout.EndHorizontal();
                    }

                    if (!inventory.HasItems())
                    {
                        EditorGUILayout.HelpBox("Inventory is empty.", MessageType.Info);
                    }
                }

            }
        }

        // OkapiBaseEditor requirements

        protected override string GetTitle()
        {
            return "Inventory";
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
