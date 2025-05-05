using NaughtyAttributes;
using System;
using UnityEngine;

namespace OkapiKit
{
    [Serializable]
    public class InputControl
    {
        public enum InputType { Axis = 0, Button = 1, Key = 2 };

        [SerializeField]
        private InputType _type;
        public InputType type => _type;
        [SerializeField, InputAxis]
        private string axis = "Horizontal";
        [SerializeField, InputAxis]
        private string buttonPositive = "Right";
        [SerializeField, InputAxis]
        private string buttonNegative = "Left";
        [SerializeField]
        private KeyCode keyPositive = KeyCode.RightArrow;
        [SerializeField]
        private KeyCode keyNegative = KeyCode.LeftArrow;

        public float GetAxis()
        {
            float v = 0.0f;

            switch (type)
            {
                case InputType.Axis:
                    v = Input.GetAxis(axis);
                    break;
                case InputType.Button:
                    if ((!string.IsNullOrEmpty(buttonPositive)) && (Input.GetButton(buttonPositive))) v += 1.0f;
                    if ((!string.IsNullOrEmpty(buttonNegative)) && (Input.GetButton(buttonNegative))) v -= 1.0f;
                    break;
                case InputType.Key:
                    if ((keyPositive != KeyCode.None) && (Input.GetKey(keyPositive))) v += 1.0f;
                    if ((keyNegative != KeyCode.None) && (Input.GetKey(keyNegative))) v -= 1.0f;
                    break;
                default:
                    break;
            }

            return v;
        }

        public bool IsPressed()
        {
            bool ret = false;

            switch (type)
            {
                case InputType.Axis:
                    ret = Mathf.Abs(Input.GetAxis(axis)) > 0.5f;
                    break;
                case InputType.Button:
                    if (!string.IsNullOrEmpty(buttonPositive)) ret = Input.GetButton(buttonPositive);
                    break;
                case InputType.Key:
                    if (keyPositive != KeyCode.None) ret = Input.GetKey(keyPositive);
                    break;
                default:
                    break;
            }

            return ret;
        }

        public bool IsDown()
        {
            bool ret = false;

            switch (type)
            {
                case InputType.Axis:
                    ret = false;
                    break;
                case InputType.Button:
                    if (!string.IsNullOrEmpty(buttonPositive)) ret = Input.GetButtonDown(buttonPositive);
                    break;
                case InputType.Key:
                    if (keyPositive != KeyCode.None) ret = Input.GetKeyDown(keyPositive);
                    break;
                default:
                    break;
            }

            return ret;
        }

        public bool IsUp()
        {
            bool ret = false;

            switch (type)
            {
                case InputType.Axis:
                    ret = false;
                    break;
                case InputType.Button:
                    if (!string.IsNullOrEmpty(buttonPositive)) ret = Input.GetButtonUp(buttonPositive);
                    break;
                case InputType.Key:
                    if (keyPositive != KeyCode.None) ret = Input.GetKeyUp(keyPositive);
                    break;
                default:
                    break;
            }

            return ret;
        }
    }

    [Flags]
    public enum AllowInput
    {
        None = 0,
        Axis = 1 << 0,
        Button = 1 << 1,
        Key = 1 << 2,
        NewInput = 1 << 3,
        All = Axis | Button | Key | NewInput
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class AllowInputAttribute : PropertyAttribute
    {
        public AllowInput AllowedInputs { get; private set; }

        public AllowInputAttribute(AllowInput allowedInputs = AllowInput.All)
        {
            AllowedInputs = allowedInputs;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class InputButtonAttribute : PropertyAttribute
    {
        // No additional properties required for this attribute
    }
}
