using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{

    [AddComponentMenu("Okapi/Action/Change Transform")]
    public class ActionChangeTransform : Action
    {
        public enum ChangeType { Position = 0, Scale = 2 };

        public enum AxisChange { None = 0, AddSub = 1, Set = 2, Multiply = 3, Divide = 4 };

        [SerializeField]
        private Transform target;

        [SerializeField]
        private ChangeType changeType = ChangeType.Position;
        [SerializeField]
        private AxisChange xAxis = AxisChange.None;
        [SerializeField]
        private Vector2 positionX;
        [SerializeField]
        private Vector2 deltaX;
        [SerializeField]
        private AxisChange yAxis = AxisChange.None;
        [SerializeField]
        private Vector2 positionY;
        [SerializeField]
        private Vector2 deltaY;
        [SerializeField]
        private bool    scaleWithTime = false;

        private Transform GetTarget()
        {
            return (target != null) ? (target) : (transform);
        }

        private string GetTargetName()
        {
            return (target != null) ? $"object {target.name}" : "this";
        }

        private string GetAxisDesc(string changeName, string targetName, string axisName, AxisChange type, Vector2 pos, Vector2 delta)
        {
            string desc = "";
            if (type == AxisChange.Set)
            {
                desc += $"set the {axisName} {changeName} of {targetName} to ";
                if (pos.x == pos.y) desc += $"{pos.x}";
                else desc += $"a random value between {pos.x} and {pos.y}";
            }
            else if (type == AxisChange.AddSub)
            {
                desc += $"changes the {axisName} {changeName} of {targetName}, adding ";
                if (delta.x == delta.y) desc += $"{delta.x}";
                else desc += $"a random value between {delta.x} and {delta.y}";
                if (scaleWithTime) desc += ", scaled with time.";
            }
            else if (type == AxisChange.Multiply)
            {
                desc += $"changes the {axisName} {changeName} of {targetName}, multiplying by ";
                if (delta.x == delta.y) desc += $"{delta.x}";
                else desc += $"a random value between {delta.x} and {delta.y}";
                if (scaleWithTime) desc += ", scaled with time.";
            }
            else if (type == AxisChange.Divide)
            {
                desc += $"changes the {axisName} {changeName} of {targetName}, dividing by ";
                if (delta.x == delta.y) desc += $"{delta.x}";
                else desc += $"a random value between {delta.x} and {delta.y}";
                if (scaleWithTime) desc += ", scaled with time.";
            }

            return desc;
        }

        public override string GetActionTitle() => "Change Transform";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetName = GetTargetName();

            if (changeType == ChangeType.Position)
            {
                string xDesc = GetAxisDesc("position", targetName, "X", xAxis, positionX, deltaX);
                string yDesc = GetAxisDesc("position", targetName, "Y", yAxis, positionY, deltaY);

                if (xDesc != "")
                {
                    desc += $"{xDesc}";
                    if (yDesc != "") desc += $", and {yDesc}.";
                    else desc += ".";
                }
                else if (yDesc != "")
                {
                    desc += $"{yDesc}.";
                }
                else
                {
                    desc += "[No transform operation]";
                }
            }
            else if (changeType == ChangeType.Scale)
            {
                string xDesc = GetAxisDesc("scale", targetName, "X", xAxis, positionX, deltaX);
                string yDesc = GetAxisDesc("scale", targetName, "Y", yAxis, positionY, deltaY);

                if (xDesc != "")
                {
                    desc += $"{xDesc}";
                    if (yDesc != "") desc += $", and {yDesc}.";
                    else desc += ".";
                }
                else if (yDesc != "")
                {
                    desc += $"{yDesc}.";
                }
                else
                {
                    desc += "[No transform operation]";
                }
            }
            else
            {
                desc += $"UNDEFINED CHANGE TYPE ${changeType}";
            }

            return desc;
        }

        protected override void CheckErrors(int level)
        {
            base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (target == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Transform to modify is this object, but it should be explicitly linked!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if ((changeType == ChangeType.Position) || (changeType == ChangeType.Scale))
            {
                var t = GetTarget();
                Vector2 current = Vector2.zero;

                if (changeType == ChangeType.Position) current = t.position;
                else if (changeType == ChangeType.Scale) current = t.localScale;

                    switch (xAxis)
                {
                    case AxisChange.None:
                        break;
                    case AxisChange.AddSub:
                        current.x += Random.Range(deltaX.x, deltaX.y) * ((scaleWithTime)?(Time.deltaTime):(1.0f));
                        break;
                    case AxisChange.Multiply:
                        current.x *= Random.Range(deltaX.x, deltaX.y) * ((scaleWithTime) ? (Time.deltaTime) : (1.0f));
                        break;
                    case AxisChange.Divide:
                        current.x /= Random.Range(deltaX.x, deltaX.y) * ((scaleWithTime) ? (Time.deltaTime) : (1.0f));
                        break;
                    case AxisChange.Set:
                        current.x = Random.Range(positionX.x, positionX.y);
                        break;
                    default:
                        break;
                }

                switch (yAxis)
                {
                    case AxisChange.None:
                        break;
                    case AxisChange.AddSub:
                        current.y += Random.Range(deltaY.x, deltaY.y) * ((scaleWithTime) ? (Time.deltaTime) : (1.0f));
                        break;
                    case AxisChange.Multiply:
                        current.y *= Random.Range(deltaY.x, deltaY.y) * ((scaleWithTime) ? (Time.deltaTime) : (1.0f));
                        break;
                    case AxisChange.Divide:
                        current.y /= Random.Range(deltaY.x, deltaY.y) * ((scaleWithTime) ? (Time.deltaTime) : (1.0f));
                        break;
                    case AxisChange.Set:
                        current.y = Random.Range(positionY.x, positionY.y);
                        break;
                    default:
                        break;
                }

                if (changeType == ChangeType.Position)
                    t.position = current;
                else if (changeType == ChangeType.Scale)
                    t.localScale = current;
            }
        }
    }
}