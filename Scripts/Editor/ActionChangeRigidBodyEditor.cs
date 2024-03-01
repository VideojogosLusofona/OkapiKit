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
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the target rigid body?"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of change on the rigid body?\nBody Type: Switch the type of body to static/dynamic or kinematic\nMass: Change the mass\nLinear Drag: Change the linear drag\nGravity Scale: Change the gravity scale."));

                var changeType = (ActionChangeRigidBody.ChangeType)propChangeType.intValue;

                if (changeType == ActionChangeRigidBody.ChangeType.SetBodyType)
                {
                    EditorGUILayout.PropertyField(propBodyType, new GUIContent("Body Type", "What's the body type?\nStatic: Object will not be moved, collisions can happen with it, it won't be affected by forces like gravity.\nDynamic: Object can move and collide with other objects, according to the laws of physics.\nKinematic: Object can detect collision and can hit other objects, but it doesn't react to forces in general, only obeying its own velocity."));
                }
                else if ((changeType == ActionChangeRigidBody.ChangeType.Mass) ||
                         (changeType == ActionChangeRigidBody.ChangeType.LinearDrag) ||
                         (changeType == ActionChangeRigidBody.ChangeType.AngularDrag) ||
                         (changeType == ActionChangeRigidBody.ChangeType.GravityScale))
                {
                    EditorGUILayout.PropertyField(propValue, new GUIContent("Value", $"Value to set {changeType}."));
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