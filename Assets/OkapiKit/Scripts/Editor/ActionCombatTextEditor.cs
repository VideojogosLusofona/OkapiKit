using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionCombatText))]
    public class ActionCombatTextEditor: ActionEditor
    {
        SerializedProperty targetTransform;
        SerializedProperty text;
        SerializedProperty color;
        SerializedProperty duration;

        protected override void OnEnable()
        {
            base.OnEnable();

            targetTransform = serializedObject.FindProperty("targetTransform");
            text = serializedObject.FindProperty("text");
            color = serializedObject.FindProperty("color");
            duration = serializedObject.FindProperty("duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as Action);
                if (action == null) return;
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(targetTransform, new GUIContent("Target", "Where to spawn the combat text. You can use a CombatText component to define more precisely where in the object to spawn."));
                EditorGUILayout.PropertyField(text, new GUIContent("Text", "Text to display."));
                EditorGUILayout.PropertyField(color, new GUIContent("Color", "Color of the text"));
                EditorGUILayout.PropertyField(duration, new GUIContent("Duration", "For how long should the movement happen?"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}