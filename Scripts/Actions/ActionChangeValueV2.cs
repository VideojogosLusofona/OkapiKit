using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Value")]
    public class ActionChangeValueV2 : Action
    {
        public enum OperationType { Reset = 0, Set = 1, Add = 2, Subtract = 3, RevSubtract = 4, Multiply = 5, Divide = 6, RevDivide = 7 };

        [SerializeField]
        private VariableInstance    valueHandler;
        [SerializeField]
        private Variable            variable;
        [SerializeField]
        private OperationType       operation = OperationType.Reset;
        [SerializeField]
        private float               changeValue;
        [SerializeField]
        private VariableInstance    changeValueHandler;
        [SerializeField]
        private Variable            changeVariable;
        [SerializeField]
        private bool                scaleWithTime = false;

        public override string GetActionTitle() { return "Change Value"; }

        string GetChangeValueString()
        {
            if (changeValueHandler)
            {
                return $"value of {changeValueHandler.name}";
            }
            if (changeVariable)
            {
                return $"value of variable {changeVariable.name}";
            }

            return $"{changeValue}";
        }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string n = "[UNKNOWN]";
            if (variable) n = variable.name;
            else if (valueHandler) n = valueHandler.name;

            string desc = GetPreconditionsString(gameObject);

            switch (operation)
            {
                case OperationType.Reset:
                    desc += $"resets variable {n}";
                    break;
                case OperationType.Set:
                    desc += $"sets variable {n} to {GetChangeValueString()}";
                    break;
                case OperationType.Add:
                    desc += $"adds {GetChangeValueString()} to variable {n}";
                    if (scaleWithTime) desc += ", every second.";
                    break;
                case OperationType.Subtract:
                    desc += $"subtracts {GetChangeValueString()} from variable {n}";
                    if (scaleWithTime) desc += ", every second.";
                    break;
                case OperationType.RevSubtract:
                    desc += $"subtracts variable {n} from {GetChangeValueString()}";
                    if (scaleWithTime) desc += ", every second.";
                    break;
                case OperationType.Multiply:
                    desc += $"multiplies {n} by {GetChangeValueString()}";
                    if (scaleWithTime) desc += ", every second.";
                    break;
                case OperationType.Divide:
                    desc += $"divides {n} by {GetChangeValueString()}";
                    if (scaleWithTime) desc += ", every second.";
                    break;
                case OperationType.RevDivide:
                    desc += $"divides {GetChangeValueString()} by {n}";
                    if (scaleWithTime) desc += ", every second.";
                    break;
                default:
                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((variable == null) && (valueHandler == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined target variable!", "We're changing the value of a variable, so we need a target so we know which variable to change."));
            }
            if ((operation == OperationType.Add) || (operation == OperationType.Subtract))
            {
                if ((changeValueHandler == null) && (changeVariable == null) && (changeValue == 0.0f))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Value is not changing (changing by zero) - select a change value!", "Changing a value by zero is the same as not changing it, which I guess is not what you intended"));
                }
            }
            if ((operation == OperationType.Multiply) || (operation == OperationType.Divide))
            {
                if ((changeValueHandler == null) && (changeVariable == null) && (changeValue == 1.0f))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Value is not changing (changing by one) - select a change value!", "Multiplying or dividng a value by zero is the same as not changing it, which I guess is not what you intended"));
                }
            }
            if (operation == OperationType.Divide)
            {
                if ((changeValueHandler == null) && (changeVariable == null) && (changeValue == 0.0f))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Can't divide by zero!", "Division by zero is impossible!"));
                }
            }
        }
        float GetCurrentValue()
        {
            float f = 0.0f;
            if (valueHandler != null) f = valueHandler.currentValue;
            else if (variable != null) f = variable.currentValue;

            return f;
        }

        float GetChangeValue()
        {
            float f = 0.0f;
            if (changeValueHandler != null) f = changeValueHandler.currentValue;
            else if (changeVariable != null) f = changeVariable.currentValue;
            else f = changeValue;

            if ((scaleWithTime) && (operation != OperationType.Set)) f *= Time.deltaTime;

            return f;
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if (operation == OperationType.Reset)
            {
                if (valueHandler != null) valueHandler.Reset();
                else if (variable != null) variable.ResetValue();
                return;
            }

            float currentChangeValue = GetCurrentValue();
            float deltaChangeValue = GetChangeValue();
            float finalValue = deltaChangeValue;
            switch (operation)
            {
                case OperationType.Add:
                    finalValue = currentChangeValue + deltaChangeValue;
                    break;
                case OperationType.Subtract:
                    finalValue = currentChangeValue - deltaChangeValue;
                    break;
                case OperationType.RevSubtract:
                    finalValue = deltaChangeValue - currentChangeValue;
                    break;
                case OperationType.Multiply:
                    finalValue = currentChangeValue * deltaChangeValue;
                    break;
                case OperationType.Divide:
                    finalValue = currentChangeValue / deltaChangeValue;
                    break;
                case OperationType.RevDivide:
                    finalValue = deltaChangeValue / currentChangeValue;
                    break;
                default:
                    break;
            }

            if (valueHandler != null) valueHandler.SetValue(finalValue);
            else if (variable != null) variable.SetValue(finalValue);
        }
    }
}