using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeParticleSystem))]
    public class ActionChangeParticleSystemEditor : ActionEditor
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

                var action = (target as ActionChangeParticleSystem);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propParticleSystem, new GUIContent("Particle System", "What's the target particle system?"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of change to perform on the particle system?\nEmission: New particles emission"));
                if (propChangeType.intValue == (int)ActionChangeParticleSystem.ChangeType.Emission)
                {
                    EditorGUILayout.PropertyField(propEmission, new GUIContent("Emission", "On: Activate particle emission\nOff: Disable particle emission\nToggle: If on, turn off particle emission, otherwise turn it on."));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}