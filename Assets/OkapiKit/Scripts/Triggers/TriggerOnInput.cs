using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class TriggerOnInput : Trigger
    {
        public enum InputType { Button = 0, Key = 1, Axis = 2 };

        [SerializeField]
        InputType inputType = InputType.Button;
        [SerializeField]
        private string buttonName;
        [SerializeField]
        private KeyCode key;
        [SerializeField, InputAxis]
        private string axis;
        [SerializeField, MinMaxSlider(-1.0f, 1.0f)]
        private Vector2 deadArea = new Vector2(-0.2f, 0.2f);
        [SerializeField]
        private bool continuous = true;
        [SerializeField]
        private bool negate = false;
        [SerializeField]
        private bool useCooldown = false;
        [SerializeField]
        private float cooldown = 1.0f;

        float cooldownTimer = 0.0f;

        public override string GetTriggerTitle() => "On Input";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "When ";
            if (inputType == InputType.Button) desc += $"button {buttonName} ";
            else if (inputType == InputType.Key) desc += $"key {key} ";
            else
            {
                desc += $"axis {axis} ";
            }
            if (continuous)
            {
                if (negate) desc += "is not held";
                else
                {
                    desc += "is held";
                }
            }
            else
            {
                desc += "is pressed";
            }
            if (useCooldown) desc += " (this can only happen every " + cooldown + " seconds)";

            return desc;
        }

        void Update()
        {
            if (!isTriggerEnabled) return;
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
            bool c = continuous;

            if (inputType == InputType.Button)
            {
                isTrigger = (continuous) ? (Input.GetButton(buttonName)) : (Input.GetButtonDown(buttonName));
            }
            else if (inputType == InputType.Key)
            {
                isTrigger = (continuous) ? (Input.GetKey(key)) : (Input.GetKeyDown(key));
            }
            else if (inputType == InputType.Axis)
            {
                float a = Input.GetAxis(axis);
                isTrigger = (a < deadArea.x) || (a > deadArea.y);
                if (isTrigger)
                {
                    int ba = 10;
                    ba++;
                }
                c = true;
            }

            if ((c) && (negate)) isTrigger = !isTrigger;

            if (isTrigger)
            {
                ExecuteTrigger();

                if (useCooldown) cooldownTimer = cooldown;
            }
        }
    }
}