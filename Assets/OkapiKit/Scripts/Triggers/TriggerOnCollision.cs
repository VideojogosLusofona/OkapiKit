using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnCollision : Trigger
{
    [SerializeField] private enum CollisionEvent { Enter, Stay, Exit };

    [SerializeField]
    private bool isTrigger = true;
    [SerializeField]
    private CollisionEvent eventType;
    [SerializeField]
    private Hypertag[] tags;

    protected override string GetRawDescription()
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
                desc += tags[i].name;
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
        if (eventType != CollisionEvent.Enter) return;
        if (!EvaluatePreconditions()) return;


        if (collision.gameObject.HasHypertags(tags))
        {
            ExecuteTrigger();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTrigger) return;
        if (eventType != CollisionEvent.Enter) return;
        if (!EvaluatePreconditions()) return;

        if (collision.gameObject.HasHypertags(tags))
        {
            ExecuteTrigger();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isTrigger) return;
        if (eventType != CollisionEvent.Stay) return;
        if (!EvaluatePreconditions()) return;

        if (collision.gameObject.HasHypertags(tags))
        {
            ExecuteTrigger();
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isTrigger) return;
        if (eventType != CollisionEvent.Stay) return;
        if (!EvaluatePreconditions()) return;

        if (collision.gameObject.HasHypertags(tags))
        {
            ExecuteTrigger();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isTrigger) return;
        if (eventType != CollisionEvent.Exit) return;
        if (!EvaluatePreconditions()) return;

        if (collision.gameObject.HasHypertags(tags))
        {
            ExecuteTrigger();
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isTrigger) return;
        if (eventType != CollisionEvent.Exit) return;
        if (!EvaluatePreconditions()) return;

        if (collision.gameObject.HasHypertags(tags))
        {
            ExecuteTrigger();
        }
    }
}
