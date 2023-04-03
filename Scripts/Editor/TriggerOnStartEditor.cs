using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(TriggerOnStart))]
    public class TriggerOnStartEditor : TriggerEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Reset");

            return varTexture;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                ActionPanel();
            }
        }
    }
}