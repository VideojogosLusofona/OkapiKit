using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ActionChangeValue : Action
    {
        public enum OperationType { Set = 0, Change = 1, Reset = 2 };

        [SerializeField, ShowIf("needValueHandler")]
        private VariableInstance valueHandler;
        [SerializeField, ShowIf("needValueVariable")]
        private Variable variable;
        [SerializeField]
        private OperationType operation = OperationType.Change;
        [SerializeField, ShowIf("isChange")]
        private float deltaValue;
        [SerializeField, ShowIf("isChange")]
        private bool scaleWithTime = false;
        [SerializeField, ShowIf("isSet")]
        private float value;

        private bool needValueHandler => (variable == null);
        private bool needValueVariable => (valueHandler == null);
        private bool isChange => operation == OperationType.Change;
        private bool isSet => operation == OperationType.Set;

        public override string GetActionTitle() { return "Change Value"; }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string n = "[UNKNOWN]";
            if (variable) n = variable.name;
            else if (valueHandler) n = valueHandler.name;

            string desc = GetPreconditionsString(gameObject);

            if (operation == OperationType.Set)
            {
                desc += $"sets variable {n} to {value}";
            }
            else if (operation == OperationType.Reset)
            {
                desc += $"resets variable {n}";
            }
            else
            {
                if (scaleWithTime) desc += $"adds {deltaValue} every second to variable {n}";
                else desc += $"adds {deltaValue} to variable {n}";
            }

            return desc;
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if (valueHandler != null)
            {
                switch (operation)
                {
                    case OperationType.Set:
                        valueHandler.SetValue(value);
                        break;
                    case OperationType.Change:
                        if (scaleWithTime) valueHandler.ChangeValue(deltaValue * Time.deltaTime);
                        else valueHandler.ChangeValue(deltaValue);
                        break;
                    case OperationType.Reset:
                        valueHandler.Reset();
                        break;
                    default:
                        break;
                }
            }
            else if (variable != null)
            {
                switch (operation)
                {
                    case OperationType.Set:
                        variable.SetValue(value);
                        break;
                    case OperationType.Change:
                        if (scaleWithTime) variable.ChangeValue(deltaValue * Time.deltaTime);
                        else variable.ChangeValue(deltaValue);
                        break;
                    case OperationType.Reset:
                        variable.ResetValue();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}