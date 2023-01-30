using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class ValueHandler : MonoBehaviour
{
    [SerializeField] 
    private Variable    value;
    [SerializeField, ShowIf("hasInternalValue")]
    private float currentValue = 0;
    [SerializeField, ShowIf("hasInternalValue")]
    private float defaultValue = 0;
    [SerializeField, ShowIf("hasInternalValue")]
    private bool isInteger = false;
    [SerializeField, ShowIf("hasInternalValue")]
    private bool hasLimits = true;
    [SerializeField, ShowIf("hasLimitsAndInternalValue")]
    private float minValue = -float.MaxValue;
    [SerializeField, ShowIf("hasLimitsAndInternalValue")]
    private float maxValue = float.MaxValue;
    [SerializeField]
    private Action[]    onChange;
    [SerializeField]
    private Action[]    onIncrease;
    [SerializeField]
    private Action[]    onDecrease;

    private bool hasInternalValue => value == null;
    private bool hasLimitsAndInternalValue => hasLimits && value == null;


    void Start()
    {
        if (value == null)
        {
            value = ScriptableObject.CreateInstance<Variable>();
            value.SetProperties(currentValue, defaultValue, isInteger, hasLimits, minValue, maxValue);
        }
    }

    public Variable GetVariable()
    {
        return value;
    }

    public void SetValue(float value)
    {
        float delta = value - this.value.currentValue;
        ChangeValue(value);
    }

    public void Reset()
    {
        this.value.ResetValue();
    }

    public void ChangeValue(float value)
    {
        //Debug.Log($"Change value {name}: Old = {this.value.currentValue}, New = {this.value.currentValue + value}");

        float prevValue = this.value.currentValue;

        this.value.ChangeValue(value);

        if (prevValue != this.value.currentValue)
        {
            foreach (var action in onChange)
            {
                action.Execute();
            }
            if (value > 0)
            {
                foreach (var action in onIncrease)
                {
                    action.Execute();
                }
            }
            else if (value < 0)
            {
                foreach (var action in onDecrease)
                {
                    action.Execute();
                }
            }
        }
    }
}
