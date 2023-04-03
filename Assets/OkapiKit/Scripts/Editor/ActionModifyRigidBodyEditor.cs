using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionModifyRigidBody))]
    public class ActionModifyRigidBodyEditor : ActionEditor
    {
        SerializedProperty propChangeType;
        SerializedProperty propBodyType;
        SerializedProperty propValue;

        protected override void OnEnable()
        {
            base.OnEnable();

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

                var action = (target as ActionModifyRigidBody);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));

                var changeType = (ActionModifyRigidBody.ChangeType)propChangeType.enumValueIndex;

                if (changeType == ActionModifyRigidBody.ChangeType.SetBodyType)
                {
                    EditorGUILayout.PropertyField(propBodyType, new GUIContent("Body Type"));
                }
                else if ((changeType == ActionModifyRigidBody.ChangeType.Mass) ||
                         (changeType == ActionModifyRigidBody.ChangeType.LinearDrag) ||
                         (changeType == ActionModifyRigidBody.ChangeType.AngularDrag) ||
                         (changeType == ActionModifyRigidBody.ChangeType.GravityScale))
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