using UnityEditor;
using UnityEngine;
using OkapiKit;
using OkapiKit.Editor;

namespace OkapiKitEditor
{
    [CustomEditor(typeof(Item))]
    public class ItemEditor : OkapiBaseEditor
    {
        SerializedProperty categoriesProp;
        SerializedProperty isCategoryProp;
        SerializedProperty displayNameProp;
        SerializedProperty displaySpriteColorProp;
        SerializedProperty displaySpriteProp;
        SerializedProperty displayTextColorProp;
        SerializedProperty isStackableProp;
        SerializedProperty maxStackProp;
        SerializedProperty inventorySlotsProp;

        Item item;

        protected override void OnEnable()
        {
            base.OnEnable();

            item = (Item)target;

            categoriesProp = serializedObject.FindProperty("categories");
            isCategoryProp = serializedObject.FindProperty("isCategory");
            displayNameProp = serializedObject.FindProperty("displayName");
            displaySpriteColorProp = serializedObject.FindProperty("displaySpriteColor");
            displaySpriteProp = serializedObject.FindProperty("displaySprite");
            displayTextColorProp = serializedObject.FindProperty("displayTextColor");
            isStackableProp = serializedObject.FindProperty("isStackable");
            maxStackProp = serializedObject.FindProperty("maxStack");
            inventorySlotsProp = serializedObject.FindProperty("inventorySlots");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(isCategoryProp);
                EditorGUILayout.PropertyField(categoriesProp, true);

                if (!isCategoryProp.boolValue)
                {
                    EditorGUILayout.PropertyField(displayNameProp);
                    EditorGUILayout.PropertyField(displaySpriteProp);

                    if (displaySpriteProp.objectReferenceValue != null)
                    {
                        EditorGUILayout.PropertyField(displaySpriteColorProp);
                    }

                    EditorGUILayout.PropertyField(displayTextColorProp);
                    EditorGUILayout.PropertyField(isStackableProp);

                    if (isStackableProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(maxStackProp);
                    }

                    EditorGUILayout.PropertyField(inventorySlotsProp);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        // OkapiBaseEditor requirements:

        protected override string GetTitle()
        {
            if (item.isCategory) return item.name;
            return (item.displayName != "Item Display Name") ? (item.displayName) : item.name;
        }

        protected override Color GetIconBackgroundColor()
        {
            return (item.displaySprite) ? (Color.black) : (base.GetIconBackgroundColor());
        }

        protected override Texture2D GetIcon()
        {
            return (item.displaySprite) ? AssetPreview.GetAssetPreview(item.displaySprite) : GUIUtils.GetTexture("Item");
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
            return (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#969721"));
        }
    }
}
