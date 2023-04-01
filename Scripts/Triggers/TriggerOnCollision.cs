using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class TriggerOnCollision : Trigger
    {
        public enum CollisionEvent { Enter, Stay, Exit };

        [SerializeField]
        private bool isTrigger = true;
        [SerializeField]
        private CollisionEvent eventType;
        [SerializeField]
        private Hypertag[] tags;

        public override string GetTriggerTitle() { return "On Collision"; }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            var desc = "";
            if (eventType == CollisionEvent.Stay) desc = "While a collision with ";
            else desc = "When a collision with ";

            if (isTrigger) desc += "a trigger ";
            else desc += "a bounding volume ";
            if ((tags != null) && (tags.Length > 0))
            {
                desc += "with tags [";
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i] == null) desc += "NULL";
                    else desc += tags[i].name;
                    if (i < tags.Length - 1) desc += ",";
                }
                desc += "] ";
            }
            switch (eventType)
            {
                case CollisionEvent.Enter: desc += "starts"; break;
                case CollisionEvent.Stay: desc += "happens"; break;
                case CollisionEvent.Exit: desc += "ends"; break;
            }

            return desc;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isTrigger) return;
            if (!isTriggerEnabled) return;
            if (eventType != CollisionEvent.Enter) return;
            if (!collision.gameObject.HasHypertags(tags)) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isTrigger) return;
            if (!isTriggerEnabled) return;
            if (eventType != CollisionEvent.Enter) return;
            if (!collision.gameObject.HasHypertags(tags)) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!isTrigger) return;
            if (!isTriggerEnabled) return;
            if (eventType != CollisionEvent.Stay) return;
            if (!collision.gameObject.HasHypertags(tags)) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }


        private void OnCollisionStay2D(Collision2D collision)
        {
            if (isTrigger) return;
            if (!isTriggerEnabled) return;
            if (eventType != CollisionEvent.Stay) return;
            if (!collision.gameObject.HasHypertags(tags)) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isTrigger) return;
            if (!isTriggerEnabled) return;
            if (eventType != CollisionEvent.Exit) return;
            if (!collision.gameObject.HasHypertags(tags)) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }


        private void OnCollisionExit2D(Collision2D collision)
        {
            if (isTrigger) return;
            if (!isTriggerEnabled) return;
            if (eventType != CollisionEvent.Exit) return;
            if (!collision.gameObject.HasHypertags(tags)) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }
    }
}