using UnityEditor;
using UnityEngine;
using OkapiKit;
using OkapiKit.Editor;
using UnityEditor.Graphs;

namespace OkapiKitEditor
{
    [CustomEditor(typeof(ItemDisplay))]
    public class ItemDisplayEditor : OkapiBaseEditor
    {
        SerializedProperty itemSource;
        SerializedProperty inventory;
        SerializedProperty equipment;
        SerializedProperty inventorySlot;
        SerializedProperty equipmentSlot;
        SerializedProperty imageRef;
        SerializedProperty quantityRef;

        protected override void OnEnable()
        {
            base.OnEnable();

            itemSource = serializedObject.FindProperty("itemSource");
            inventory = serializedObject.FindProperty("inventory");
            equipment = serializedObject.FindProperty("equipment");
            inventorySlot = serializedObject.FindProperty("inventorySlot");
            equipmentSlot = serializedObject.FindProperty("equipmentSlot");
            imageRef = serializedObject.FindProperty("imageRef");
            quantityRef = serializedObject.FindProperty("quantityRef");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(itemSource);

                if ((ItemDisplay.ItemSource)itemSource.enumValueIndex == ItemDisplay.ItemSource.Inventory)
                {
                    EditorGUILayout.PropertyField(inventory);
                    EditorGUILayout.PropertyField(inventorySlot);
                }
                else
                {
                    EditorGUILayout.PropertyField(equipment);
                    EditorGUILayout.PropertyField(equipmentSlot);
                }

                EditorGUILayout.PropertyField(imageRef);
                EditorGUILayout.PropertyField(quantityRef);

                if (serializedObject.ApplyModifiedProperties())
                {
                    (target as OkapiElement).UpdateExplanation();
                }
            }
        }

        protected override string GetTitle()
        {
            if ((ItemDisplay.ItemSource)itemSource.enumValueIndex == ItemDisplay.ItemSource.Inventory)
            {
                return "Inventory Item Display";
            }
            else
            {
                return "Equipped Item Display";

            }
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("VarDisplay"); // fallback to named icon or replace with a sprite
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
