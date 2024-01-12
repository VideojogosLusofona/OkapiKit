using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeRigidBody))]
    public class ActionChangeRigidBodyEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propChangeType;
        SerializedProperty propBodyType;
        SerializedProperty propValue;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propChangeType = serializedObject.FindProperty("changeType");
            propBodyType = serializedObject.FindProperty("bodyType");
            propValue = serializedObject.FindProperty("value");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeRigidBody);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));

                var changeType = (ActionChangeRigidBody.ChangeType)propChangeType.enumValueIndex;

                if (changeType == ActionChangeRigidBody.ChangeType.SetBodyType)
                {
                    EditorGUILayout.PropertyField(propBodyType, new GUIContent("Body Type"));
                }
                else if ((changeType == ActionChangeRigidBody.ChangeType.Mass) ||
                         (changeType == ActionChangeRigidBody.ChangeType.LinearDrag) ||
                         (changeType == ActionChangeRigidBody.ChangeType.AngularDrag) ||
                         (changeType == ActionChangeRigidBody.ChangeType.GravityScale))
                {
                    EditorGUILayout.PropertyField(propValue, new GUIContent("Value"));
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