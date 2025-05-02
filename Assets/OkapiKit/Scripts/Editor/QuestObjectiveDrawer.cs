using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomPropertyDrawer(typeof(Quest.QuestObjective))]
    public class QuestObjectiveDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float spacing = 4f;
            float x = position.x;
            float y = position.y;
            float height = position.height;
            float totalWidth = position.width;

            float typeWidth = 70f;
            float countWidth = 50f;
            float nameWidth = 100f;
            float remaining = totalWidth - typeWidth - countWidth - spacing * 3;

            SerializedProperty typeProp = property.FindPropertyRelative("type");
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            SerializedProperty itemProp = property.FindPropertyRelative("item");
            SerializedProperty tagProp = property.FindPropertyRelative("tag");
            SerializedProperty countProp = property.FindPropertyRelative("count");

            Rect typeRect = new Rect(x, y, typeWidth, height);
            Rect nameRect = new Rect(typeRect.xMax + spacing, y, nameWidth, height);
            Rect valueRect = new Rect(nameRect.xMax + spacing, y, remaining - nameWidth, height);
            Rect countRect = new Rect(valueRect.xMax + spacing, y, countWidth, height);

            EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);

            Quest.QuestObjective.Type type = (Quest.QuestObjective.Type)typeProp.enumValueIndex;

            if (type == Quest.QuestObjective.Type.Token)
            {
                EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
                EditorGUI.PropertyField(valueRect, tagProp, GUIContent.none);
            }
            else // Item
            {
                // Expand item field to use name + tag field space
                Rect itemRect = new Rect(nameRect.x, y, valueRect.xMax - nameRect.x, height);
                EditorGUI.PropertyField(itemRect, itemProp, GUIContent.none);
            }

            EditorGUI.PropertyField(countRect, countProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
