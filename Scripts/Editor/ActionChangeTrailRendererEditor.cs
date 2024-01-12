using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeTrailRenderer))]
    public class ActionChangeTrailRendererEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propChangeType;
        SerializedProperty propEmitter;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propChangeType = serializedObject.FindProperty("changeType");
            propEmitter = serializedObject.FindProperty("emitter");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeTrailRenderer);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));

                if (propChangeType.enumValueIndex == (int)ActionChangeTrailRenderer.ChangeType.Emitter)
                {
                    EditorGUILayout.PropertyField(propEmitter, new GUIContent("Emitter"));
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