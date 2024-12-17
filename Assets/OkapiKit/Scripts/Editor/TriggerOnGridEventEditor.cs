using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(TriggerOnGridEvent))]
    public class TriggerOnGridEventEditor : TriggerEditor
    {
        SerializedProperty propEventType;
        SerializedProperty propTags;

        protected override void OnEnable()
        {
            base.OnEnable();

            propEventType = serializedObject.FindProperty("eventType");
            propTags = serializedObject.FindProperty("tags");
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("GridEvent");

            return varTexture;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var trigger = (target as TriggerOnGridEvent);
                if (trigger == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propEventType, new GUIContent("Event type", "What kind of event we want to detect.\nHitWall: When this object hits a wall\nPushObject: When this object pushes another\nHitObject: When this object hits an object that it can't push\nStep: When this object finishes moving one grid position"));

                var eventType = (TriggerOnGridEvent.GridEvent)propEventType.enumValueIndex;
                if ((eventType == TriggerOnGridEvent.GridEvent.PushObject) ||
                    (eventType == TriggerOnGridEvent.GridEvent.HitObject) ||
                    (eventType == TriggerOnGridEvent.GridEvent.WasHit) ||
                    (eventType == TriggerOnGridEvent.GridEvent.WasPushed))
                {
                    EditorGUILayout.PropertyField(propTags, new GUIContent("Tags", "Which tags to use for the interaction.\nOnly objects with these tags can throw an event"));
                }

                EditorGUI.EndChangeCheck();

                ActionPanel();
            }
        }
    }
}