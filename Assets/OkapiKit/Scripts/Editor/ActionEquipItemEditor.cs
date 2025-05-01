using UnityEditor;
using UnityEngine;
using static OkapiKit.ActionEquipItem;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionEquipItem))]
    public class ActionEquipItemEditor : ActionEditor
    {
        SerializedProperty itemSource;
        SerializedProperty item;
        SerializedProperty inventory;
        SerializedProperty inventorySlot;
        SerializedProperty equipment;
        SerializedProperty unequipIfEquipped;

        protected override void OnEnable()
        {
            base.OnEnable();

            itemSource = serializedObject.FindProperty("itemSource");
            item = serializedObject.FindProperty("item");
            inventory = serializedObject.FindProperty("inventory");
            inventorySlot = serializedObject.FindProperty("inventorySlot");
            equipment = serializedObject.FindProperty("equipment");
            unequipIfEquipped = serializedObject.FindProperty("unequipIfEquipped");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = target as ActionEquipItem;
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(equipment, new GUIContent("Equipment", "The equipment manager to be changed."));

                EditorGUILayout.PropertyField(itemSource, new GUIContent("Source", "What to equip?"));
                var source = (ActionEquipItem.ItemSource)itemSource.enumValueIndex;
                switch (source)
                {
                    case ItemSource.Explicit:
                        EditorGUILayout.PropertyField(item, new GUIContent("Item", "The item to equip in the slot."));
                        break;
                    case ItemSource.InventoryItem:
                        EditorGUILayout.PropertyField(item, new GUIContent("Item", "The item to equip in the slot."));
                        EditorGUILayout.PropertyField(inventory, new GUIContent("Inventory", "The source inventory for the item, if it exists.\nIt has to contain this item to be equipped."));
                        break;
                    case ItemSource.InventorySlot:
                        EditorGUILayout.PropertyField(inventory, new GUIContent("Inventory", "The item to equip from the given inventory, if it exists."));
                        EditorGUILayout.PropertyField(inventorySlot, new GUIContent("Slot", "The slot from which to equip, if possible."));
                        break;
                    default:
                        break;
                }

                EditorGUILayout.PropertyField(unequipIfEquipped, new GUIContent("Unequip if equipped", "Do we unequip the item if it is already equipped?"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}
