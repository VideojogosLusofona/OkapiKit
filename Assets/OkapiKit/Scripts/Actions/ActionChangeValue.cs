using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionChangeValue : Action
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
    [SerializeField, ShowIf("isChange")] 
    private bool            scaleWithTime = false;
    [SerializeField, ShowIf("isSet")] 
    private float           value;

    private bool needValueHandler => (variable == null);
    private bool needValueVariable => (valueHandler == null);
    private bool isChange => operation == OperationType.Change;
    private bool isSet => operation == OperationType.Set;

    public override string GetActionTitle() { return "Change Value"; }

    public override string GetRawDescription(string ident)
    {
        string n = "[]";
        if (variable) n = variable.name;
        else if (valueHandler) n = valueHandler.name;

        string desc = GetPreconditionsString();

        if (operation == OperationType.Set)
        {
            desc += $"Sets variable {n} to {value}";
        }
        else
        {
            if (scaleWithTime) desc += $"Adds {deltaValue} every second to variable {n}";
            else desc += $"Adds {deltaValue} to variable {n}";
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
                default:
                    break;
            }
        }
    }
}
