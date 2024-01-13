using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(TriggerOnCollision))]
    public class TriggerOnCollisionEditor : TriggerEditor
    {
        SerializedProperty propIsTrigger;
        SerializedProperty propEventType;
        SerializedProperty propTags;

        protected override void OnEnable()
        {
            base.OnEnable();

            propIsTrigger = serializedObject.FindProperty("isTrigger");
            propEventType = serializedObject.FindProperty("eventType");
            propTags = serializedObject.FindProperty("tags");
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Collision");

            return varTexture;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var trigger = (target as TriggerOnCollision);
                if (trigger == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propIsTrigger, new GUIContent("Collider Is Trigger?", "Is active, only collisions with trigger colliders will be detected."));
                EditorGUILayout.PropertyField(propEventType, new GUIContent("Event type", "What kind of event we want to detect.\nEnter: When a collision happens\nStay: While a collision happens\nExit: When a collision stops happening"));
                EditorGUILayout.PropertyField(propTags, new GUIContent("Tags", "Which tags to use for the collisions.\nOnly objects with these tags (and a collider) can be detected"));
                EditorGUI.EndChangeCheck();

                ActionPanel();
            }
        }
    }
}