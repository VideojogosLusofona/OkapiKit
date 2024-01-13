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

            EditorGUILayout.PropertyField(propFill, new GUIContent("Fill", "UI object to scale depending on current variable value and its limits"), true);
            EditorGUILayout.PropertyField(propSetColor, new GUIContent("Set color?", "Do we want to set the color as well as scale it?\nOnly works if Fill is an UI Image component"), true);
            if (propSetColor.boolValue)
            {
                EditorGUILayout.PropertyField(propColor, new GUIContent("Color Gradient", "Color gradient to use. Leftmost color is used when the value is close to 0%, rightmost color is used when value is close to 100%"), true);
            }

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."), true);

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();
        }

    }
}