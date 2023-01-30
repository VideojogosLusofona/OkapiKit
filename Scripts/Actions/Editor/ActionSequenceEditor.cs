using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionSequence))]
public class ActionSequenceEditor : ActionEditor
{
    SerializedProperty propActions;

    protected override void OnEnable()
    {
        base.OnEnable();

        propActions = serializedObject.FindProperty("actions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionSequence);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propActions, new GUIContent("Action Sequence"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
