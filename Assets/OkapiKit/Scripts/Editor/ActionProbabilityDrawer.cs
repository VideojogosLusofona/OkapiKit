using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomPropertyDrawer(typeof(ActionRandom.ActionProbability))]
    public class ActionRandom_ActionProbabilityDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propAction = property.FindPropertyRelative("action");

            if (Event.current.type == EventType.ContextClick)
            {
                if (position.Contains(Event.current.mousePosition))
                {
                    // Create the context menu
                    GenericMenu menu = new GenericMenu();

                    // Add custom menu items
                    menu.AddItem(new GUIContent("Ping"), false, () => { propAction.isExpanded = true; PingAction(propAction.objectReferenceValue as Action); });

                    // Show the menu
                    menu.ShowAsContext();

                    // Use the event
                    Event.current.Use();
                }
            }

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            GUIContent newLabel = new GUIContent(label.text.Replace("Element", "Action"), "What's the action and probability of that action running?");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), newLabel);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var actionRect = new Rect(position.x, position.y, position.width - 60, 18);
            var delayRect = new Rect(position.x + position.width - 60, position.y, 50, 18);

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
        private void PingAction(Action action)
        {
            if (action == null) return;

            EditorGUIUtility.PingObject(action);

            // Select object
            Selection.activeObject = action.gameObject;
            OkapiConfig.PingComponent(action);
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
}