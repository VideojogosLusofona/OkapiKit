using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{

    public class ActionModifyTransform : Action
    {
        public enum ChangeType { Position = 0 };

        public enum AxisChange { None = 0, Change = 1, Set = 2 };

        [SerializeField]
        private Transform target;

        [SerializeField]
        private ChangeType changeType = ChangeType.Position;
        [SerializeField, ShowIf("isPositionChange")]
        private AxisChange xAxis = AxisChange.None;
        [SerializeField, ShowIf("isSetX")]
        private Vector2 positionX;
        [SerializeField, ShowIf("isChangeX")]
        private Vector2 deltaX;
        [SerializeField, ShowIf("isPositionChange")]
        private AxisChange yAxis = AxisChange.None;
        [SerializeField, ShowIf("isSetY")]
        private Vector2 positionY;
        [SerializeField, ShowIf("isChangeY")]
        private Vector2 deltaY;

        private bool isPositionChange => changeType == ChangeType.Position;
        private bool isSetX => isPositionChange && xAxis == AxisChange.Set;
        private bool isChangeX => isPositionChange && xAxis == AxisChange.Change;
        private bool isSetY => isPositionChange && yAxis == AxisChange.Set;
        private bool isChangeY => isPositionChange && yAxis == AxisChange.Change;


        private Transform GetTarget()
        {
            return (target != null) ? (target) : (transform);
        }

        private string GetTargetName()
        {
            return (target != null) ? $"object {target.name}" : "this";
        }

        private string GetAxisDesc(string targetName, string axisName, AxisChange type, Vector2 pos, Vector2 delta)
        {
            string desc = "";
            if (type == AxisChange.Set)
            {
                desc += $"set the {axisName} position of {targetName} to ";
                if (pos.x == pos.y) desc += $"{pos.x}";
                else desc += $"a random value between {pos.x} and {pos.y}";
            }
            else if (type == AxisChange.Change)
            {
                desc += $"changes the {axisName} position of {targetName}, adding ";
                if (delta.x == delta.y) desc += $"{delta.x}";
                else desc += $"a random value between {delta.x} and {delta.y}";
            }

            return desc;
        }

        public override string GetActionTitle() => "Modify Transform";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetName = GetTargetName();

            if (changeType == ChangeType.Position)
            {
                string xDesc = GetAxisDesc(targetName, "X", xAxis, positionX, deltaX);
                string yDesc = GetAxisDesc(targetName, "Y", yAxis, positionY, deltaY);

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

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if (changeType == ChangeType.Position)
            {
                var t = GetTarget();
                Vector2 currentPos = t.position;

                switch (xAxis)
                {
                    case AxisChange.None:
                        break;
                    case AxisChange.Change:
                        currentPos.x += Random.Range(deltaX.x, deltaX.y);
                        break;
                    case AxisChange.Set:
                        currentPos.x = Random.Range(positionX.x, positionX.y);
                        break;
                    default:
                        break;
                }

                switch (yAxis)
                {
                    case AxisChange.None:
                        break;
                    case AxisChange.Change:
                        currentPos.y += Random.Range(deltaY.x, deltaY.y);
                        break;
                    case AxisChange.Set:
                        currentPos.y = Random.Range(positionY.x, positionY.y);
                        break;
                    default:
                        break;
                }

                t.position = currentPos;
            }
        }
    }
}