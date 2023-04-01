using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace OkapiKit
{
    public class ActionQuitApplication : Action
    {
        public override string GetActionTitle() => "Quit Application";
        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            return $"{GetPreconditionsString(gameObject)}quit application";
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
#endif
            Application.Quit();
        }
    }
}