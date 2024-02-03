using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Set Animation Parameter")]
    public class ActionSetAnimationParameter : Action
    {
        public enum ValueType { Int = 0, Float = 1, Boolean = 2, Trigger = 3, Value = 4, VelocityX = 5, VelocityY = 6 };

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
        [SerializeField, ShowIf("needsAbsolute")]
        private bool absolute = false;

        private bool needsInt => valueType == ValueType.Int;
        private bool needsFloat => valueType == ValueType.Float;
        private bool needsBoolean => valueType == ValueType.Boolean;
        private bool needsValue => (valueType == ValueType.Value) && (variable == null);
        private bool needsVariable => (valueType == ValueType.Value) && (valueHandler == null);
        private bool needsAbsolute => ((valueType == ValueType.VelocityX) || (valueType == ValueType.VelocityY));

        Vector3     prevPos;
        Vector3     currentVelocity;

        protected override void Awake()
        {
            base.Awake();

            prevPos = transform.position;
        }

        protected void FixedUpdate()
        {
            currentVelocity = (transform.position - prevPos) / Time.fixedDeltaTime;
            prevPos = transform.position;
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (animationParameter == "") return;
            if (!EvaluatePreconditions()) return;

            if (animator == null) animator = GetComponent<Animator>();
            if (!animator) return;

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
                        if (v.type == Variable.Type.Float)
                            animator.SetFloat(animationParameter, v.currentValue);
                        else if (v.type == Variable.Type.Integer)
                            animator.SetInteger(animationParameter, (int)v.currentValue);
                    }
                    break;
                case ValueType.VelocityX:
                    animator.SetFloat(animationParameter, (absolute) ? (Mathf.Abs(currentVelocity.x)) : (currentVelocity.x));
                    break;
                case ValueType.VelocityY:
                    animator.SetFloat(animationParameter, (absolute) ? (Mathf.Abs(currentVelocity.y)) : (currentVelocity.y));
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Set Animation Parameter";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            var anm = animator;
            if (anm == null) anm = GetComponent<Animator>();

            if (anm == null)
            {
                desc += "sets the value of a animation parameter to a value (animator not set)!";
            }
            else
            {
                switch (valueType)
                {
                    case ValueType.Int:
                        desc += $"sets animation parameter {animationParameter} of object {anm.name} to {integerValue}";
                        break;
                    case ValueType.Float:
                        desc += $"sets animation parameter {animationParameter} of object {anm.name} to {floatValue}";
                        break;
                    case ValueType.Boolean:
                        desc += $"sets animation parameter {animationParameter} of object {anm.name} to {boolValue}";
                        break;
                    case ValueType.Trigger:
                        desc += $"triggers the animation parameter {animationParameter} of object {anm.name}";
                        break;
                    case ValueType.Value:
                        if (valueHandler)
                        {
                            desc += $"sets animation parameter {animationParameter} of object {anm.name} to the value of {valueHandler.name}";
                        }
                        else if (variable)
                        {
                            desc += $"sets animation parameter {animationParameter} of object {anm.name} to the value of variable {variable.name}";
                        }
                        else
                        {
                            desc += $"sets animation parameter {animationParameter} of object {anm.name} to the value of [UNDEFINED]";
                        }
                        break;
                    case ValueType.VelocityX:
                        if (absolute) desc += $"sets animation parameter {animationParameter} of object {anm.name} to the absolute horizontal velocity of this object";
                        else desc += $"sets animation parameter {animationParameter} of object {anm.name} to the horizontal velocity of this object";
                        break;
                    case ValueType.VelocityY:
                        if (absolute) desc += $"sets animation parameter {animationParameter} of object {anm.name} to the absolute vertical velocity of this object";
                        else desc += $"sets animation parameter {animationParameter} of object {anm.name} to the vertical velocity of this object";
                        break;
                }
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            var anm = animator;
            if (anm == null)
            {
                anm = GetComponent<Animator>();
                if (anm == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined animator!", "This action changes some parameter in the animator, so we need to have a target animator, or an animator in the same object."));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Animator to modify is on this object, but it should be explicitly linked!", "This works as it is, but it's usually good practice to explicitly link the animator component to this action, in that case if something changes this is more likely it will work."));
                }
            }

            if (anm) 
            {
                if (anm.runtimeAnimatorController == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Linked animator is missing a controller!", "An animator without an animation controller doesn't do anything. Create one, and assign it to the linked controller!"));
                }
                else
                {
                    if (animationParameter != "")
                    {
                        bool found = false;
                        bool rightType = false;
                        string pType = "[UNKNOWN]";
                        
                        for (int i = 0; i < anm.parameterCount; i++)
                        {
                            var param = anm.GetParameter(i);
                            if (param.name == animationParameter)
                            {
                                found = true;
                                pType = param.type.ToString();
                                switch (param.type)
                                {
                                    case AnimatorControllerParameterType.Float:
                                        if ((valueType == ValueType.Float) || (valueType == ValueType.VelocityX) || (valueType == ValueType.VelocityY))
                                        {
                                            rightType = true;
                                        }                                        
                                        else if (valueType == ValueType.Value)
                                        {
                                            if (variable) rightType = (variable.type == Variable.Type.Float);
                                            else if (valueHandler) rightType = (valueHandler.type == Variable.Type.Float);
                                        }                                            
                                        break;
                                    case AnimatorControllerParameterType.Int:
                                        if (valueType == ValueType.Int)
                                        {
                                            rightType = true;
                                        }
                                        else if (valueType == ValueType.Value)
                                        {
                                            if (variable) rightType = (variable.type == Variable.Type.Integer);
                                            else if (valueHandler) rightType = (valueHandler.type == Variable.Type.Integer);
                                        }
                                        break;
                                    case AnimatorControllerParameterType.Bool:
                                        rightType = (valueType == ValueType.Boolean);
                                        break;
                                    case AnimatorControllerParameterType.Trigger:
                                        rightType = (valueType == ValueType.Trigger);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            }
                        }

                        if (!found)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Parameter animator {animationParameter} is missing!", $"You're trying to set the {animationParameter} parameter, but the animator doesn't have a parameter with that name!"));
                        }
                        else
                        {
                            if (!rightType)
                            {
                                _logs.Add(new LogEntry(LogEntry.Type.Error, $"Value is of wrong type for animator parameter {animationParameter} (should be {pType})!", $"The parameter you're trying to set is of type {pType} but you're setting it to a value of type {valueType}. Change the type of the parameter on the animator, or on this action."));
                            }
                        }
                    }
                    else
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined animation parameter!", "You need to set the name of the parameter you want to change!"));
                    }
                }
            }
            if (valueType == ValueType.Value)
            {
                if ((valueHandler == null) && (variable == null))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined variable!", "You're trying to set the parameter to the value of a variable, but there's no variable set!"));
                }
            }
        }
    }
}