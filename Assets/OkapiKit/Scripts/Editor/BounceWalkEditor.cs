using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(BounceWalk))]
    public class BounceWalkEditor : OkapiBaseEditor
    {
        SerializedProperty targetT;
        SerializedProperty stepTime;
        SerializedProperty stepHeight;
        SerializedProperty teleportDistance;

        protected override void OnEnable()
        {
            base.OnEnable();

            targetT = serializedObject.FindProperty("target");
            stepTime = serializedObject.FindProperty("stepTime");
            stepHeight = serializedObject.FindProperty("stepHeight");
            teleportDistance = serializedObject.FindProperty("teleportDistance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(targetT, new GUIContent("Target Transform", "Transform to \"bounce\". This will be a change in local coordinates, usually it's a child object."));
                EditorGUILayout.PropertyField(stepTime, new GUIContent("Step Time", "How long does each step take, in seconds."));
                EditorGUILayout.PropertyField(stepHeight, new GUIContent("Step Height", "How much to bounce on each step."));
                EditorGUILayout.PropertyField(teleportDistance, new GUIContent("Teleport Distance", "If between frames, this object moves more than this, it doesn't count as a movement and the object will go back to the rest position."));

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
            return "Bounce Walk";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("BounceWalk");
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#969721"));
    }
}