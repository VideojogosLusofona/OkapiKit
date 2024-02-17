using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class Shaker : OkapiElement
    {
        public enum RunType { FixedUpdate = 0, Update = 1, LateUpdate = 2 };

        private Vector3     prevDelta;
        private float       timer;
        private float       strength;
        private RunType     runType = RunType.LateUpdate;

        private void Awake()
        {
            prevDelta = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (runType != RunType.FixedUpdate) return;
            Run();
        }

        private void Update()
        {
            if (runType != RunType.Update) return;
            Run();
        }
        private void LateUpdate()
        {
            if (runType != RunType.LateUpdate) return;
            Run();
        }

        void Run()
        {
            // Revert previous movement
            transform.position -= prevDelta;

            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
                
                if (timer < 0.0f)
                {
                    // Eliminate this shaker
                    Destroy(this);
                    return;
                }

                prevDelta = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
                prevDelta = prevDelta.normalized * strength;

                transform.position += prevDelta;
            }
            else prevDelta = Vector3.zero;
        }

        public void Run(float duration, float strength, RunType runType = RunType.LateUpdate)
        {
            if ((duration > timer) || (strength > this.strength))
            {
                // This one will be the priority
                timer = duration;
                this.strength = strength;
                this.runType = runType;
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "This is a system to take care of the shaking animation.";
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }
    }
}
