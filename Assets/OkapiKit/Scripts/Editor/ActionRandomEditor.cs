using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionRandom))]
public class ActionRandomEditor : ActionEditor
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

            var action = (target as ActionRandom);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propActions, new GUIContent("Choices"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
