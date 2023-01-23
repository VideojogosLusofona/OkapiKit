using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MovementRotate : Movement
{
    [SerializeField] private enum InputType { Axis, Button, Key };

    [SerializeField] 
    private float       speed = 200.0f;
    [SerializeField]
    private bool        inputEnabled;
    [SerializeField, ShowIf("inputEnabled")]
    private InputType   inputType;
    [SerializeField, ShowIf("axisEnabled"), InputAxis]
    private string      rotationAxis = "Horizontal";
    [SerializeField, ShowIf("buttonEnabled")]    
    private string      rotationButtonPositive = "Right";
    [SerializeField, ShowIf("buttonEnabled")]    
    private string      rotationButtonNegative = "Left";
    [SerializeField, ShowIf("keyEnabled")]    
    private KeyCode     rotationKeyPositive = KeyCode.RightArrow;
    [SerializeField, ShowIf("keyEnabled")]
    private KeyCode     rotationKeyNegative = KeyCode.LeftArrow;

    private bool axisEnabled => inputEnabled && (inputType == InputType.Axis);
    private bool buttonEnabled => inputEnabled && (inputType == InputType.Button);
    private bool keyEnabled => inputEnabled && (inputType == InputType.Key);


    public override Vector2 GetSpeed() => new Vector2(speed, speed);
    public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

    override public bool IsLinear() { return false; }

    void FixedUpdate()
    {
        if (inputEnabled)
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
        else
        {
            RotateZ(speed * Time.fixedDeltaTime);
        }
    }
}
