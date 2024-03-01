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
                EditorGUILayout.PropertyField(propSearchType, new GUIContent("Search Type", "Where to find objects with actions tagged with the trigger tags?\nGlobal: Search for actions in all the scene\nTagged: Find actions tagged in this way in objects tagged by the search tags\nChildren: Find objects in the list of childrens of this object\nWithin collider: Find tagged action in objects inside a certain collider."));
                if (propSearchType.intValue == (int)ActionTagged.SearchType.Tagged)
                {
                    EditorGUILayout.PropertyField(propSearchTags, new GUIContent("Search Tags", "What objects to search for tagged actions?"));
                }
                if (propSearchType.intValue == (int)ActionTagged.SearchType.WithinCollider)
                {
                    EditorGUILayout.PropertyField(propSearchTags, new GUIContent("Search Tags", "What objects to search for tagged actions?\nThe objects tagged with this also have to be inside one of the given colliders."));
                    EditorGUILayout.PropertyField(propColliders, new GUIContent("Search Colliders", "In which colliders to search for objects?\nNote that for an object to be detected as being inside the collider, it needs to have a collider as well!"));
                }
                EditorGUILayout.PropertyField(propTriggerType, new GUIContent("Trigger Type", "If we find more than one tagged action, what to do?All: Trigger all tagged actions\nSequence: Trigger the first action, then the next time the same set is found, trigger the second action, and so forth\nRandom: Select a random action to trigger."));
                EditorGUILayout.PropertyField(propTriggerTags, new GUIContent("Trigger Tags", "We're finding actions tagged with any of these"));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}