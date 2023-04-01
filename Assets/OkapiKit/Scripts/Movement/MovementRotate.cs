using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class MovementRotate : Movement
    {
        public enum RotateMode { Auto = 0, InputSet = 1, InputDelta = 2, Target = 3, Movement = 4 };
        public enum InputType { Axis = 0, Button = 1, Key = 2, Mouse = 3 };
        public enum Axis { UpAxis = 0, RightAxis = 1 };

        [SerializeField]
        private float speed = 200.0f;
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

        override public string GetTitle() => "Rotate";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            string axisName = (axisToAlign == Axis.UpAxis) ? ("up") : ("right");

            if (mode == RotateMode.InputDelta)
            {
                desc += $"Rotational movement, at a maximum of {speed} degrees per second.\n";
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
                desc += $"Rotational movement, at a maximum of {speed} degrees per second.\n";

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
                    desc += $"This object will align its {axisName} axis with the direction towards {targetObject.name}, at a maximum {speed} degrees per second.\n";
                }
                else if (targetTag)
                {
                    desc += $"This object will align its {axisName} axis with the direction towards the closest object tagged with {targetTag.name}, at a maximum {speed} degrees per second.\n";
                }
                else
                {
                    desc += $"This object will align its {axisName} axis with the direction towards [UNDEFINED], at a maximum {speed} degrees per second.\n";
                }
            }
            else if (mode == RotateMode.Movement)
            {
                desc += $"This object will align its {axisName} axis with the movement direction, at a maximum {speed} degrees per second.\n";
            }
            else
            {
                desc += $"Rotational movement, at {speed} degrees per second.\n";
            }
            return desc;
        }

        void FixedUpdate()
        {
            if (mode == RotateMode.InputDelta)
            {
                float rotationValue = 0.0f;
                switch (inputType)
                {
                    case InputType.Axis:
                        if ((rotationAxis != "None") && (rotationAxis != ""))
                        {
                            rotationValue = -Input.GetAxis(rotationAxis) * Time.fixedDeltaTime * speed;
                        }
                        break;
                    case InputType.Button:
                        if ((rotationButtonPositive != "None") && (rotationButtonPositive != ""))
                        {
                            rotationValue += (Input.GetButton(rotationButtonPositive)) ? (-Time.fixedDeltaTime * speed) : (0.0f);
                        }
                        if ((rotationButtonNegative != "None") && (rotationButtonNegative != ""))
                        {
                            rotationValue += (Input.GetButton(rotationButtonNegative)) ? (Time.fixedDeltaTime * speed) : (0.0f);
                        }
                        break;
                    case InputType.Key:
                        if (rotationKeyPositive != KeyCode.None)
                        {
                            rotationValue += (Input.GetKey(rotationKeyPositive)) ? (-Time.fixedDeltaTime * speed) : (0.0f);
                        }
                        if (rotationKeyNegative != KeyCode.None)
                        {
                            rotationValue += (Input.GetKey(rotationKeyNegative)) ? (Time.fixedDeltaTime * speed) : (0.0f);
                        }
                        break;
                    default:
                        break;
                }

                RotateZ(rotationValue);
            }
            else if (mode == RotateMode.InputSet)
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

                if (dir.sqrMagnitude > 1e-6)
                {
                    dir.Normalize();

                    if (axisToAlign == Axis.RightAxis) dir = new Vector2(-dir.y, dir.x);

                    RotateTo(dir, speed * Time.fixedDeltaTime);
                }
            }
            else if (mode == RotateMode.Target)
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
                    if (dir.sqrMagnitude > 1e-6)
                    {
                        dir.Normalize();

                        if (axisToAlign == Axis.RightAxis) dir = new Vector2(-dir.y, dir.x);

                        RotateTo(dir, speed * Time.fixedDeltaTime);
                    }
                }
            }
            else if (mode == RotateMode.Movement)
            {
                Vector3 dir = Vector3.zero;
                if (rb) dir = rb.velocity;
                if (dir.sqrMagnitude < 1e-6)
                {
                    dir = transform.position - prevPosition;
                }
                if (dir.sqrMagnitude > 1e-6)
                {
                    dir.Normalize();

                    if (axisToAlign == Axis.RightAxis) dir = new Vector2(-dir.y, dir.x);

                    RotateTo(dir, speed * Time.fixedDeltaTime);
                }
            }
            else
            {
                RotateZ(speed * Time.fixedDeltaTime);
            }

            Vector3 deltaPos = transform.position - prevPosition;
            if (deltaPos.sqrMagnitude > 1e-6)
            {
                prevPosition = transform.position;
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