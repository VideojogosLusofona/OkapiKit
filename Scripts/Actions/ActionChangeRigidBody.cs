using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using Codice.Client.BaseCommands;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Rigid Body")]
    public class ActionChangeRigidBody : Action
    {
        public enum ChangeType { SetBodyType = 0, Mass = 1, LinearDrag = 2, AngularDrag = 3, GravityScale = 4, VelocityChange = 5, VelocitySet = 6 };
        public enum Axis { AbsoluteRight = 0, AbsoluteLeft = 1, AbsoluteUp = 2, AbsoluteDown = 3, RelativeRight = 4, RelativeLeft = 5, RelativeUp = 6, RelativeDown = 7, Current = 8, InverseCurrent = 9, Custom = 10 };

        [SerializeField]
        private Rigidbody2D     target;
        [SerializeField]
        private ChangeType      changeType;
        [SerializeField]
        private RigidbodyType2D bodyType;
        [SerializeField]
        private float           value;
        [SerializeField]
        private bool            timeScaled;
        [SerializeField]
        private bool            random;
        [SerializeField]
        private Axis            axis = Axis.AbsoluteUp;
        [SerializeField]
        private Vector2         angleMinMax = new Vector2(0.0f, 360.0f);
        [SerializeField]
        private Vector2         speedMinMax = new Vector2(0.0f, 100.0f);
        [SerializeField]
        private bool            clampSpeed;
        [SerializeField]
        private Vector2         clampTo = new Vector2(0.0f, 5.0f);

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var rb = GetRB();

            switch (changeType)
            {
                case ChangeType.SetBodyType:
                    rb.bodyType = bodyType;
                    break;
                case ChangeType.Mass:
                    rb.mass = value;
                    break;
                case ChangeType.LinearDrag:
                    rb.linearDamping = value;
                    break;
                case ChangeType.AngularDrag:
                    rb.angularDamping = value;
                    break;
                case ChangeType.GravityScale:
                    rb.gravityScale = value;
                    break;
                case ChangeType.VelocitySet:
                    {
                        var dir = GetAngle();
                        var perp = new Vector2(-dir.y, dir.x);
                        var speed = Random.Range(speedMinMax.x, speedMinMax.y);
                        rb.linearVelocity = dir * speed + perp * Vector2.Dot(perp, rb.linearVelocity);
                        if (clampSpeed)
                        {
                            rb.linearVelocity = Mathf.Clamp(rb.linearVelocity.magnitude, clampTo.x, clampTo.y) * rb.linearVelocity.normalized;
                        }
                    }
                    break;
                case ChangeType.VelocityChange:
                    {
                        var dir = GetAngle();
                        var perp = new Vector2(-dir.y, dir.x);
                        var speed = Random.Range(speedMinMax.x, speedMinMax.y);
                        rb.linearVelocity = rb.linearVelocity + dir * speed * ((timeScaled) ? (Time.deltaTime) : (1.0f));
                        if (clampSpeed)
                        {
                            rb.linearVelocity = Mathf.Clamp(rb.linearVelocity.magnitude, clampTo.x, clampTo.y) * rb.linearVelocity.normalized;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Change Rigid Body";
        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            var desc = GetPreconditionsString(gameObject);
            var targetName = (target != null) ? (target.name) : ("this object");
            switch (changeType)
            {
                case ChangeType.SetBodyType:
                    desc += $"changes body type of {targetName} to {bodyType}";
                    break;
                case ChangeType.Mass:
                    desc += $"changes mass of {targetName} to {value}";
                    break;
                case ChangeType.LinearDrag:
                    desc += $"changes linear drag of {targetName} to {value}";
                    break;
                case ChangeType.AngularDrag:
                    desc += $"changes angular drag of {targetName} to {value}";
                    break;
                case ChangeType.GravityScale:
                    desc += $"changes gravity scale of {targetName} to {value}";
                    break;
                case ChangeType.VelocityChange:
                case ChangeType.VelocitySet:
                    string axisName = "";
                    switch (axis)
                    {
                        case Axis.AbsoluteRight: axisName = "right"; break;
                        case Axis.AbsoluteLeft: axisName = "left"; break;
                        case Axis.AbsoluteUp: axisName = "up"; break;
                        case Axis.AbsoluteDown: axisName = "down"; break;
                        case Axis.RelativeRight: axisName = "relative right"; break;
                        case Axis.RelativeLeft: axisName = "relative left"; break;
                        case Axis.RelativeUp: axisName = "relative up"; break;
                        case Axis.RelativeDown: axisName = "relative down"; break;
                        case Axis.Current: axisName = "current"; break;
                        case Axis.InverseCurrent: axisName = "inverse current"; break;
                        case Axis.Custom: axisName = "custom"; break;
                        default: break;
                    }
                    if (random)
                    {
                        if (changeType == ChangeType.VelocityChange)
                            desc += $"Changes current velocity of {targetName} to a random one.\n";
                        else
                            desc += $"Sets the current velocity of {targetName} to a random one.\n";
                        if (axis == Axis.Custom)
                        {
                            if (angleMinMax.x == angleMinMax.y)
                                desc += $"That velocity will have a direction with the angle of {angleMinMax.x} degrees, ";
                            else
                                desc += $"That velocity will have a random direction with an angle between {angleMinMax.x} and {angleMinMax.y}, ";
                        }
                        else
                        {
                            desc += $"That velocity will have a {axisName} direction, ";
                        }

                        if (speedMinMax.x == speedMinMax.y)
                            desc += $"and a speed of {speedMinMax.x} units/sec.\n";
                        else
                            desc += $"and a speed between {speedMinMax.x} and {speedMinMax.y} units/sec.\n";
                    }   
                    else
                    {
                        if (changeType == ChangeType.VelocityChange)
                            desc += $"Changes ";
                        else
                            desc += $"Sets ";

                        desc += $"the current velocity of {targetName} to ";
                        
                        if (speedMinMax.x == speedMinMax.y)
                            desc += $"a speed of {speedMinMax.x} units/sec, ";
                        else
                            desc += $"a speed between {speedMinMax.x} and {speedMinMax.y} units/sec, ";

                        if (axis == Axis.Custom)
                        {
                            if (angleMinMax.x == angleMinMax.y)
                                desc += $"at an angle of {angleMinMax.x} degrees.\n";
                            else
                                desc += $"at an angle between {angleMinMax.x} and {angleMinMax.y} degrees.\n";
                        }
                        else
                        {
                            desc += $"in the {axisName} direction.";
                        }
                    }

                    if ((changeType == ChangeType.VelocityChange) && (timeScaled))
                    {
                        desc += $"\nThe velocity change will be modulated with the elapsed time, making it suitable for accelerating/deccelerating with input, for example.\n";
                    }

                    if (clampSpeed)
                    {
                        desc += $"The speed will be clamped between {clampTo.x} and {clampTo.y}.\n";
                    }
                    break;
                default:
                    desc += $"UNKNOWN CHANGE!";
                    if (clampSpeed)
                    {
                        desc += $", and clamps the speed to be between [{clampTo.x},{clampTo.y}].\n";
                    }
                    else
                    {
                        desc += ".\n";
                    }

                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (target == null)
            {
                if (GetComponent<Rigidbody2D>() == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined target rigid body", "We're changing something on a rigid body, so we need a target so we know which rigid bodyto change."));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Rigid body to modify is on this object, but it should be explicitly linked!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                }
            }
        }

        Vector2 GetAngle()
        {
            switch (axis)
            {
                case Axis.AbsoluteRight: return Vector2.right;
                case Axis.AbsoluteLeft: return Vector2.left;
                case Axis.AbsoluteUp: return Vector2.up;
                case Axis.AbsoluteDown: return Vector2.down;
                case Axis.RelativeRight: return transform.right;
                case Axis.RelativeLeft: return -transform.right;
                case Axis.RelativeUp: return transform.up;
                case Axis.RelativeDown: return -transform.up;
                case Axis.Current: { var rb = GetRB(); if (rb) return rb.linearVelocity.normalized; else return Vector2.zero; }
                case Axis.InverseCurrent: { var rb = GetRB(); if (rb) return -rb.linearVelocity.normalized; else return Vector2.zero; }
                case Axis.Custom: { var ang = Random.Range(angleMinMax.x, angleMinMax.y) * Mathf.Deg2Rad; return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)); }
                default:
                    break;
            }

            return Vector2.zero;
        }

        Rigidbody2D GetRB()
        {
            if (target) return target;

            target = GetComponent<Rigidbody2D>();

            return target;
        }
    }
}
