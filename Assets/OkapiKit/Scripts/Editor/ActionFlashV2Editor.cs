using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionFlashV2))]
    public class ActionFlashV2Editor : ActionEffectEditor
    {
        SerializedProperty color;
        SerializedProperty duration;
        SerializedProperty mode;

        protected override void OnEnable()
        {
            base.OnEnable();

            color = serializedObject.FindProperty("color");
            duration = serializedObject.FindProperty("duration");
            mode = serializedObject.FindProperty("mode");
        }

        protected override void ActionEffect_OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(mode, new GUIContent("Mode", "Select if you want to flash a specific color, or just invert colors during the flash."));
            var modeValue = (ActionFlashV2.Mode)mode.enumValueIndex;
            if (modeValue == ActionFlashV2.Mode.ColorFlash)
            {
                EditorGUILayout.PropertyField(color, new GUIContent("Color", "Color gradient to flash the object.\nIf alpha equals zero, there's no flash at that point, if it's equal to 1, only the flash color will be visible."));
            }
            EditorGUILayout.PropertyField(duration, new GUIContent("Duration", "For how long to flash the object, in seconds."));
        }
    }
}