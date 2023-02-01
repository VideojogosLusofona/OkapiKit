using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionModifyMovement : Action
{
    public enum ChangeType { Velocity = 0 };

    public enum VelocityOperation { Set = 0, PercentageModify = 1, AbsoluteModify = 2 };
    public enum Axis { AbsoluteRight = 0, AbsoluteLeft = 1, AbsoluteUp = 2, AbsoluteDown = 3, RelativeRight = 4, RelativeLeft = 5, RelativeUp = 6, RelativeDown = 7, Current = 8, InverseCurrent = 9 };

    [SerializeField, ShowIf("needMovementComponent")]
    private Movement            movementComponent;
    [SerializeField, ShowIf("needRigidBodyComponent")]
    private Rigidbody2D         rigidBodyComponent;

    [SerializeField]
    private ChangeType          changeType = ChangeType.Velocity;
    [SerializeField, ShowIf("isChangeVelocity")] 
    private VelocityOperation   operation = VelocityOperation.Set;
    [SerializeField, ShowIf("isPercentageModify")]
    private Vector2             percentageValue = new Vector2(1.0f, 1.0f);
    [SerializeField, ShowIf("isAbsoluteModify")]
    private Vector2             value = new Vector2(1.0f, 1.0f);
    [SerializeField, ShowIf("isAbsoluteModify")]
    private Axis                axis;
    [SerializeField, ShowIf("isSet")] 
    private bool                useRotation = false;
    [SerializeField, ShowIf("isSet")]
    private bool                useRandom = false;
    [SerializeField, ShowIf("isSet")]
    private float               startAngle = 0.0f;
    [SerializeField, ShowIf("isSet")]
    private float               endAngle = 360.0f;
    [SerializeField, ShowIf("isSet")]
    private Vector2             speedRange = new Vector2(100, 100);
    [SerializeField, ShowIf("isSetNotRandom")]
    private Vector2             minVelocity = new Vector2(100, 100);
    [SerializeField, ShowIf("isSetNotRandom")]
    private Vector2             maxVelocity = new Vector2(100, 100);
    [SerializeField, ShowIf("isModify")]
    private bool                clampSpeed;
    [SerializeField, ShowIf("hasClamp")]
    private Vector2             clampTo;

    private bool isChangeVelocity => (changeType == ChangeType.Velocity);
    private bool isSet => isChangeVelocity && (operation == VelocityOperation.Set) && (useRandom);
    private bool isSetNotRandom => isChangeVelocity && (operation == VelocityOperation.Set) && (!useRandom);
    private bool isPercentageModify => isChangeVelocity && (operation == VelocityOperation.PercentageModify);
    private bool isAbsoluteModify => isChangeVelocity && (operation == VelocityOperation.AbsoluteModify);
    private bool isModify => isChangeVelocity && ((operation == VelocityOperation.PercentageModify) || (operation == VelocityOperation.AbsoluteModify));
    private bool hasClamp => isModify && clampSpeed;

    private bool needMovementComponent => rigidBodyComponent == null;
    private bool needRigidBodyComponent => movementComponent == null;


    public override string GetActionTitle() { return "Modify Movement"; }

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
                    desc += $"changes the current velocity of {targetName} by {percentageValue.x*100}%";
                }
                else
                {
                    desc += $"changes the current velocity of {targetName} by a percentage in the [{percentageValue.x*100},{percentageValue.y*100}] range";
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
        else
        {
            desc += $"UNDEFINED CHANGE TYPE ${changeType}";
        }

        return desc;
    }

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        if (changeType == ChangeType.Velocity)
        {
            Movement movement = null;
            Rigidbody2D rb = null;

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
    }
}
