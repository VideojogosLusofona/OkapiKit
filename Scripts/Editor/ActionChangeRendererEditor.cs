using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeRenderer))]
    public class ActionChangeRendererEditor : ActionEditor
    {
        SerializedProperty propRenderer;
        SerializedProperty propChangeType;
        SerializedProperty propVisibility;

        protected override void OnEnable()
        {
            base.OnEnable();

            propRenderer = serializedObject.FindProperty("renderer");
            propChangeType = serializedObject.FindProperty("changeType");
            propVisibility = serializedObject.FindProperty("visibility");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeRenderer);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propRenderer, new GUIContent("Renderer", "What's the target renderer?"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of change to perform?\nVisibility: Visibility of the renderer."));

                if (propChangeType.intValue == (int)ActionChangeRenderer.ChangeType.Visibility)
                {
                    EditorGUILayout.PropertyField(propVisibility, new GUIContent("Visibility", "On: Turn on the renderer\nOff: turn off the renderer\nToggle: If on, turn off the renderer, otherwise turn on."));
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