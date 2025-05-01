using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Order2d))]
    public class Order2dEditor : OkapiBaseEditor
    {
        SerializedProperty propOffsetZ;

        protected override void OnEnable()
        {
            base.OnEnable();

            propOffsetZ = serializedObject.FindProperty("offsetZ");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                if (OkapiConfig.orderMode == OrderMode.Z)
                {
                    EditorGUILayout.PropertyField(propOffsetZ, new GUIContent("Z Offset", "Offset of the ordering in Z - this is added to the z coord of this object when ordering"));
                }

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();
            }
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override string GetTitle()
        {
            return "Order2d";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Order2d");
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#969721"));
    }
}