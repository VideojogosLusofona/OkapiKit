using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{

    [AddComponentMenu("Okapi/Action/Play Particle System")]
    public class ActionPlayParticleSystem : Action
    {
        [SerializeField] private ParticleSystem target;

        public override string GetActionTitle() => "Play Particle System";

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if (target == null)
            {
                target = GetComponent<ParticleSystem>();
            }
            if (target)
            {
                target.Play();
            }
        }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string targetName = (target) ? (target.name) : (name);

            return $"{GetPreconditionsString(gameObject)}play particle system {targetName}";
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (target == null)
            {
                if (GetComponent<ParticleSystem>() == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined particle system!", "We need to know what particle system to play"));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Particle system to play is this object, but it should be explicitly linked!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                }
            }
        }
    }
}