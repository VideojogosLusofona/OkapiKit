using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class TriggerOnTimer : Trigger
    {
        [SerializeField]
        private bool startTriggered = false;
        [SerializeField, MinMaxSlider(0.0f, 10.0f)]
        private Vector2 timeInterval = new Vector2(5.0f, 5.0f);

        private float timer;

        public override string GetTriggerTitle() => "On Timer";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            if (timeInterval.x == timeInterval.y)
            {
                if (allowRetrigger)
                    return $"Every {timeInterval.x} seconds, trigger the actions:";
                else
                    return $"After {timeInterval.x} seconds, trigger the actions:";
            }

            if (allowRetrigger)
                return $"Every [{timeInterval.x},{timeInterval.y}] seconds, trigger the actions:";
            else
                return $"After [{timeInterval.x},{timeInterval.y}] seconds, trigger the actions:";
        }

        void Start()
        {
            if (isTriggerEnabled)
            {
                if (startTriggered)
                {
                    ExecuteTrigger();
                }
            }
            timer = Random.Range(timeInterval.x, timeInterval.y);
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