using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Windows;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Trigger/On Timer")]
    public class TriggerOnTimer : Trigger
    {
        [SerializeField]
        private bool startTriggered = false;
        [SerializeField]
        private bool initialDelayEnable = false;
        [SerializeField, MinMaxSlider(0.0f, 60.0f)]
        private Vector2 initialDelay = new Vector2(5.0f, 5.0f);
        [SerializeField, MinMaxSlider(0.0f, 60.0f)]
        private Vector2 timeInterval = new Vector2(5.0f, 5.0f);

        private float timer;

        public override string GetTriggerTitle() => "On Timer";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string text = "";

            if (timeInterval.x == timeInterval.y)
            {
                if (allowRetrigger)
                {
                    text = $"Every {timeInterval.x} seconds, trigger the actions:";
                }
                else
                    text = $"After {timeInterval.x} seconds, trigger the actions:";
            }
            else
            {
                if (allowRetrigger)
                    text = $"Every {timeInterval.x} to {timeInterval.y} seconds, trigger the actions:";
                else
                    text = $"After {timeInterval.x} to {timeInterval.y} seconds, trigger the actions:";
            }

            if ((allowRetrigger) && (!startTriggered) && (initialDelayEnable))
            {
                text = char.ToLower(text[0]) + text.Substring(1);
                if (initialDelay.x == initialDelay.y)
                {
                    text = $"Wait for {initialDelay.x} seconds, and then {text}";
                }
                else
                {
                    text = $"Wait for between {initialDelay.x} and {initialDelay.y} seconds, and then {text}";
                }
            }

            return text;
        }

        void Start()
        {
            timer = Random.Range(timeInterval.x, timeInterval.y);
            if (isTriggerEnabled)
            {
                if (startTriggered)
                {
                    ExecuteTrigger();
                }
                else if (initialDelayEnable)
                {
                    timer = Random.Range(initialDelay.x, initialDelay.y);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isTriggerEnabled) return;

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                if (EvaluatePreconditions())
                {
                    ExecuteTrigger();
                }

                timer += Random.Range(timeInterval.x, timeInterval.y);
            }
        }
    }
}