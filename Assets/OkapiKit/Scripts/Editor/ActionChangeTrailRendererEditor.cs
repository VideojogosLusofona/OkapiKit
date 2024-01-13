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
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the target trail renderer?"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of change on the trail renderer?\nEmission: Effectively turns on/off the trail renderer, without making the existing trail disappear.\nIt's a good idea to disable the trail renderer before teleporting it, for example."));

                if (propChangeType.enumValueIndex == (int)ActionChangeTrailRenderer.ChangeType.Emitter)
                {
                    EditorGUILayout.PropertyField(propEmitter, new GUIContent("Emitter", "On: Emit trail\nOff: Stop trail from being generated, preserving what's already there.\nToggle: Turn it on if it is off, and vice-versa."));
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