using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionAddItem))]
    public class ActionAddItemEditor : ActionEditor
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

                var action = target as ActionAddItem;
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propItem, new GUIContent("Item", "The item to be added."));
                EditorGUILayout.PropertyField(propQuantity, new GUIContent("Quantity", "Quantity of the item to add."));
                EditorGUILayout.PropertyField(propInventory, new GUIContent("Inventory", "The inventory to add the item to."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}
