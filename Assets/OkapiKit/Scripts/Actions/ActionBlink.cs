using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Blink")]
    public class ActionBlink : Action
    {
        [SerializeField] private Renderer target;
        [SerializeField] private bool includeChildren;
        [SerializeField] private float blinkTimeOn = 0.2f;
        [SerializeField] private float blinkTimeOff = 0.2f;
        [SerializeField] private float duration = 2.0f;

        bool startState;
        float timer;
        float blinkPhaseTimer;
        List<Renderer> renderers;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;
            if (target == null) return;

            startState = target.enabled;
            EnableRenderers(!startState);
            timer = duration;
            blinkPhaseTimer = (target.enabled) ? (blinkTimeOn) : (blinkTimeOff);
        }

        public override string GetActionTitle() => "Blink";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            if (target == null)
            {
                if (includeChildren)
                    desc += $"blinks the renderers below this object for {duration} seconds";
                else
                    desc += $"blinks this renderer for {duration} seconds";
            }
            else
            {
                if (includeChildren)
                    desc += $"blinks renderer {target.name} and all his children for {duration} seconds";
                else
                    desc += $"blinks renderer {target.name} for {duration} seconds";
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (GetTarget() == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No renderer available for blinking - place one in this object or in a child object", "Blinking is turning on/off a renderer, so for this to work, there has to be at least one renderer on this object, or reference a renderer."));
            }
        }

        protected override void Awake()
        {
            base.Awake();

            target = GetTarget();

            if (target)
            {
                renderers = new List<Renderer>();
                renderers.Add(target);

                if (includeChildren)
                {
                    var allRenderers = target.GetComponentsInChildren<Renderer>();
                    foreach (var r in allRenderers)
                    {
                        if (!renderers.Contains(r)) renderers.Add(r);
                    }
                }
            }
            timer = 0;
        }

        protected Renderer GetTarget()
        {
            Renderer ret = target;
            if (ret == null)
            {
                ret = GetComponent<Renderer>();
                if (ret == null)
                {
                    ret  = GetComponentInChildren<Renderer>();
                }
            }

            return ret;
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    EnableRenderers(startState);
                }
                else
                {
                    blinkPhaseTimer -= Time.deltaTime;
                    if (blinkPhaseTimer <= 0)
                    {
                        EnableRenderers(!target.enabled);
                        blinkPhaseTimer = (target.enabled) ? (blinkTimeOn) : (blinkTimeOff);
                    }
                }
            }
        }

        void EnableRenderers(bool b)
        {
            foreach (var r in renderers)
            {
                r.enabled = b;
            }
        }
    }
};