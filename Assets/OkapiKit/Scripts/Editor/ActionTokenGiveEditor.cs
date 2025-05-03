using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionTokenGive))]
    public class ActionTokenGiveEditor : ActionEditor
    {
        SerializedProperty questManager;
        SerializedProperty token;
        SerializedProperty quantity;

        protected override void OnEnable()
        {
            base.OnEnable();

            questManager = serializedObject.FindProperty("questManager");
            token = serializedObject.FindProperty("token");
            quantity = serializedObject.FindProperty("quantity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as Action);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(questManager, new GUIContent("Quest Manager", "Target quest manager - who receives these tokens?"));
                EditorGUILayout.PropertyField(token, new GUIContent("Token", "Token to give to player"));
                EditorGUILayout.PropertyField(quantity, new GUIContent("Quantity", "Quantity of tokens to give to player"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}