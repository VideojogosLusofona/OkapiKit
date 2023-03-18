using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionFlash))]
public class ActionFlashEditor : ActionEditor
{
    SerializedProperty propTarget;
    SerializedProperty propColor;
    SerializedProperty propDuration;

    protected override void OnEnable()
    {
        base.OnEnable();

        propTarget = serializedObject.FindProperty("target");
        propColor = serializedObject.FindProperty("color");
        propDuration = serializedObject.FindProperty("duration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionFlash);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
            EditorGUILayout.PropertyField(propColor, new GUIContent("Color"));
            EditorGUILayout.PropertyField(propDuration, new GUIContent("Duration"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
