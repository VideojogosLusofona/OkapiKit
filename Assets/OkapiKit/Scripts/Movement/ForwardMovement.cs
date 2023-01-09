using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMovement : Movement
{
    [SerializeField] private float speed = 200.0f;

    public override Vector2 GetSpeed() => new Vector2(speed, speed);
    public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

    void FixedUpdate()
    {
        MoveDelta(speed * transform.up * Time.fixedDeltaTime);
    }
}
