using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionModifySpriteRenderer))]
    public class ActionModifySpriteRendererEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propChangeType;
        SerializedProperty propSprite;
        SerializedProperty propColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propChangeType = serializedObject.FindProperty("changeType");
            propSprite = serializedObject.FindProperty("sprite");
            propColor = serializedObject.FindProperty("color");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionModifySpriteRenderer);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));

                if (propChangeType.enumValueIndex == (int)ActionModifySpriteRenderer.ChangeType.Sprite)
                {
                    EditorGUILayout.PropertyField(propSprite, new GUIContent("Sprite"));
                }
                if (propChangeType.enumValueIndex == (int)ActionModifySpriteRenderer.ChangeType.Color)
                {
                    EditorGUILayout.PropertyField(propColor, new GUIContent("Color"));
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