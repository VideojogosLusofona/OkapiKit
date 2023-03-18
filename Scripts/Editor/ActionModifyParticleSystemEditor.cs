using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionModifyParticleSystem))]
public class ActionModifyParticleSystemEditor : ActionEditor
{
    SerializedProperty propParticleSystem;
    SerializedProperty propChangeType;
    SerializedProperty propEmission;

    protected override void OnEnable()
    {
        base.OnEnable();

        propParticleSystem = serializedObject.FindProperty("particleSystem");
        propChangeType = serializedObject.FindProperty("changeType");
        propEmission = serializedObject.FindProperty("emission");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionModifyParticleSystem);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propParticleSystem, new GUIContent("Particle System"));
            EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));

            if (propChangeType.enumValueIndex == (int)ActionModifyParticleSystem.ChangeType.Emission)
            {
                EditorGUILayout.PropertyField(propEmission, new GUIContent("Emission"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
