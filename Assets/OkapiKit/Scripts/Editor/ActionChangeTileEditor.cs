using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static OkapiKit.ActionChangeTile;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeTile))]
    public class ActionChangeTileEditor : ActionEditor
    {
        SerializedProperty propVicinityType;
        SerializedProperty propRadius;
        SerializedProperty propTarget;
        SerializedProperty propTags;
        SerializedProperty propRules;

        protected override void OnEnable()
        {
            base.OnEnable();

            propVicinityType= serializedObject.FindProperty("vicinityType");
            propRadius= serializedObject.FindProperty("radius");
            propTarget= serializedObject.FindProperty("target");
            propTags= serializedObject.FindProperty("tags");
            propRules = serializedObject.FindProperty("rules");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeTile);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propVicinityType, new GUIContent("Vicinity", "What's the vicinity of the affected tiles:\nSingle: Just a single tile is affected;\nFour Way: Tiles to the up/down/left/right up to the given radius are affected;\nEight Way: Tiles to the up/down/left/right, plus the diagonals, up to the given radius are affected;\nCircle: All tiles in a circle with a certain radius are affected."));
                var vicinityType = (ActionChangeTile.VicinityType)propVicinityType.enumValueIndex;
                if (vicinityType != VicinityType.Single)
                {
                    EditorGUILayout.PropertyField(propRadius, new GUIContent("Radius", "Radius of influence of the change"));
                }
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What is the central point of the tile change"));
                if (propTarget.objectReferenceValue == null)
                {
                    EditorGUILayout.PropertyField(propTags, new GUIContent("Tags", "What tags define the central points of the tile changes"));
                }
                EditorGUILayout.PropertyField(propRules, new GUIContent("Rules", "What are the change rules."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}