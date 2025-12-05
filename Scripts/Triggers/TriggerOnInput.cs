using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Trigger/On Input")]
    public class TriggerOnInput : Trigger
    {
        public enum InputType { Button = 0, Key = 1, Axis = 2, AnyKey = 3 };

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
        [SerializeField] 
        protected ActionTrigger[] elseActions;

        float cooldownTimer = 0.0f;
        bool prevAnyKey = false;

        public override string GetTriggerTitle() => "On Input";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "When ";
            if (inputType == InputType.Button) desc += $"button {buttonName} ";
            else if (inputType == InputType.Key) desc += $"key {key} ";
            else if (inputType == InputType.Axis)
            {
                desc += $"axis {axis} ";
            }
            else
            {
                desc += $"any key ";
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

        protected override string Internal_UpdateExplanation()
        {
            base.Internal_UpdateExplanation();

            if ((elseActions != null) && (elseActions.Length > 0))
            {
                _explanation += "else\n" + GetDescriptionActions(elseActions);
            }

            return _explanation;
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
            bool elseTrigger = false;
            bool c = continuous;

            if (inputType == InputType.Button)
            {
                isTrigger = (continuous) ? (Input.GetButton(buttonName)) : (Input.GetButtonDown(buttonName));
                elseTrigger = (continuous) ? !(Input.GetButton(buttonName)) : (Input.GetButtonUp(buttonName));
            }
            else if (inputType == InputType.Key)
            {
                isTrigger = (continuous) ? (Input.GetKey(key)) : (Input.GetKeyDown(key));
                elseTrigger = (continuous) ? (!Input.GetKey(key)) : (Input.GetKeyUp(key));
            }
            else if (inputType == InputType.Axis)
            {
                float a = Input.GetAxis(axis);
                isTrigger = (a < deadArea.x) || (a > deadArea.y);
                elseTrigger = !isTrigger;
                c = true;
            }
            else if (inputType == InputType.AnyKey)
            {
                if (continuous)
                {
                    isTrigger = Input.anyKey;
                    elseTrigger = !Input.anyKey;
                }
                else
                {
                    isTrigger = Input.anyKeyDown;
                    elseTrigger = !Input.anyKey && prevAnyKey;
                }
                prevAnyKey = Input.anyKey;
            }

            if ((c) && (negate))
            {
                isTrigger = !isTrigger;
                elseTrigger = !elseTrigger;
            }

            if (isTrigger)
            {
                ExecuteTrigger();

                if (useCooldown) cooldownTimer = cooldown;
            }
            if (elseTrigger)
            {
                ExecuteElseTrigger();
            }
        }

        public virtual void ExecuteElseTrigger()
        {
            if (!enableTrigger) return;

            foreach (var action in elseActions)
            {
                if (action.action != null)
                {
                    if (action.action.isActionEnabled)
                    {
                        if (action.delay > 0)
                        {
                            StartCoroutine(ExecuteTriggerCR(action));
                        }
                        else
                        {
                            action.action.Execute();
                        }
                    }
                }
            }
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if ((elseActions != null) && (elseActions.Length > 0))
            {
                int index = 0;
                foreach (var action in elseActions)
                {
                    if (action.action == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Warning, $"Else action {index} in not defined on else action list!", "Empty actions don't do anything, so either remove it or fill it in."));
                    }
                    else
                    {
                        action.action.ForceCheckErrors(level + 1);
                        var actionLogs = action.action.logs;
                        foreach (var log in actionLogs)
                        {
                            _logs.Add(new LogEntry(log.type, $"On else action {index}: " + log.text, log.tooltip));
                        }

                        var destroyObjectAction = action.action as ActionDestroyObject;
                        if (destroyObjectAction != null)
                        {
                            if (destroyObjectAction.WillDestroyThis(gameObject))
                            {
                                // This is a destroy action
                                if (index < actions.Length - 1)
                                {
                                    _logs.Add(new LogEntry(LogEntry.Type.Warning, $"Else action on index {index} is a Destroy Object action!", "Destroy actions should always be the last actions of the sequence of action, because they will destroyu the object and stop other actions after from being executed most of the times!"));
                                }
                            }
                        }
                    }
                    index++;
                }
            }
            if (inputType == InputType.Button)
            {
                CheckButton("Button", buttonName);
            }
        }

    }
}
