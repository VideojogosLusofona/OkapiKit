using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Shake")]
    public class ActionShake : Action
    {
        [SerializeField] private Hypertag targetTag;
        [SerializeField] private Transform targetObject;
        [SerializeField] private float strength = 10;
        [SerializeField] private float duration = 0.5f;

        Vector3 prevDelta;
        float timer;
        Transform target;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            timer = duration;

            target = null;
            if (targetObject)
            {
                target = targetObject;
            }
            else if (targetTag)
            {
                var potentialObjects = gameObject.FindObjectsOfTypeWithHypertag<Transform>(targetTag);
                var minDist = float.MaxValue;
                foreach (var obj in potentialObjects)
                {
                    var d = Vector3.Distance(obj.position, transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        target = obj;
                    }
                }
            }
        }


        public override string GetActionTitle() => "Shake";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            if (targetObject == null)
            {
                if (targetTag == null)
                {
                    desc += $"shakes this object with intensity {strength} for {duration} seconds";
                }
                else
                {
                    desc += $"shakes the object with tag [{targetTag.name}] with intensity {strength} for {duration} seconds";
                }
            }
            else
            {
                desc += $"shakes the object [{targetObject.name}] with intensity {strength} for {duration} seconds";
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((targetObject == null) && (targetTag == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Shake target is this object, but it should be set explicitly!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
            }
            if (strength == 0.0f)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Shake strength is zero, object will not shake!", "Strength is how much the object will shake, in world coordinates (pixels).\nZero strength means the object will not shake."));
            }
            if (duration <= 0.0f)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Shake duration is less or equal to zero, object will not shake!", "Duration is for how long (in seconds) the shake will happen. A duration of zero means the object will not shake at all."));
            }
        }

        protected override void Awake()
        {
            base.Awake();

            prevDelta = Vector3.zero;
            timer = 0;
            target = null;
        }

        void LateUpdate()
        {
            if (target == null)
            {
                target = transform;
            }

            // Revert previous movement
            target.position -= prevDelta;

            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;

                prevDelta = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
                prevDelta = prevDelta.normalized * strength;

                target.position += prevDelta;
            }
            else prevDelta = Vector2.zero;
        }
    }
}