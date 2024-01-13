using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionUnityEvent))]
    public class ActionUnityEventEditor : ActionEditor
    {
        SerializedProperty propUnityEvent;

        protected override void OnEnable()
        {
            base.OnEnable();

            propUnityEvent = serializedObject.FindProperty("unityEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionUnityEvent);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propUnityEvent, new GUIContent("Unity Event", "Unity event to trigger"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}