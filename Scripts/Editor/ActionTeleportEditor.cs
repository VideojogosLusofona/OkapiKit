using UnityEditor;
using UnityEngine;
using static OkapiKit.ActionTeleport;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionTeleport))]
    public class ActionTeleportEditor : ActionEditor
    {
        SerializedProperty propSubject;
        SerializedProperty propSubjectTransform;
        SerializedProperty propSubjectTag;
        SerializedProperty propTeleportTarget;
        SerializedProperty propTargetTransforms;
        SerializedProperty propTargetTags;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSubject = serializedObject.FindProperty("subject");
            propSubjectTransform = serializedObject.FindProperty("subjectTransform");
            propSubjectTag = serializedObject.FindProperty("subjectTag");
            propTeleportTarget = serializedObject.FindProperty("teleportTarget");
            propTargetTransforms = serializedObject.FindProperty("targetTransforms");
            propTargetTags = serializedObject.FindProperty("targetTags");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionTeleport);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propSubject, new GUIContent("Subject", "What's the type of object that should be teleported?"));

                var subjectType = (ActionTeleport.TeleportSubject)propSubject.enumValueIndex;
                switch (subjectType)
                {
                    case TeleportSubject.Target:
                        EditorGUILayout.PropertyField(propSubjectTransform, new GUIContent("Subject Transform", "What is the subject's transform?"));
                        break;
                    case TeleportSubject.Tag:
                        EditorGUILayout.PropertyField(propSubjectTransform, new GUIContent("Subject Tag", "What is the subject's tag?"));
                        break;
                    default:
                        break;
                }

                EditorGUILayout.PropertyField(propTeleportTarget, new GUIContent("Target", "Where to teleport the object?"));

                var targetType = (ActionTeleport.TeleportTarget)propTeleportTarget.enumValueIndex;

                switch (targetType)
                {
                    case TeleportTarget.Target:
                        EditorGUILayout.PropertyField(propTargetTransforms, new GUIContent("Targets", "What are the potential targets?"));
                        break;
                    case TeleportTarget.Tag:
                        EditorGUILayout.PropertyField(propTargetTransforms, new GUIContent("Tags", "What are the potential target tags?"));
                        break;
                    default:
                        break;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}