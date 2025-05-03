using System;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    public abstract class OkapiTargetEditor<T> : PropertyDrawer where T : Component
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label and get remaining rect for fields
            Rect contentRect = EditorGUI.PrefixLabel(position, label);

            float enumWidth = contentRect.width * 0.35f;
            float valueWidth = contentRect.width * 0.65f;

            Rect typeRect = new Rect(contentRect.x, contentRect.y, enumWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(contentRect.x + enumWidth + 2, contentRect.y, valueWidth - 2, EditorGUIUtility.singleLineHeight);
            Rect flagsRect = valueRect;
            flagsRect.width *= 0.3f;
            valueRect.width -= flagsRect.width;
            flagsRect.x = valueRect.xMax;

            // Get properties
            var typeProp = property.FindPropertyRelative("type");
            var tagProp = property.FindPropertyRelative("tag");
            var objProp = property.FindPropertyRelative("obj");
            var flagsProp = property.FindPropertyRelative("flags");

            // Draw enum and appropriate field
            EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);

            var enumVal = (OkapiTarget<T>.Type)typeProp.enumValueIndex;
            switch (enumVal)
            {
                case OkapiTarget<T>.Type.Hypertag:
                case OkapiTarget<T>.Type.ColliderChildTag:
                    EditorGUI.PropertyField(valueRect, tagProp, GUIContent.none);
                    break;
                case OkapiTarget<T>.Type.Object:
                    EditorGUI.PropertyField(valueRect, objProp, GUIContent.none);
                    break;
                case OkapiTarget<T>.Type.Self:
                case OkapiTarget<T>.Type.Collider:
                    flagsRect.width = valueWidth * 0.5f;
                    flagsRect.x = contentRect.x + enumWidth + 2;
                    break;
            }

            EditorGUI.PropertyField(flagsRect, flagsProp, GUIContent.none);

            CustomOnGUI(position, property, label, contentRect);

            EditorGUI.EndProperty();
        }

        protected virtual void CustomOnGUI(Rect position, SerializedProperty property, GUIContent label, Rect contentRect)
        {

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(TargetInventory))]
    public class TargetInventoryEditor : OkapiTargetEditor<Inventory>
    {
    }

    [CustomPropertyDrawer(typeof(TargetEquipment))]
    public class EquipmentInventoryEditor : OkapiTargetEditor<Equipment>
    {
    }

    [CustomPropertyDrawer(typeof(TargetRenderer))]
    public class TargetRendererEditor : OkapiTargetEditor<Renderer>
    {
    }

    [CustomPropertyDrawer(typeof(TargetResource))]
    public class TargetResourceEditor : OkapiTargetEditor<Resource>
    {
        protected override void CustomOnGUI(Rect position, SerializedProperty property, GUIContent label, Rect contentRect)
        {
            var typeProp = property.FindPropertyRelative("type");

            if (typeProp.enumValueIndex == (int)TargetResource.Type.Object) return;

            var resourceType = property.FindPropertyRelative("resourceType");

            var rect = contentRect;
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height = EditorGUIUtility.singleLineHeight;

            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = rect.width * 0.15f;

            EditorGUI.PropertyField(rect, resourceType, new GUIContent("Type"));

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var typeProp = property.FindPropertyRelative("type");
            if (typeProp.enumValueIndex == (int)TargetResource.Type.Object)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 2;
        }
    }

    [CustomPropertyDrawer(typeof(TargetQuestManager))]
    public class TargetQuestManagerEditor : OkapiTargetEditor<QuestManager>
    {
    }

    [CustomPropertyDrawer(typeof(TargetTransform))]
    public class TargetTransformEditor : OkapiTargetEditor<Transform>
    {
    }
}
