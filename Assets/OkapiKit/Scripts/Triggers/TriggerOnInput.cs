using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TriggerOnInput : Trigger
{
    public enum InputType { Button = 0, Key = 1};

    [SerializeField]
    InputType       inputType = InputType.Button;
    [SerializeField, ShowIf("isButton")]
    private string  buttonName;
    [SerializeField, ShowIf("isKey")]
    private KeyCode key;
    [SerializeField]
    private bool    continuous = true;
    [SerializeField, ShowIf("continuous")]
    private bool    negate = false;
    [SerializeField, ShowIf("continuousAndNotNegated")]
    private bool    useCooldown = false;
    [SerializeField, ShowIf("useCooldown")]
    private float   cooldown = 1.0f;

    private bool isButton => inputType == InputType.Button;
    private bool isKey => inputType == InputType.Key;
    private bool continuousAndNotNegated => continuous && (!negate);


    float cooldownTimer = 0.0f;

    public override string GetTriggerTitle() => "On Input";

    protected override string GetRawDescription()
    {
        string desc = "When ";
        if (inputType == InputType.Button) desc += "button " + buttonName + " ";
        else desc += "key " + key + " ";
        if (continuous)
        {
            if (negate) desc += "is not held";
            else
            {
                desc += "is held";
                if (useCooldown) desc += " (this can only happen every " + cooldown + " seconds)";
            }
        }
        else desc += "is pressed";

        return desc;
    }

    void Update()
    {
        if (!EvaluatePreconditions()) return;

        if (cooldownTimer >= 0.0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if ((useCooldown) && (cooldownTimer > 0))
        {
            return;
        }

        bool isTrigger = false;

        if (inputType == InputType.Button)
        {
            isTrigger = (continuous) ? (Input.GetButton(buttonName)) : (Input.GetButtonDown(buttonName));
        }
        else
        {
            isTrigger = (continuous) ? (Input.GetKey(key)) : (Input.GetKeyDown(key));
        }
        if ((continuous) && (negate)) isTrigger = !isTrigger;

        if (isTrigger)
        {
            ExecuteTrigger();

            if (useCooldown) cooldownTimer = cooldown;
        }
    }
}