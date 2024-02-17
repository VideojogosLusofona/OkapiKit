using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeScene))]
    public class ActionChangeSceneEditor : ActionEditor
    {
        SerializedProperty propSceneName;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSceneName = serializedObject.FindProperty("sceneName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeScene);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                string prevSceneName = propSceneName.stringValue;
                EditorGUILayout.PropertyField(propSceneName, new GUIContent("Scene", "Selects the next scene to be loaded."));
                if ((propSceneName.stringValue != prevSceneName) || (EditorGUI.EndChangeCheck()))
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}