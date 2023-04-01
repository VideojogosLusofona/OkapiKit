using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [RequireComponent(typeof(Animator))]
    public class ActionSetAnimationParameter : Action
    {
        public enum ValueType { Int = 0, Float = 1, Boolean = 2, Trigger = 3, Value = 4 };

        [SerializeField]
        private Animator animator;

        [SerializeField, AnimatorParam("animator")]
        private string animationParameter;
        [SerializeField]
        private ValueType valueType = ValueType.Int;
        [SerializeField, ShowIf("needsInt")]
        private int integerValue;
        [SerializeField, ShowIf("needsFloat")]
        private float floatValue;
        [SerializeField, ShowIf("needsBoolean")]
        private bool boolValue;
        [SerializeField, ShowIf("needsValue")]
        private VariableInstance valueHandler;
        [SerializeField, ShowIf("needsVariable")]
        private Variable variable;

        private bool needsInt => valueType == ValueType.Int;
        private bool needsFloat => valueType == ValueType.Float;
        private bool needsBoolean => valueType == ValueType.Boolean;
        private bool needsValue => (valueType == ValueType.Value) && (variable == null);
        private bool needsVariable => (valueType == ValueType.Value) && (valueHandler == null);


        public override void Execute()
        {
            if (!enableAction) return;
            if (!animator) return;
            if (animationParameter == "") return;
            if (!EvaluatePreconditions()) return;

            switch (valueType)
            {
                case ValueType.Int:
                    animator.SetInteger(animationParameter, integerValue);
                    break;
                case ValueType.Float:
                    animator.SetFloat(animationParameter, floatValue);
                    break;
                case ValueType.Boolean:
                    animator.SetBool(animationParameter, boolValue);
                    break;
                case ValueType.Trigger:
                    animator.SetTrigger(animationParameter);
                    break;
                case ValueType.Value:
                    var v = variable;
                    if (valueHandler) v = valueHandler.GetVariable();
                    if (v != null)
                    {
                        animator.SetFloat(animationParameter, v.currentValue);
                    }
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Set Animation Parameter";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            if (animator == null)
            {
                desc += "sets the value of a animation parameter to a value (animator set set)!";
            }
            else
            {
                switch (valueType)
                {
                    case ValueType.Int:
                        desc += $"sets animation parameter {animationParameter} of object {animator.name} to {integerValue}";
                        break;
                    case ValueType.Float:
                        desc += $"sets animation parameter {animationParameter} of object {animator.name} to {floatValue}";
                        break;
                    case ValueType.Boolean:
                        desc += $"sets animation parameter {animationParameter} of object {animator.name} to {boolValue}";
                        break;
                    case ValueType.Trigger:
                        desc += $"triggers the animation parameter {animationParameter} of object {animator.name}";
                        break;
                    case ValueType.Value:
                        if (valueHandler)
                        {
                            desc += $"sets animation parameter {animationParameter} of object {animator.name} to the value of {valueHandler.name}";
                        }
                        else
                        {
                            desc += $"sets animation parameter {animationParameter} of object {animator.name} to the value of variable {variable.name}";
                        }
                        break;
                }
            }

            return desc;
        }
    }
}