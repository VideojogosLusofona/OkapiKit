using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeTransform))]
    public class ActionChangeTransformEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propChangeType;
        SerializedProperty propXAxis;
        SerializedProperty propPositionX;
        SerializedProperty propDeltaX;
        SerializedProperty propYAxis;
        SerializedProperty propPositionY;
        SerializedProperty propDeltaY;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propChangeType = serializedObject.FindProperty("changeType");
            propXAxis = serializedObject.FindProperty("xAxis");
            propPositionX = serializedObject.FindProperty("positionX");
            propDeltaX = serializedObject.FindProperty("deltaX");
            propYAxis = serializedObject.FindProperty("yAxis");
            propPositionY = serializedObject.FindProperty("positionY");
            propDeltaY = serializedObject.FindProperty("deltaY");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeTransform);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));
                if (propChangeType.enumValueIndex == (int)ActionChangeTransform.ChangeType.Position)
                {
                    EditorGUILayout.PropertyField(propXAxis, new GUIContent("X Axis"));
                    if (propXAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionX, new GUIContent("X Position (random value between [X..Y])"));
                    }
                    else if (propXAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Change)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])"));
                    }

                    EditorGUILayout.PropertyField(propYAxis, new GUIContent("Y Axis"));
                    if (propYAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionY, new GUIContent("Y Position (random value between [X..Y])"));
                    }
                    else if (propYAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Change)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])"));
                    }
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