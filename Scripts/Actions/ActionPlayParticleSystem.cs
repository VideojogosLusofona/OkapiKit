using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{

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
    }
}