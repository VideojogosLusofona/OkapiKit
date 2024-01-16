using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Movement")]
    public class ActionChangeMovement : Action
    {
        public enum ChangeType
        {
            Velocity = 0,
            GravityScale = 1,
            MaxJumpCount = 2,
            JumpHoldTime = 3,
            GlideMaxTime = 4
        };

        public enum VelocityOperation { Set = 0, PercentageModify = 1, AbsoluteModify = 2 };
        public enum FloatPlatformerPropertyOperation { Set = 0, PercentageModify = 1 };
        public enum Axis { AbsoluteRight = 0, AbsoluteLeft = 1, AbsoluteUp = 2, AbsoluteDown = 3, RelativeRight = 4, RelativeLeft = 5, RelativeUp = 6, RelativeDown = 7, Current = 8, InverseCurrent = 9 };

        [SerializeField]
        private Movement movementComponent;
        [SerializeField]
        private Rigidbody2D rigidBodyComponent;

        [SerializeField]
        private ChangeType changeType = ChangeType.Velocity;
        [SerializeField]
        private VelocityOperation operation = VelocityOperation.Set;
        [SerializeField]
        private FloatPlatformerPropertyOperation floatPlatformerOperation = FloatPlatformerPropertyOperation.Set;
        [SerializeField]
        private Vector2 percentageValue = new Vector2(1.0f, 1.0f);
        [SerializeField]
        private Vector2 value = new Vector2(1.0f, 1.0f);
        [SerializeField]
        private Axis axis;
        [SerializeField]
        private bool useRotation = false;
        [SerializeField]
        private bool useRandom = false;
        [SerializeField]
        private float startAngle = 0.0f;
        [SerializeField]
        private float endAngle = 360.0f;
        [SerializeField]
        private Vector2 speedRange = new Vector2(100, 100);
        [SerializeField]
        private Vector2 minVelocity = new Vector2(100, 100);
        [SerializeField]
        private Vector2 maxVelocity = new Vector2(100, 100);
        [SerializeField]
        private bool clampSpeed;
        [SerializeField]
        private Vector2 clampTo;
        [SerializeField]
        private float fValue;
        [SerializeField]
        private int iValue;

        public override string GetActionTitle() { return "Change Movement"; }

        private (Movement, Rigidbody2D) GetTarget()
        {
            Movement movement = null;
            Rigidbody2D rb = null;

            if ((movementComponent != null) && (movementComponent.IsLinear()))
            {
                movement = movementComponent;
            }
            else if (rigidBodyComponent != null)
            {
                rb = rigidBodyComponent;
            }
            else
            {
                movement = GetComponent<Movement>();
                if ((movement == null) || (!movement.IsLinear()))
                {
                    movement = null;
                    rb = GetComponent<Rigidbody2D>();
                }
            }

            return (movement, rb);
        }

        private string GetTargetName()
        {
            Movement movement = null;
            Rigidbody2D rb = null;

            (movement, rb) = GetTarget();
            if (movement != null) return $"{movement.name}'s movement";
            else if (rb != null) return $"{rb.name}'s rigid body";

            return "[UNKNOWN]";
        }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetName = GetTargetName();

            if (changeType == ChangeType.Velocity)
            {
                if (operation == VelocityOperation.Set)
                {
                    if (useRandom)
                    {
                        if (speedRange.x == speedRange.y)
                        {
                            desc += $"select a random angle between {startAngle} and {endAngle} and set the velocity of {targetName} towards that direction, with a magnitude of {speedRange.x}";
                        }
                        else
                        {
                            desc += $"select a random angle between {startAngle} and {endAngle} and set the velocity of {targetName} towards that direction, with a magnitude between {speedRange.x} and {speedRange.y}";
                        }
                        if (useRotation) desc += "; angles are relative to the object rotation";
                        return desc;
                    }
                    desc += $"select a random velocity between {minVelocity} and {maxVelocity} and set it to {targetName}";
                }
                else if (operation == VelocityOperation.PercentageModify)
                {
                    if (percentageValue.x == percentageValue.y)
                    {
                        desc += $"changes the current velocity of {targetName} by {percentageValue.x * 100}%";
                    }
                    else
                    {
                        desc += $"changes the current velocity of {targetName} by a percentage in the [{percentageValue.x * 100},{percentageValue.y * 100}] range";
                    }
                }
                else if (operation == VelocityOperation.AbsoluteModify)
                {
                    if (value.x == value.y)
                    {
                        desc += $"changes the current velocity of {targetName} by {value.x}";
                    }
                    else
                    {
                        desc += $"changes the current velocity of {targetName} by a value between {value.x} and {value.y}";
                    }
                    switch (axis)
                    {
                        case Axis.AbsoluteRight:
                            desc += " along the global X axis";
                            break;
                        case Axis.AbsoluteLeft:
                            desc += " along the global negative X axis";
                            break;
                        case Axis.AbsoluteUp:
                            desc += " along the global Y axis";
                            break;
                        case Axis.AbsoluteDown:
                            desc += " along the global negative Y axis";
                            break;
                        case Axis.RelativeRight:
                            desc += " along the object's X axis";
                            break;
                        case Axis.RelativeLeft:
                            desc += " along the object's negative X axis";
                            break;
                        case Axis.RelativeUp:
                            desc += " along the object's Y axis";
                            break;
                        case Axis.RelativeDown:
                            desc += " along the object's negative Y axis";
                            break;
                        case Axis.Current:
                            desc += " along the object's current velocity axis";
                            break;
                        case Axis.InverseCurrent:
                            desc += " along the object's negative velocity axis";
                            break;
                        default:
                            break;
                    }
                }
                if (clampSpeed)
                {
                    desc += $", and clamps the speed to be between [{clampTo.x},{clampTo.y}].";
                }
                else
                {
                    desc += ".";
                }
            }
            else if ((changeType == ChangeType.GravityScale) || (changeType == ChangeType.JumpHoldTime) || (changeType == ChangeType.GlideMaxTime))
            {
                string propName = "[UNKNOWN]";
                switch (changeType)
                {
                    case ChangeType.GravityScale: propName = "gravity scale"; break;
                    case ChangeType.JumpHoldTime: propName = "jump hold time"; break;
                    case ChangeType.GlideMaxTime: propName = "glide maximum time"; break;
                    default:
                        break;
                }
                if (floatPlatformerOperation == FloatPlatformerPropertyOperation.Set)
                {
                    if (percentageValue.x != percentageValue.y)
                        desc += $"select a random {propName} between {percentageValue.x} and {percentageValue.y} and set it to {targetName}";
                    else
                        desc += $"sets the {propName} to {percentageValue.x} on {targetName}";
                }
                else if (floatPlatformerOperation == FloatPlatformerPropertyOperation.PercentageModify)
                {
                    if (percentageValue.x == percentageValue.y)
                    {
                        desc += $"changes the {propName} of {targetName} by {percentageValue.x * 100}%";
                    }
                    else
                    {
                        desc += $"changes the {propName} of {targetName} by a percentage in the [{percentageValue.x * 100}%,{percentageValue.y * 100}%] range";
                    }
                }
            }
            else if (changeType == ChangeType.MaxJumpCount)
            {
                desc += $"sets the maximum jump count of {targetName} to {iValue}";
            }
            else
            {
                desc += $"UNDEFINED CHANGE TYPE ${changeType}";
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            Movement movement = null;
            Rigidbody2D rb = null;

            (movement, rb) = GetTarget();

            if ((movement == null) && (rb == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No target for movement change\n  - place a Movement or RigidBody2D component on this object or\n  - assign a Movement or RigidBody2D target", "If you want to change the movement properties, you need to define either a Movement or Rigidbody2D component, or have one on this object."));
            }
            else
            {
                if ((movement != null) && (movementComponent == null))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Target is movement on this object, but it's better to set it explicitly", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                }
                else if ((rb != null) && (rigidBodyComponent == null))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Target is RigidBody2D on this object, but it's better to set it explicitly", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                }
            }
            if (changeType == ChangeType.Velocity)
            {
                if ((movementComponent != null) && (!movementComponent.IsLinear()))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Target movement is not linear, velocity can't be changed!", "Velocity can only be changed in an object that's moving, not rotating. Use as target a linear movement component."));
                }
                else if ((GetComponent<Movement>() != null) && (movement == null))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Target movement is not linear, velocity can't be changed!", "Velocity can only be changed in an object that's moving, not rotating. Use as target a linear movement component."));
                }
            }
            if (movement == null)
            {
                if ((changeType == ChangeType.MaxJumpCount) ||
                    (changeType == ChangeType.JumpHoldTime) ||
                    (changeType == ChangeType.GlideMaxTime) ||
                    (changeType == ChangeType.GravityScale))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, $"{changeType} can only be used with a Platformer Movement target!", $"{changeType} is a property of a MovementPlatformer component, so you need to define one as a target!"));
                }
            }
            else
            {
                if (((changeType == ChangeType.MaxJumpCount) ||
                     (changeType == ChangeType.JumpHoldTime) ||
                     (changeType == ChangeType.GlideMaxTime) ||
                     (changeType == ChangeType.GravityScale)) &&
                    (movement as MovementPlatformer == null))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, $"{changeType} can only be used with a Platformer Movement target!", $"{changeType} is a property of a MovementPlatformer component, so you can only use this with a Platfomer Movement component"));
                }
            }
            if (rb == null)
            {
                
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if (changeType == ChangeType.Velocity)
            {
                Movement movement;
                Rigidbody2D rb;

                (movement, rb) = GetTarget();

                Vector2 velocity = Vector2.zero;

                if (operation == VelocityOperation.Set)
                {
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
                }
                else if (operation == VelocityOperation.PercentageModify)
                {
                    if ((movement) && (movement.IsLinear()))
                    {
                        velocity = movement.GetSpeed();
                    }
                    else if (rb)
                    {
                        velocity = rb.velocity;
                    }

                    float r = Random.Range(percentageValue.x, percentageValue.y);

                    velocity = velocity + velocity * r;
                }
                else if (operation == VelocityOperation.AbsoluteModify)
                {
                    if ((movement) && (movement.IsLinear()))
                    {
                        velocity = movement.GetSpeed();
                    }
                    else if (rb)
                    {
                        velocity = rb.velocity;
                    }

                    float r = Random.Range(value.x, value.y);

                    switch (axis)
                    {
                        case Axis.AbsoluteRight:
                            velocity = velocity + Vector2.right * r;
                            break;
                        case Axis.AbsoluteLeft:
                            velocity = velocity + Vector2.left * r;
                            break;
                        case Axis.AbsoluteUp:
                            velocity = velocity + Vector2.up * r;
                            break;
                        case Axis.AbsoluteDown:
                            velocity = velocity + Vector2.down * r;
                            break;
                        case Axis.RelativeRight:
                            velocity = velocity + new Vector2(transform.right.x * r, transform.right.y * r);
                            break;
                        case Axis.RelativeLeft:
                            velocity = velocity - new Vector2(transform.right.x * r, transform.right.y * r);
                            break;
                        case Axis.RelativeUp:
                            velocity = velocity + new Vector2(transform.up.x * r, transform.up.y * r);
                            break;
                        case Axis.RelativeDown:
                            velocity = velocity - new Vector2(transform.up.x * r, transform.up.y * r);
                            break;
                        case Axis.Current:
                            velocity = velocity + velocity.normalized * r;
                            break;
                        case Axis.InverseCurrent:
                            velocity = velocity - velocity.normalized * r;
                            break;
                        default:
                            break;
                    }
                }
                if (clampSpeed)
                {
                    float n = velocity.magnitude;
                    if (n > 0)
                    {
                        n = Mathf.Clamp(n, clampTo.x, clampTo.y);

                        velocity = n * velocity.normalized;
                    }
                }

                if ((movement) && (movement.IsLinear()))
                {
                    movement.SetSpeed(velocity);
                }
                else if (rb)
                {
                    rb.velocity = velocity;
                }
            }
            else if (changeType == ChangeType.GravityScale)
            {
                Rigidbody2D rb;
                Movement movement;

                (movement, rb) = GetTarget();
                if (movement == null) return;
                MovementPlatformer platMovement = movement as MovementPlatformer;
                if (platMovement != null) return;

                if (floatPlatformerOperation == FloatPlatformerPropertyOperation.Set)
                {
                    var value = Random.Range(percentageValue.x, percentageValue.y);
                    platMovement.SetGravityScale(value);
                }
                else if (floatPlatformerOperation == FloatPlatformerPropertyOperation.PercentageModify)
                {
                    float r = Random.Range(percentageValue.x, percentageValue.y);
                    platMovement.SetGravityScale(r * platMovement.GetGravityScale());
                }
            }
            else if (changeType == ChangeType.MaxJumpCount)
            {
                Rigidbody2D rb;
                Movement movement;

                (movement, rb) = GetTarget();
                if (movement == null) return;
                MovementPlatformer platMovement = movement as MovementPlatformer;
                if (platMovement != null) return;

                platMovement.SetMaxJumpCount(iValue);
            }
            else if (changeType == ChangeType.JumpHoldTime)
            {
                Rigidbody2D rb;
                Movement movement;

                (movement, rb) = GetTarget();
                if (movement == null) return;
                MovementPlatformer platMovement = movement as MovementPlatformer;
                if (platMovement != null) return;

                if (floatPlatformerOperation == FloatPlatformerPropertyOperation.Set)
                {
                    var value = Random.Range(percentageValue.x, percentageValue.y);
                    platMovement.SetJumpHoldTime(value);
                }
                else if (floatPlatformerOperation == FloatPlatformerPropertyOperation.PercentageModify)
                {
                    float r = Random.Range(percentageValue.x, percentageValue.y);
                    platMovement.SetJumpHoldTime(r * platMovement.GetJumpHoldTime());
                }
            }
            else if (changeType == ChangeType.GlideMaxTime)
            {
                Rigidbody2D rb;
                Movement movement;

                (movement, rb) = GetTarget();
                if (movement == null) return;
                MovementPlatformer platMovement = movement as MovementPlatformer;
                if (platMovement != null) return;

                if (floatPlatformerOperation == FloatPlatformerPropertyOperation.Set)
                {
                    var value = Random.Range(percentageValue.x, percentageValue.y);
                    platMovement.SetGlideMaxTime(value);
                }
                else if (floatPlatformerOperation == FloatPlatformerPropertyOperation.PercentageModify)
                {
                    float r = Random.Range(percentageValue.x, percentageValue.y);
                    platMovement.SetGlideMaxTime(r * platMovement.GetGlideMaxTime());
                }
            }
        }
    }
}