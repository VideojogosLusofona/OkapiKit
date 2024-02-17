using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionShake))]
    public class ActionShakeEditor : ActionEditor
    {
        private SerializedProperty propTargetTag;
        private SerializedProperty propTargetObject;
        private SerializedProperty propStrength;
        private SerializedProperty propDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTargetTag = serializedObject.FindProperty("targetTag");
            propTargetObject = serializedObject.FindProperty("targetObject");
            propStrength = serializedObject.FindProperty("strength");
            propDuration = serializedObject.FindProperty("duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionShake);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                if (propTargetTag.objectReferenceValue == null)
                {
                    if (propTargetObject.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "An object with this tag will be the target.\nYou can set either a tag or a target object, but not both at the same time."));
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the target object?\nYou can set either a tag or a target object, but not both at the same time."));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the target object?\nYou can set either a tag or a target object, but not both at the same time."));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "An object with this tag will be the target.\nYou can set either a tag or a target object, but not both at the same time."));
                }
                EditorGUILayout.PropertyField(propStrength, new GUIContent("Strength", "Strength of the shaking, in world units (pixels)."));
                EditorGUILayout.PropertyField(propDuration, new GUIContent("Duration", "Duration of the shaking."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}