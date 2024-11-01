using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Movement/Forward Grid Movement")]
    public class MovementGridForward : MovementGrid
    {
        public enum Axis { Up = 0, Down = 1, Right = 2, Left = 3 };

        [SerializeField] private Axis axis = Axis.Up;
        [SerializeField] private float speed = 200.0f;

        public override Vector2 GetSpeed() => new Vector2(speed, speed);
        public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

        override public string GetTitle() => "Forward Grid Movement";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            float gridSpeed = speed / gridSize.y;
            string desc = "";
            if (speed != 0.0f)
            {
                desc += $"Single axis grid movement towards the {axis} axis of the object, at {speed} units per second ({gridSpeed} cells/sec).\n";
            }
            else
            {
                desc += $"Single axis grid movement towards the {axis} axis of the object, although {speed} is set to zero!\n";
            }
            if (cooldown > 0.0f)
            {
                desc += $"The movement steps can only happen once every {cooldown} seconds.\n";
            }
            if (pushStrength > 0)
            {
                desc += $"This object can push objects as long as their mass combined doesn't exceed {pushStrength}.\n";
            }
            else
            {
                desc += $"This object can not push objects.\n";
            }

            return desc;
        }

        void FixedUpdate()
        {
            if (!isMovementActive()) return;

            if (gridObject.isMoving)
            {
            }
            else if (moveCooldownTimer <= 0.0f)
            {
                Vector2 moveDir = Vector2.zero;
                switch (axis)
                {
                    case Axis.Up: moveDir = transform.up; break;
                    case Axis.Down: moveDir = -transform.up; break;
                    case Axis.Right: moveDir = transform.right; break;
                    case Axis.Left: moveDir = -transform.right; break;

                    default:
                        break;
                }

                // Get grid direction from vector
                MoveInDirection(moveDir, true);
            }
        }
    }
}