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
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "Type of target to set this object as a parent."));
                if (propTarget.enumValueIndex == (int)ActionSetParent.Target.Object)
                {
                    EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Object", "What object is the new parent of this object?\nLeave it empty to set this object to the root level (with no parent)."));
                }
                else if (propTarget.enumValueIndex == (int)ActionSetParent.Target.Tag)
                {
                    EditorGUILayout.PropertyField(propTag, new GUIContent("Tag", "Find an object with this tag to be the new parent of this object.\nLeave it empty to set this object to the root level (with no parent)."));
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