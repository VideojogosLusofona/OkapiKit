using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeSpriteRenderer))]
    public class ActionChangeSpriteRendererEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propChangeType;
        SerializedProperty propSprite;
        SerializedProperty propColor;
        SerializedProperty propBoolState;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propChangeType = serializedObject.FindProperty("changeType");
            propSprite = serializedObject.FindProperty("sprite");
            propColor = serializedObject.FindProperty("color");
            propBoolState = serializedObject.FindProperty("boolState");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeSpriteRenderer);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the sprite renderer to modify?"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of change to apply?\nSprite: Changes the image of the sprite renderer\nColor: Change the color of the sprite renderer.\nNote that if there's an animator linked to this object, the change can be overwritten by it."));

                if (propChangeType.intValue == (int)ActionChangeSpriteRenderer.ChangeType.Sprite)
                {
                    EditorGUILayout.PropertyField(propSprite, new GUIContent("Sprite", "What's the new sprite to set?"));
                }
                if (propChangeType.intValue == (int)ActionChangeSpriteRenderer.ChangeType.Color)
                {
                    EditorGUILayout.PropertyField(propColor, new GUIContent("Color", "What's the new color we want to set?"));
                }
                if ((propChangeType.intValue == (int)ActionChangeSpriteRenderer.ChangeType.FlipX) ||
                    (propChangeType.intValue == (int)ActionChangeSpriteRenderer.ChangeType.FlipY))
                {
                    EditorGUILayout.PropertyField(propBoolState, new GUIContent("Flip?", "Should we flip this axis?"));
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