using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Movement : MonoBehaviour
{
    protected Rigidbody2D rb;

    abstract public Vector2 GetSpeed();
    abstract public void SetSpeed(Vector2 speed);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

    }

    protected void RotateZ(float angle)
    {
        if (rb != null)
        {
            rb.MoveRotation(rb.rotation + angle * Mathf.Deg2Rad);
        }
        else
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, angle);
        }

    }
}
