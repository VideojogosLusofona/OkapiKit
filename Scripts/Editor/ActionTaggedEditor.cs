using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionTagged))]
    public class ActionTaggedEditor : ActionEditor
    {
        SerializedProperty propSearchType;
        SerializedProperty propSearchTags;
        SerializedProperty propTriggerType;
        SerializedProperty propTriggerTags;
        SerializedProperty propColliders;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSearchType = serializedObject.FindProperty("searchType");
            propSearchTags = serializedObject.FindProperty("searchTags");
            propTriggerType = serializedObject.FindProperty("triggerType");
            propTriggerTags = serializedObject.FindProperty("triggerTags");
            propColliders = serializedObject.FindProperty("colliders");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionTagged);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propSearchType, new GUIContent("Search Type"));
                if (propSearchType.enumValueIndex == (int)ActionTagged.SearchType.Tagged)
                {
                    EditorGUILayout.PropertyField(propSearchTags, new GUIContent("Search Tags"));
                }
                if (propSearchType.enumValueIndex == (int)ActionTagged.SearchType.WithinCollider)
                {
                    EditorGUILayout.PropertyField(propSearchTags, new GUIContent("Search Tags"));
                    EditorGUILayout.PropertyField(propColliders, new GUIContent("Search Colliders"));
                }
                EditorGUILayout.PropertyField(propTriggerType, new GUIContent("Trigger Type"));
                EditorGUILayout.PropertyField(propTriggerTags, new GUIContent("Trigger Tags"));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}