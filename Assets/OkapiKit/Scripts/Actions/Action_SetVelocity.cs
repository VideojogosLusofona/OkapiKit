using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Action_SetVelocity : Action
{
    [SerializeField] 
    private bool    useRotation = false;
    [SerializeField] 
    private bool    useRandom = false;
    [SerializeField, ShowIf("useRandom")]
    private float   startAngle = 0.0f;
    [SerializeField, ShowIf("useRandom")]
    private float   endAngle = 360.0f;
    [SerializeField, ShowIf("useRandom")]
    private Vector2 speedRange = new Vector2(100, 100);
    [SerializeField, HideIf("useRandom")]
    private Vector2 minVelocity = new Vector2(100, 100);
    [SerializeField, HideIf("useRandom")]
    private Vector2 maxVelocity = new Vector2(100, 100);

    public override string GetRawDescription(string ident)
    {
        if (useRandom)
        {
            var desc = $"Select a random angle between {startAngle} and {endAngle} and set the velocity of this object towards that direction, with a magnitude between {speedRange.x} and {speedRange.y}";
            if (useRotation) desc += "; Angles are relative to the object rotation.";
            return desc;
        }
        return $"Select a random velocity between {minVelocity} and {maxVelocity} and set it to this object";
    }

    public override void Execute()
    {
        if (!enableAction) return;

        Vector2 velocity = Vector2.zero;

        if (useRandom)
        {
            float angle = Random.Range(Mathf.Min(startAngle, endAngle), Mathf.Max(startAngle, endAngle)) * Mathf.Deg2Rad;
            float speed = Random.Range(speedRange.x, speedRange.y);

            velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
        }
        else
        {
            velocity = new Vector2(Random.Range(minVelocity.x, maxVelocity.y), Random.Range(minVelocity.y, maxVelocity.y));
        }

        if (useRotation)
        {
            velocity = velocity.x * transform.right + velocity.y * transform.up;
        }

        XYMovement mxy = GetComponent<XYMovement>();
        if (mxy)
        {
            mxy.SetSpeed(velocity);
        }
        else
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = velocity;
        }
    }
}
