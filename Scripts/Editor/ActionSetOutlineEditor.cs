using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionSetOutline))]
    public class ActionSetOutlineEditor : ActionEffectEditor
    {
        SerializedProperty color;
        SerializedProperty thickness;

        protected override void OnEnable()
        {
            base.OnEnable();

            color = serializedObject.FindProperty("color");
            thickness = serializedObject.FindProperty("thickness");
        }

        protected override void ActionEffect_OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(color, new GUIContent("Color", "Color of the outline."));
            EditorGUILayout.PropertyField(thickness, new GUIContent("Thickness", "What's the thickness of the outline. Note that this might not be possible, use sprites with 'Rect' mode (instead of 'Tight'), and with some space around the main image for the outline to work properly."));
        }
    }
}