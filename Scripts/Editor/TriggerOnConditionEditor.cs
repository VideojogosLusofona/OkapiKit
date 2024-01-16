using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(TriggerOnCondition))]
    public class TriggerOnConditionEditor : TriggerEditor
    {
        SerializedProperty propContinuous;
        SerializedProperty propConditions;
        SerializedProperty propElseActions;

        protected override void OnEnable()
        {
            base.OnEnable();

            propContinuous = serializedObject.FindProperty("continuous");
            propConditions = serializedObject.FindProperty("conditions");
            propElseActions = serializedObject.FindProperty("elseActions");
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Condition");

            return varTexture;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false, false);

                var trigger = (target as TriggerOnCondition);
                if (trigger == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propContinuous, new GUIContent("Continuous Check", "When active, this trigger will execute everytime the conditions are true, otherwise it will only execute when the conditions change."));
                EditorGUILayout.PropertyField(propConditions, new GUIContent("Conditions", "When these conditions are all true, this trigger will execute."));
                EditorGUI.EndChangeCheck();

                ActionPanel();

                var actionsRect = GUILayoutUtility.GetLastRect();
                actionsRect = new Rect(actionsRect.xMin, actionsRect.yMax, actionsRect.width, 20.0f);

                TryDragActionToActionDelayList(actionsRect, propElseActions);

                EditorGUILayout.PropertyField(propElseActions, new GUIContent("Else Actions", "What actions do we want if the conditions are not true?"), true);

                serializedObject.ApplyModifiedProperties();
                (target as Trigger).UpdateExplanation();
            }
        }
    }
}