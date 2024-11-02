using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.IO;
using System;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Movement/Rotate Grid Movement")]
    public class MovementGridRotate : MovementGrid
    {
        public enum RotateMode { Auto = 0, InputSet = 1, InputDelta = 2, Target = 3, Movement = 4 };
        public enum InputType { Axis = 0, Button = 1, Key = 2, Mouse = 3 };
        public enum Axis { UpAxis = 0, RightAxis = 1 };

        [SerializeField]
        private float speed = 200.0f;
        [SerializeField]
        private float angularStepSize = 90.0f;
        [SerializeField]
        RotateMode mode = RotateMode.Auto;
        [SerializeField]
        private InputType inputType;
        [SerializeField, InputAxis]
        private string rotationAxis = "Horizontal";
        [SerializeField]
        private string rotationButtonPositive = "Right";
        [SerializeField]
        private string rotationButtonNegative = "Left";
        [SerializeField]
        private KeyCode rotationKeyPositive = KeyCode.RightArrow;
        [SerializeField]
        private KeyCode rotationKeyNegative = KeyCode.LeftArrow;
        [SerializeField, InputAxis]
        private string rotationAxisX = "Horizontal";
        [SerializeField, InputAxis]
        private string rotationAxisY = "Vertical";
        [SerializeField]
        private string rotationButtonPositiveX = "Right";
        [SerializeField]
        private string rotationButtonNegativeX = "Left";
        [SerializeField]
        private string rotationButtonPositiveY = "Up";
        [SerializeField]
        private string rotationButtonNegativeY = "Down";
        [SerializeField]
        private KeyCode rotationKeyPositiveX = KeyCode.RightArrow;
        [SerializeField]
        private KeyCode rotationKeyNegativeX = KeyCode.LeftArrow;
        [SerializeField]
        private KeyCode rotationKeyPositiveY = KeyCode.UpArrow;
        [SerializeField]
        private KeyCode rotationKeyNegativeY = KeyCode.DownArrow;
        [SerializeField]
        private Axis axisToAlign = Axis.UpAxis;
        [SerializeField]
        private Transform targetObject;
        [SerializeField]
        private Hypertag targetTag;
        [SerializeField]
        private Camera cameraObject;
        [SerializeField]
        private Hypertag cameraTag;

        Vector3 prevPosition;

        public override Vector2 GetSpeed() => new Vector2(speed, speed);
        public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

        override public bool IsLinear() { return false; }

        override public string GetTitle() => "Grid Rotate";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            string axisName = (axisToAlign == Axis.UpAxis) ? ("up") : ("right");

            if (mode == RotateMode.InputDelta)
            {
                desc += $"Rotational movement, at a maximum of {speed} degrees per second, in increments of {angularStepSize} degrees.\n";
                if (inputType == InputType.Axis)
                {
                    if ((rotationAxis != "") && (rotationAxis != "None"))
                    {
                        desc += $"Rotation will be controlled by the [{rotationAxis}] axis.\n";
                    }
                }
                else if (inputType == InputType.Button)
                {
                    if ((rotationButtonNegative != "") || (rotationButtonPositive != ""))
                    {
                        desc += $"The [{rotationButtonNegative}] button will turn counter-clockwise, and the [{rotationButtonPositive}] button will turn clockwise.\n";
                    }
                }
                else if (inputType == InputType.Key)
                {
                    if ((rotationKeyNegative != KeyCode.None) || (rotationKeyPositive != KeyCode.None))
                    {
                        desc += $"[{rotationKeyNegative}] will turn counter-clockwise, and [{rotationButtonPositive}] will turn clockwise.\n";
                    }
                }
                else if (inputType == InputType.Mouse)
                {
                    desc += $"Object's {axisName} axis will point in the direction of the mouse.\n";
                }
            }
            else if (mode == RotateMode.InputSet)
            {
                desc += $"Rotational movement, at a maximum of {speed} degrees per second, in increments of {angularStepSize} degrees.\n";

                if (inputType == InputType.Axis)
                {
                    if ((rotationAxisX != "") && (rotationAxisX != "None") && (rotationAxisY != "") && (rotationAxisY != "None"))
                    {
                        desc += $"Object's {axisName} axis will point in the direction given by axis [{rotationAxisX}] and [{rotationAxisY}].\n";
                    }
                }
                else if (inputType == InputType.Button)
                {
                    if ((rotationButtonNegativeX != "") || (rotationButtonPositiveX != "") && (rotationButtonNegativeY != "") || (rotationButtonPositiveY != ""))
                    {
                        desc += $"Object's {axisName} axis will point in the direction given by buttons [{rotationButtonNegativeX}], [{rotationButtonPositiveX}], [{rotationButtonNegativeY}] and [{rotationButtonPositiveY}].\n";
                    }
                }
                else if (inputType == InputType.Key)
                {
                    if ((rotationKeyNegativeX != KeyCode.None) || (rotationKeyPositiveX != KeyCode.None) || (rotationKeyNegativeY != KeyCode.None) || (rotationKeyPositiveY != KeyCode.None))
                    {
                        desc += $"Object's {axisName} axis will point in the direction given by keys [{rotationKeyNegativeX}], [{rotationKeyPositiveX}], [{rotationKeyNegativeY}] and [{rotationKeyPositiveY}].\n";
                    }
                }
                else if (inputType == InputType.Mouse)
                {
                    desc += $"Object's {axisName} axis will point in the direction of the mouse.\n";
                }
            }
            else if (mode == RotateMode.Target)
            {
                if (targetObject)
                {
                    desc += $"This object will align its {axisName} axis with the direction towards {targetObject.name}, at a maximum {speed} degrees per second, in increments of {angularStepSize} degrees.\n";
                }
                else if (targetTag)
                {
                    desc += $"This object will align its {axisName} axis with the direction towards the closest object tagged with {targetTag.name}, at a maximum {speed} degrees per second, in increments of {angularStepSize} degrees.\n";
                }
                else
                {
                    desc += $"This object will align its {axisName} axis with the direction towards [UNDEFINED], at a maximum {speed} degrees per second, in increments of {angularStepSize} degrees.\n";
                }
            }
            else if (mode == RotateMode.Movement)
            {
                desc += $"This object will align its {axisName} axis with the movement direction, at a maximum {speed} degrees per second, in increments of {angularStepSize} degrees.\n";
            }
            else
            {
                desc += $"Rotational movement, at {speed} degrees per second, in increments of {angularStepSize} degrees.\n";
            }
            if (cooldown > 0.0f)
            {
                desc += $"Each {angularStepSize} degree rotation can can only happen once every {cooldown} seconds.\n";
            }
            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (mode == RotateMode.InputSet)
            {
                if (inputType == InputType.Button)
                {
                    CheckButton("Positive X rotation button", rotationButtonPositiveX);
                    CheckButton("Negative X rotation button", rotationButtonNegativeX);
                    CheckButton("Positive Y rotation button", rotationButtonPositiveY);
                    CheckButton("Negative Y rotation button", rotationButtonNegativeY);
                }
            }
            else if (mode == RotateMode.InputDelta)
            {
                if (inputType == InputType.Button)
                {
                    CheckButton("Positive rotation button", rotationButtonPositive);
                    CheckButton("Negative rotation button", rotationButtonNegative);
                }
            }

            if (mode == RotateMode.Target)
            {
                if ((targetTag == null) && (targetObject == null))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Target not defined - use either tag or drag the object to the slot below!", "If we want to rotate towards an object, we need to define which object."));
                }
            }
        }

        void FixedUpdate()
        {
            if (!isMovementActive()) return;

            if (gridObject.isRotating)
            {

            }
            else if (moveCooldownTimer <= 0.0f)
            {
                switch (mode)
                {
                    case RotateMode.Auto:
                        RotateAuto();
                        break;
                    case RotateMode.InputSet:
                        RotateInputSet();
                        break;
                    case RotateMode.InputDelta:
                        RotateInputDelta();
                        break;
                    case RotateMode.Target:
                        RotateTarget();
                        break;
                    case RotateMode.Movement:
                        RotateMovement();
                        break;
                    default:
                        break;
                }
            }

/*            if (mode == RotateMode.InputSet)
            {
                
            }
            else if (mode == RotateMode.Target)
            {
                
            }
            else if (mode == RotateMode.Movement)
            {
                
            }
            else
            {
                RotateZ(speed * Time.fixedDeltaTime);
            }

            Vector3 deltaPos = transform.position - prevPosition;
            if (deltaPos.sqrMagnitude > 1e-6)
            {
                prevPosition = transform.position;
            }*/
        }

        void RotateAuto()
        {
            if (gridObject.RotateOnGrid(GetSpeed().x, angularStepSize))
            {
                moveCooldownTimer = Mathf.Min(Mathf.Abs(GetSpeed().x) / angularStepSize, cooldown);
            }
        }

        void RotateInputDelta()
        {
            float rotationValue = 0.0f;
            switch (inputType)
            {
                case InputType.Axis:
                    if ((rotationAxis != "None") && (rotationAxis != ""))
                    {
                        rotationValue = -Input.GetAxis(rotationAxis);
                    }
                    break;
                case InputType.Button:
                    if ((rotationButtonPositive != "None") && (rotationButtonPositive != ""))
                    {
                        rotationValue += (Input.GetButton(rotationButtonPositive)) ? (-1.0f) : (0.0f);
                    }
                    if ((rotationButtonNegative != "None") && (rotationButtonNegative != ""))
                    {
                        rotationValue += (Input.GetButton(rotationButtonNegative)) ? (1.0f) : (0.0f);
                    }
                    break;
                case InputType.Key:
                    if (rotationKeyPositive != KeyCode.None)
                    {
                        rotationValue += (Input.GetKey(rotationKeyPositive)) ? (-1.0f) : (0.0f);
                    }
                    if (rotationKeyNegative != KeyCode.None)
                    {
                        rotationValue += (Input.GetKey(rotationKeyNegative)) ? (1.0f) : (0.0f);
                    }
                    break;
                default:
                    break;
            }
            if (rotationValue < -1e-3) rotationValue = -1.0f;
            else if (rotationValue > 1e-3) rotationValue = 1.0f;
            else rotationValue = 0.0f;

            if (gridObject.RotateOnGrid(GetSpeed().x * rotationValue, angularStepSize))
            {
                moveCooldownTimer = Mathf.Min(Mathf.Abs(GetSpeed().x) / angularStepSize, cooldown);
            }            
        }

        void RotateInputSet()
        {
            Vector2 dir = Vector2.zero;

            switch (inputType)
            {
                case InputType.Axis:
                    dir.x = Input.GetAxis(rotationAxisX);
                    dir.y = Input.GetAxis(rotationAxisY);
                    break;
                case InputType.Button:
                    dir.x = (Input.GetButton(rotationButtonNegativeX) ? (-1.0f) : (0.0f)) + (Input.GetButton(rotationButtonPositiveX) ? (1.0f) : (0.0f));
                    dir.y = (Input.GetButton(rotationButtonNegativeY) ? (-1.0f) : (0.0f)) + (Input.GetButton(rotationButtonPositiveY) ? (1.0f) : (0.0f));
                    break;
                case InputType.Key:
                    dir.x = (Input.GetKey(rotationKeyNegativeX) ? (-1.0f) : (0.0f)) + (Input.GetKey(rotationKeyPositiveX) ? (1.0f) : (0.0f));
                    dir.y = (Input.GetKey(rotationKeyNegativeY) ? (-1.0f) : (0.0f)) + (Input.GetKey(rotationKeyPositiveY) ? (1.0f) : (0.0f));
                    break;
                case InputType.Mouse:
                    {
                        var camera = GetCamera();
                        if (camera == null) dir = Vector2.zero;
                        else
                        {
                            Vector3 mp = camera.ScreenToWorldPoint(Input.mousePosition);
                            dir = mp - transform.position;
                        }
                    }
                    break;
                default:
                    break;
            }

            RotateToDirection(dir);
        }
        
        void RotateTarget()
        {
            Transform targetTransform = null;

            if (targetTag)
            {
                var potentialObjects = gameObject.FindObjectsOfTypeWithHypertag<Transform>(targetTag);
                var minDist = float.MaxValue;
                foreach (var obj in potentialObjects)
                {
                    var d = Vector3.Distance(obj.position, transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        targetTransform = obj;
                    }
                }
            }
            else if (targetObject)
            {
                targetTransform = targetObject;
            }

            if (targetTransform)
            {
                Vector2 dir = (targetTransform.position - transform.position);
                RotateToDirection(dir);
            }
        }

        void RotateMovement()
        {
            Vector3 dir = Vector3.zero;
            if (rb) dir = rb.linearVelocity;
            if (dir.sqrMagnitude < 1e-6)
            {
                dir = transform.position - prevPosition;
            }
            RotateToDirection(dir);
        }

        void RotateToDirection(Vector2 dir)
        {
            if (dir.sqrMagnitude > 1e-6)
            {
                dir.Normalize();

                float targetAngle = Mathf.RoundToInt((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) / angularStepSize) * angularStepSize;
                float sourceAngle = Mathf.RoundToInt(transform.rotation.eulerAngles.z / angularStepSize) * angularStepSize;
                if (axisToAlign == Axis.UpAxis) sourceAngle += 90.0f;

                float deltaAngle = Mathf.DeltaAngle(sourceAngle, targetAngle);
                if (deltaAngle < -1e-3) deltaAngle = -1.0f;
                else if (deltaAngle > 1e-3) deltaAngle = 1.0f;
                else deltaAngle = 0.0f;

                if (gridObject.RotateOnGrid(GetSpeed().x * deltaAngle, angularStepSize))
                {
                    moveCooldownTimer = Mathf.Min(Mathf.Abs(GetSpeed().x) / angularStepSize, cooldown);
                }
            }
        }

        Camera GetCamera()
        {
            if (cameraTag)
            {
                return gameObject.FindObjectOfTypeWithHypertag<Camera>(cameraTag);
            }

            return cameraObject;
        }
    }
}