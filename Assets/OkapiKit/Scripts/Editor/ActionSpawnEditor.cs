using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionSpawn))]
    public class ActionSpawnEditor : ActionEditor
    {
        SerializedProperty propPrefabObject;
        SerializedProperty propSpawnPosition;
        SerializedProperty propTargetPosition;
        SerializedProperty propTargetTag;
        SerializedProperty propSetParent;

        protected override void OnEnable()
        {
            base.OnEnable();

            propPrefabObject = serializedObject.FindProperty("prefabObject");
            propSpawnPosition = serializedObject.FindProperty("spawnPosition");
            propTargetPosition = serializedObject.FindProperty("targetPosition");
            propTargetTag = serializedObject.FindProperty("targetTag");
            propSetParent = serializedObject.FindProperty("setParent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionSpawn);
                if (action == null) return;

                Spawner spawner = action.GetComponent<Spawner>();

                EditorGUI.BeginChangeCheck();
                if (spawner == null)
                {
                    EditorGUILayout.PropertyField(propPrefabObject, new GUIContent("Prefab Object", "What is the prefab to spawn?\nThis has to be an object from the project, not from the scene!"));

                    if (propPrefabObject.objectReferenceValue != null)
                    {
                        EditorGUILayout.PropertyField(propSpawnPosition, new GUIContent("Position", "What is the position of the new object?\nDefault: Object will spawn on the position where it was when the prefab was created\nThis: Object will spawn with the same position/rotation/scale as the current object\nTarget: Object will spawn at the position/rotation/scale of the target object\nTag: Object will spawn at the position/rotation/scale of an object tagged with the tag given."));
                        if (propSpawnPosition.intValue == (int)ActionSpawn.SpawnPosition.Target)
                        {
                            EditorGUILayout.PropertyField(propTargetPosition, new GUIContent("Target", "What's the target object where to spawn?"));
                        }
                        else if (propSpawnPosition.intValue == (int)ActionSpawn.SpawnPosition.Tag)
                        {
                            EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Tag", "Find all objects with this tag, and select a random one where to spawn the new object."));
                        }
                    }
                    if (propSpawnPosition.intValue != (int)ActionSpawn.SpawnPosition.Default)
                    {
                        EditorGUILayout.PropertyField(propSetParent, new GUIContent("Set Parent", "Should we set the parent of the new object to this object?"));
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