using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionSetParent))]
    public class ActionSetParentEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propTargetObject;
        SerializedProperty propTag;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propTargetObject = serializedObject.FindProperty("targetObject");
            propTag = serializedObject.FindProperty("tag");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionSetParent);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
                if (propTag.enumValueIndex == (int)ActionSetParent.Target.Object)
                {
                    EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Object"));
                }
                else if (propTag.enumValueIndex == (int)ActionSetParent.Target.Tag)
                {
                    EditorGUILayout.PropertyField(propTag, new GUIContent("Tag"));
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