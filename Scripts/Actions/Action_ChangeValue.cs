using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Action_ChangeValue : Action
{
    [SerializeField] enum OperationType { Set, Change };

    [SerializeField, ShowIf("needValueHandler")]
    private ValueHandler    valueHandler;
    [SerializeField, ShowIf("needValueVariable")]
    private Variable        variable;
    [SerializeField] 
    private OperationType   operation = OperationType.Change;
    [SerializeField, ShowIf("isChange")] 
    private float           deltaValue;
    [SerializeField, ShowIf("isSet")] 
    private float           value;

    private bool needValueHandler => (variable == null);
    private bool needValueVariable => (valueHandler == null);
    private bool isChange => operation == OperationType.Change;
    private bool isSet => operation == OperationType.Set;

    public override string GetRawDescription(string ident)
    {
        string n = "[]";
        if (variable) n = variable.name;
        else if (valueHandler) n = valueHandler.name;

        if (operation == OperationType.Set)
        {
            return $"Sets variable {n} to {value}";
        }

        return $"Adds {deltaValue} to variable {n}";
    }

    public override void Execute()
    {
        if (!enableAction) return;

        if (valueHandler != null) 
        {
            switch (operation)
            {
                case OperationType.Set:
                    valueHandler.SetValue(value);
                    break;
                case OperationType.Change:
                    valueHandler.ChangeValue(deltaValue);
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
                    variable.ChangeValue(deltaValue);
                    break;
                default:
                    break;
            }
        }
    }
}
