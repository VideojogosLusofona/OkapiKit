using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionModifySystem))]
public class ActionModifySystemEditor : ActionEditor
{
    SerializedProperty propChangeType;
    SerializedProperty propState;

    protected override void OnEnable()
    {
        base.OnEnable();

        propChangeType = serializedObject.FindProperty("changeType");
        propState = serializedObject.FindProperty("state");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionModifySystem);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));

            if (propChangeType.enumValueIndex == (int)ActionModifySystem.ChangeType.MouseCursorVisibility)
            {
                EditorGUILayout.PropertyField(propState, new GUIContent("Mouse Cursor"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
