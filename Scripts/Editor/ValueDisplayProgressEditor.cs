using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ValueDisplayProgress))]
    public class ValueDisplayProgressEditor : ValueDisplayEditor
    {
        protected SerializedProperty propFill;
        protected SerializedProperty propSetColor;
        protected SerializedProperty propColor;

        protected override string typeOfDisplay { get => "a progress bar"; }

        protected override void OnEnable()
        {
            base.OnEnable();

            propFill = serializedObject.FindProperty("fill");
            propSetColor = serializedObject.FindProperty("setColor");
            propColor = serializedObject.FindProperty("color");
        }

        protected override void StdEditor(bool useOriginalEditor = true, bool isFinal = true)
        {
            base.StdEditor(useOriginalEditor, false);

            EditorGUILayout.PropertyField(propFill, new GUIContent("Fill"), true);
            EditorGUILayout.PropertyField(propSetColor, new GUIContent("Set color?"), true);
            if (propSetColor.boolValue)
            {
                EditorGUILayout.PropertyField(propColor, new GUIContent("Color Gradient"), true);
            }

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"), true);

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();
        }

    }
}