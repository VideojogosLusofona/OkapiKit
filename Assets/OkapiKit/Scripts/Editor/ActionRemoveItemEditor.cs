using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionRemoveItem))]
    public class ActionRemoveItemEditor : ActionEditor
    {
        SerializedProperty propItem;
        SerializedProperty propInventory;
        SerializedProperty propQuantity;

        protected override void OnEnable()
        {
            base.OnEnable();

            propItem = serializedObject.FindProperty("item");
            propInventory = serializedObject.FindProperty("inventory");
            propQuantity = serializedObject.FindProperty("quantity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = target as ActionRemoveItem;
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propItem, new GUIContent("Item", "The item to be removed."));
                EditorGUILayout.PropertyField(propQuantity, new GUIContent("Quantity", "Quantity of the items to remove."));
                EditorGUILayout.PropertyField(propInventory, new GUIContent("Inventory", "The inventory to remove the item from."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}
