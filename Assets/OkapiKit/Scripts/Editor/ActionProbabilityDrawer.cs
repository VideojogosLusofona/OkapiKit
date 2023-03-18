using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ActionRandom.ActionProbability))]
public class ActionRandom_ActionProbabilityDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var actionRect = new Rect(position.x, position.y, position.width - 60, 18);
        var delayRect = new Rect(position.x + position.width - 60, position.y, 50, 18);

        var propAction = property.FindPropertyRelative("action");
        EditorGUI.PropertyField(actionRect, propAction, GUIContent.none);

        if (propAction.objectReferenceValue != null)
        {
            Action action = propAction.objectReferenceValue as Action;
            if (action != null)
            {
                GUIStyle style = GUIUtils.GetTriggerActionExplanationStyle();
                EditorGUI.LabelField(new Rect(position.x + 10, position.y + 20, position.width - 10, 18), action.explanation, style);
            }
        }


        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(delayRect, property.FindPropertyRelative("probability"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = base.GetPropertyHeight(property, label);

        var propAction = property.FindPropertyRelative("action");

        if (propAction.objectReferenceValue != null)
        {
            Action a = propAction.objectReferenceValue as Action;
            if (a)
            {
                int explanationLines = a.explanation.Count((c) => c == '\n') + 1;

                height += 18 * explanationLines;
            }
            else
            {
                height += 18;
            }
        }

        return height;
    }
}
