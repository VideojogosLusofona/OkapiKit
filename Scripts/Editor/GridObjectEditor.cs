using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(GridObject))]
    public class GridObjectEditor : OkapiBaseEditor
    {
        SerializedProperty propPivot;
        SerializedProperty propCanPush;
        SerializedProperty propMass;
        SerializedProperty propSize;
        SerializedProperty propStepAnimationCurve;

        protected override void OnEnable()
        {
            base.OnEnable();

            propPivot = serializedObject.FindProperty("_pivot");
            propCanPush = serializedObject.FindProperty("_canPush");
            propMass = serializedObject.FindProperty("_mass");
            propSize = serializedObject.FindProperty("_size");
            propStepAnimationCurve = serializedObject.FindProperty("stepAnimationCurve");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propPivot, new GUIContent("Pivot", "Pivot of the visual object associated with this grid object.\nShould match the sprite placed on the sprite renderer, for example."));
                EditorGUILayout.PropertyField(propCanPush, new GUIContent("Can be pushed?", "Can this object be pushed by others?"));
                if (propCanPush.boolValue)
                {
                    EditorGUILayout.PropertyField(propMass, new GUIContent("Mass", "What's the mass of this object? This is compared to the push strength of the object that's trying to push this one."));
                    EditorGUILayout.PropertyField(propSize, new GUIContent("Size", "What's the size of this object? This is used to test for overlaps during pushing."));
                }
                // Separator
                Rect separatorRect = GUILayoutUtility.GetLastRect();
                separatorRect.yMin = separatorRect.yMax + 5;
                separatorRect.height = 5.0f;
                EditorGUI.DrawRect(separatorRect, GUIUtils.ColorFromHex("#ff6060"));
                EditorGUILayout.Space(separatorRect.height + 5);

                EditorGUILayout.PropertyField(propStepAnimationCurve, new GUIContent("Animation Curve", "How do we animate each step position update. Simplest mode is to have a line going from bottom left to upper right."));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();
            }
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override string GetTitle()
        {
            return "Grid Object";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("GridObject");
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#969721"));
    }
}