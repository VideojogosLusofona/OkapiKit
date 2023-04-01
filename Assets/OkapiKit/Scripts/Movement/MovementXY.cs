using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class MovementXY : Movement
    {
        public enum InputType { Axis = 0, Button = 1, Key = 2 };
        public enum Axis { UpAxis = 0, RightAxis = 1 };

        [SerializeField]
        private Vector2 speed = new Vector2(100, 100);
        [SerializeField]
        private bool limitSpeed = false;
        [SerializeField]
        private float speedLimit = 100.0f;
        [SerializeField]
        private bool useRotation = false;
        [SerializeField]
        private bool turnToDirection = false;
        [SerializeField]
        private Axis axisToAlign = Axis.UpAxis;
        [SerializeField]
        private float maxTurnSpeed = 360.0f;
        [SerializeField]
        private bool inputEnabled;
        [SerializeField]
        private InputType inputType;
        [SerializeField, InputAxis]
        private string horizontalAxis = "Horizontal";
        [SerializeField, InputAxis]
        private string verticalAxis = "Vertical";
        [SerializeField]
        private string horizontalButtonPositive = "Right";
        [SerializeField]
        private string horizontalButtonNegative = "Left";
        [SerializeField]
        private string verticalButtonPositive = "Up";
        [SerializeField]
        private string verticalButtonNegative = "Down";
        [SerializeField]
        private KeyCode horizontalKeyPositive = KeyCode.RightArrow;
        [SerializeField]
        private KeyCode horizontalKeyNegative = KeyCode.LeftArrow;
        [SerializeField]
        private KeyCode verticalKeyPositive = KeyCode.UpArrow;
        [SerializeField]
        private KeyCode verticalKeyNegative = KeyCode.DownArrow;
        [SerializeField]
        private bool inertiaEnable = false;
        [SerializeField]
        private float inertiaStopTime = 0.0f;

        Vector3 moveVector;
        Vector3 currentVelocity = Vector3.zero;

        public override Vector2 GetSpeed() => speed;
        public override void SetSpeed(Vector2 speed) { this.speed = speed; }

        override public string GetTitle() => "XY Movement";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            if (speed.x != 0.0f)
            {
                if (speed.y != 0.0f)
                {
                    desc += $"Dual axis movement, at {speed} units per second.\n";
                }
                else
                {
                    desc += $"Horizontal movement, at {speed.x} units per second.\n";
                }
            }
            else
            {
                if (speed.y != 0.0f)
                {
                    desc += $"Vertical movement, at {speed.y} units per second.\n";
                }
                else
                {
                    desc += $"No movement!\n";
                }
            }
            if (limitSpeed)
            {
                desc += $"Speed will be limited to {speedLimit} units per second.\n";
            }
            if (inertiaEnable)
            {
                if (inertiaStopTime > 0)
                {
                    desc += $"Object will have inertia and will stop in {inertiaStopTime} seconds when at maximum speed.\n";
                }
                else
                {
                    desc += $"Object will have inertia, but will stop immediately.\n";
                }
            }
            if (useRotation)
            {
                desc += "These directions will be relative to the current object orientation.\n";
            }
            else
            {
                if (turnToDirection)
                {
                    string axisName = (axisToAlign == Axis.UpAxis) ? ("up") : ("right");
                    desc += $"This object will turn {maxTurnSpeed} degrees/sec to align it's {axisName} axis to the movement direction.\n";
                }
            }
            if (inputEnabled)
            {
                if (inputType == InputType.Axis)
                {
                    if ((horizontalAxis != "") && (horizontalAxis != "None"))
                    {
                        desc += $"Horizontal movement will be controlled by the [{horizontalAxis}] axis.\n";
                    }
                    if ((verticalAxis != "") && (verticalAxis != "None"))
                    {
                        desc += $"Vertical movement will be controlled by the [{verticalAxis}] axis.\n";
                    }
                }
                else if (inputType == InputType.Button)
                {
                    if ((horizontalButtonPositive != "") || (horizontalButtonNegative != ""))
                    {
                        desc += $"Horizontal movement will be controlled by the [{horizontalButtonNegative}] and [{horizontalButtonPositive}] buttons.\n";
                    }
                    if ((verticalButtonPositive != "") || (verticalButtonNegative != ""))
                    {
                        desc += $"Vertical movement will be controlled by the [{verticalButtonNegative}] and [{verticalButtonPositive}] buttons.\n";
                    }
                }
                else if (inputType == InputType.Key)
                {
                    if ((horizontalKeyPositive != KeyCode.None) || (horizontalKeyNegative != KeyCode.None))
                    {
                        desc += $"Horizontal movement will be controlled by the [{horizontalKeyNegative}] and [{horizontalKeyPositive}] keys.\n";
                    }
                    if ((verticalKeyPositive != KeyCode.None) || (verticalKeyNegative != KeyCode.None))
                    {
                        desc += $"Vertical movement will be controlled by the [{verticalKeyNegative}] and [{verticalKeyPositive}] keys.\n";
                    }
                }
            }
            return desc;
        }

        void FixedUpdate()
        {
            Vector3 transformedDelta = moveVector;
            if (useRotation)
            {
                transformedDelta = moveVector.x * transform.right + moveVector.y * transform.up;
            }
            else
            {
                if (turnToDirection)
                {
                    if (transformedDelta.sqrMagnitude > 1e-6)
                    {
                        Vector3 upAxis = transformedDelta.normalized;

                        if (axisToAlign == Axis.RightAxis) upAxis = new Vector3(-upAxis.y, upAxis.x);

                        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, upAxis);

                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnSpeed * Time.fixedDeltaTime);
                    }
                }
            }

            if (inertiaEnable)
            {
                if (inertiaStopTime > 0)
                {
                    float drag = speed.magnitude / inertiaStopTime;
                    if (currentVelocity.sqrMagnitude > 1e-6)
                    {
                        float newSpeed = Mathf.Max(0, currentVelocity.magnitude - drag * Time.fixedDeltaTime);

                        currentVelocity = currentVelocity.normalized * newSpeed;
                    }
                    else
                    {
                        currentVelocity = Vector3.zero;
                    }
                }
                else
                {
                    currentVelocity = Vector3.zero;
                }
                currentVelocity += transformedDelta;

                currentVelocity.x = Mathf.Clamp(currentVelocity.x, -speed.x, speed.x);
                currentVelocity.y = Mathf.Clamp(currentVelocity.y, -speed.y, speed.y);

                if (limitSpeed) currentVelocity = currentVelocity.normalized * Mathf.Clamp(currentVelocity.magnitude, 0.0f, speedLimit);
            }
            else
            {
                currentVelocity = transformedDelta;
                if (limitSpeed) currentVelocity = currentVelocity.normalized * Mathf.Clamp(currentVelocity.magnitude, 0.0f, speedLimit);
            }
            MoveDelta(currentVelocity * Time.fixedDeltaTime);
        }

        void Update()
        {
            moveVector = Vector3.zero;
            if (inputEnabled)
            {
                switch (inputType)
                {
                    case InputType.Axis:
                        if (horizontalAxis != "") moveVector.x = Input.GetAxis(horizontalAxis) * speed.x;
                        if (verticalAxis != "") moveVector.y = Input.GetAxis(verticalAxis) * speed.y;
                        break;
                    case InputType.Button:
                        if ((horizontalButtonPositive != "") && (Input.GetButton(horizontalButtonPositive))) moveVector.x = speed.x;
                        if ((horizontalButtonNegative != "") && (Input.GetButton(horizontalButtonNegative))) moveVector.x = -speed.x;
                        if ((verticalButtonPositive != "") && (Input.GetButton(verticalButtonPositive))) moveVector.y = speed.y;
                        if ((verticalButtonNegative != "") && (Input.GetButton(verticalButtonNegative))) moveVector.y = -speed.y;
                        break;
                    case InputType.Key:
                        if ((horizontalKeyPositive != KeyCode.None) && (Input.GetKey(horizontalKeyPositive))) moveVector.x = speed.x;
                        if ((horizontalKeyNegative != KeyCode.None) && (Input.GetKey(horizontalKeyNegative))) moveVector.x = -speed.x;
                        if ((verticalKeyPositive != KeyCode.None) && (Input.GetKey(verticalKeyPositive))) moveVector.y = speed.y;
                        if ((verticalKeyNegative != KeyCode.None) && (Input.GetKey(verticalKeyNegative))) moveVector.y = -speed.y;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                moveVector = speed;
            }
        }
    }
}