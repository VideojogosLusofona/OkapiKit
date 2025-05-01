using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    public abstract class OkapiTargetEditor<T> : PropertyDrawer where T : MonoBehaviour
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

            // Get properties
            var typeProp = property.FindPropertyRelative("type");
            var tagProp = property.FindPropertyRelative("tag");
            var objProp = property.FindPropertyRelative("obj");

            // Draw enum and appropriate field
            EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);

            var enumVal = (OkapiTarget<T>.Type)typeProp.enumValueIndex;
            switch (enumVal)
            {
                case OkapiTarget<T>.Type.Hypertag:
                    EditorGUI.PropertyField(valueRect, tagProp, GUIContent.none);
                    break;
                case OkapiTarget<T>.Type.Object:
                    EditorGUI.PropertyField(valueRect, objProp, GUIContent.none);
                    break;
                case OkapiTarget<T>.Type.Self:
                case OkapiTarget<T>.Type.LastCollider:
                    break;
            }

            EditorGUI.EndProperty();
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

}
