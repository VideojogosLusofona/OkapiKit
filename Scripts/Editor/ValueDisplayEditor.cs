using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ValueDisplay))]
    public class ValueDisplayEditor : OkapiBaseEditor
    {
        protected SerializedProperty propValueHandler;
        protected SerializedProperty propVariable;

        protected virtual string typeOfDisplay { get; }

        protected override void OnEnable()
        {
            base.OnEnable();

            propValueHandler = serializedObject.FindProperty("valueHandler");
            propVariable = serializedObject.FindProperty("variable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);
            }
        }

        protected virtual void StdEditor(bool useOriginalEditor = true, bool isFinal = true)
        {
            EditorGUI.BeginChangeCheck();

            if (propValueHandler.objectReferenceValue == null)
            {
                if (propVariable.objectReferenceValue == null)
                {
                    EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Variable Instance"), true);
                    EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable"), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable"), true);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Variable Instance"), true);
            }

            if (isFinal)
            {
                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"), true);

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                // Draw old editor, need it for now
                if (useOriginalEditor)
                {
                    base.OnInspectorGUI();
                }
            }
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override string GetTitle()
        {
            string varName = "[UNDEFINED]";
            if (propValueHandler.objectReferenceValue == null)
            {
                if (propVariable.objectReferenceValue != null)
                {
                    varName = propVariable.objectReferenceValue.name;
                }
            }
            else
            {
                varName = propValueHandler.objectReferenceValue.name;
            }

            return $"Display [{varName}] as {typeOfDisplay}";
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("VarDisplay");

            return varTexture;
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#fffaa7"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ffdf6e"));

    }
}