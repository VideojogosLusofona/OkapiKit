using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    abstract public class Movement : OkapiElement
    {
        protected Rigidbody2D rb;
        protected Vector3 lastDelta;

        abstract public Vector2 GetSpeed();
        abstract public void SetSpeed(Vector2 speed);

        virtual public bool IsLinear() { return true; }

        abstract public string GetTitle();

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            UpdateExplanation();
        }

        protected void MoveDelta(Vector3 delta)
        {
            if (rb != null)
            {
                rb.MovePosition(rb.position + new Vector2(delta.x, delta.y));
            }
            else
            {
                transform.position = transform.position + delta;
            }
            lastDelta = delta;
        }

        protected void RotateZ(float angle)
        {
            if ((rb != null) && (!rb.freezeRotation))
            {
                rb.MoveRotation(rb.rotation + angle);
            }
            else
            {
                transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, angle);
            }
        }

        protected void RotateTo(Vector2 upDir, float maxAngle)
        {
            if ((rb != null) && (!rb.freezeRotation))
            {
                Quaternion target = Quaternion.LookRotation(Vector3.forward, upDir);

                Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, target, maxAngle);

                rb.MoveRotation(newRotation);
            }
            else
            {
                Quaternion target = Quaternion.LookRotation(Vector3.forward, upDir);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, maxAngle);
            }
        }

        override public string UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }
    }
}