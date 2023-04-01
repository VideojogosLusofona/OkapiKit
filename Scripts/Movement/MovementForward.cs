using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class MovementForward : Movement
    {
        public enum Axis { Up = 0, Right = 1 };

        [SerializeField] private Axis axis = Axis.Up;
        [SerializeField] private float speed = 200.0f;

        public override Vector2 GetSpeed() => new Vector2(speed, speed);
        public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

        override public string GetTitle() => "Forward Movement";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            if (speed != 0.0f)
            {
                desc += $"Single axis movement towards the {axis} axis of the object, at {speed} units per second.\n";
            }
            else
            {
                desc += $"Single axis movement towards the {axis} axis of the object, although {speed} is set to zero!\n";
            }

            return desc;
        }

        void FixedUpdate()
        {
            switch (axis)
            {
                case Axis.Up:
                    MoveDelta(speed * transform.up * Time.fixedDeltaTime);
                    break;
                case Axis.Right:
                    MoveDelta(speed * transform.right * Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }
        }
    }
}