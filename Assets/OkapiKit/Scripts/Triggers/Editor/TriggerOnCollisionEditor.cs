using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
            EditorGUILayout.PropertyField(propIsTrigger, new GUIContent("Collider Is Trigger?"));
            EditorGUILayout.PropertyField(propEventType, new GUIContent("Event type"));
            EditorGUILayout.PropertyField(propTags, new GUIContent("Tags"));
            EditorGUI.EndChangeCheck();

            ActionPanel();
        }
    }
}
