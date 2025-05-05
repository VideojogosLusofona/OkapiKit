using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace OkapiKit.Editor
{

    [CustomEditor(typeof(OkapiKitContext))]
    public class OkapiKitContextEditor : DefaultExpressionContextEvaluatorDrawer
    {
        protected override void DisplayUtilities()
        {
            base.DisplayUtilities();

            DefaultExpressionContextEvaluator evaluator = (DefaultExpressionContextEvaluator)target;

            if (GUILayout.Button("Add All Quests"))
            {
                MethodInfo addAllQuestsMethod = evaluator.GetType().GetPrivateMethod("AddAllQuests");
                if (addAllQuestsMethod != null)
                {
                    addAllQuestsMethod.Invoke(evaluator, null);
                }
                else
                {
                    Debug.LogError("Method AddAllQuests() not found on this object.");
                }
            }
        }
    }
}