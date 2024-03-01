using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Probe))]
    public class ProbeEditor : OkapiBaseEditor
    {
        SerializedProperty propType;
        SerializedProperty propDirection;
        SerializedProperty propColliders;
        SerializedProperty propRadius;
        SerializedProperty propMinDistance;
        SerializedProperty propMaxDistance;
        SerializedProperty propTargetObject;
        SerializedProperty propTargetTag;
        SerializedProperty propTags;
        SerializedProperty propTargetTransform;

        protected override void OnEnable()
        {
            base.OnEnable();

            propType = serializedObject.FindProperty("type");
            propColliders = serializedObject.FindProperty("colliders");
            propDirection = serializedObject.FindProperty("direction");
            propRadius = serializedObject.FindProperty("radius");
            propMinDistance = serializedObject.FindProperty("minDistance");
            propMaxDistance = serializedObject.FindProperty("maxDistance");
            propTargetObject = serializedObject.FindProperty("dirTransform");
            propTargetTag = serializedObject.FindProperty("dirTag");
            propTags = serializedObject.FindProperty("tags");
            propTargetTransform = serializedObject.FindProperty("targetTransform");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propType, new GUIContent("Type", "Type of probe.\nRaycast: detects intersections in a straight line (line is super-thin)\nCirclecast: detects intersections in a straight line (line has thickness)"));

                var probeType = (Probe.Type)propType.enumValueIndex;

                if ((probeType == Probe.Type.Raycast) || (probeType == Probe.Type.Circlecast))
                {
                    if (probeType == Probe.Type.Circlecast)
                    {
                        EditorGUILayout.PropertyField(propRadius, new GUIContent("Radius", "Width of line"));

                    }
                    EditorGUILayout.PropertyField(propDirection, new GUIContent("Direction", "Direction of line.\nUp/Down/Left/Right: Line is in that relative direction (based on rotation of object)\nTarget object/tag: Line is in the direction of a certain object until it reaches it.\nTarget object/tag direction: Line is in the direction of a certain object, until a maximum distance."));
                    Probe.Direction direction = (Probe.Direction)propDirection.enumValueIndex;

                    switch (direction)
                    {
                        case Probe.Direction.Up:
                        case Probe.Direction.Down:
                        case Probe.Direction.Right:
                        case Probe.Direction.Left:
                            EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance", "Minimum distance from start"));
                            EditorGUILayout.PropertyField(propMaxDistance, new GUIContent("Max Distance", "Maximum distance from start"));
                            break;
                        case Probe.Direction.TargetObjectDirection:
                            EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "Target object to specify probe direction.\nUsually it's better to use tags than links."));
                            EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance", "Minimum distance from start"));
                            EditorGUILayout.PropertyField(propMaxDistance, new GUIContent("Max Distance", "Maximum distance from start"));
                            break;
                        case Probe.Direction.TargetTagDirection:
                            EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "Target tag to specify probe direction.\nClosest object with this tag will be used."));
                            EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance", "Minimum distance from start"));
                            EditorGUILayout.PropertyField(propMaxDistance, new GUIContent("Max Distance", "Maximum distance from start"));
                            break;
                        case Probe.Direction.TargetObject:
                            EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "Target object to specify probe direction.\nUsually it's better to use tags than links."));
                            EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance", "Minimum distance from start"));
                            break;
                        case Probe.Direction.TargetTag:
                            EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "Target tag to specify probe direction.\nClosest object with this tag will be used."));
                            EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance", "Minimum distance from start"));
                            break;
                        default:
                            break;
                    }
                    EditorGUILayout.PropertyField(propTags, new GUIContent("Tags", "Tags to be considered in terms of intersection. If no tags are present, there will be no intersections detected."));
                    EditorGUILayout.PropertyField(propTargetTransform, new GUIContent("Target Transform", "If set, this transform will be set to the intersection point of the ray/circlecast"));
                }
                else if (probeType == Probe.Type.Colliders)
                {
                    EditorGUILayout.PropertyField(propColliders, new GUIContent("Colliders", "Colliders that will be checked for intersection against the scene."));

                    EditorGUILayout.PropertyField(propTags, new GUIContent("Tags", "Tags to be considered in terms of intersection. If no tags are present, there will be no intersections detected."));
                }

                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

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
            return "Probe";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Probe");
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#969721"));
    }
}