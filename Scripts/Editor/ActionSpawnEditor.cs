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
                    EditorGUILayout.PropertyField(propPrefabObject, new GUIContent("Prefab Object"));

                    if (propPrefabObject.objectReferenceValue != null)
                    {
                        EditorGUILayout.PropertyField(propSpawnPosition, new GUIContent("Position"));
                        if (propSpawnPosition.enumValueIndex == (int)ActionSpawn.SpawnPosition.Target)
                        {
                            EditorGUILayout.PropertyField(propTargetPosition, new GUIContent("Target"));
                        }
                        else if (propSpawnPosition.enumValueIndex == (int)ActionSpawn.SpawnPosition.Tag)
                        {
                            EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Tag"));
                        }
                    }
                    if (propSpawnPosition.enumValueIndex != (int)ActionSpawn.SpawnPosition.Default)
                    {
                        EditorGUILayout.PropertyField(propSetParent, new GUIContent("Set Parent"));
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