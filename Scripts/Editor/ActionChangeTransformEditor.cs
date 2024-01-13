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
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the target transform?"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of change you want to do on the transform?\nPosition: Change the position."));
                if (propChangeType.enumValueIndex == (int)ActionChangeTransform.ChangeType.Position)
                {
                    EditorGUILayout.PropertyField(propXAxis, new GUIContent("X Axis", "What do you want to do with the X axis?\nNone: Don't change the X position\nChange: Add/Subtract an amount to the current X coordinate.\nSet: Set the X coordinate to some value."));
                    if (propXAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionX, new GUIContent("X Position (random value between [X..Y])", "What's the new value for the X coordinate?\nA value in the given range will be selected."));
                    }
                    else if (propXAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Change)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])", "How much to add/subtract from the X coordinate? A random value in the given range will be selected."));
                    }

                    EditorGUILayout.PropertyField(propYAxis, new GUIContent("Y Axis", "What do you want to do with the Y axis?\nNone: Don't change the Y position\nChange: Add/Subtract an amount to the current Y coordinate.\nSet: Set the Y coordinate to some value."));
                    if (propYAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionY, new GUIContent("Y Position (random value between [X..Y])", "What's the new value for the Y coordinate?\nA value in the given range will be selected."));
                    }
                    else if (propYAxis.enumValueIndex == (int)ActionChangeTransform.AxisChange.Change)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])", "How much to add/subtract from the Y coordinate? A random value in the given range will be selected."));
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