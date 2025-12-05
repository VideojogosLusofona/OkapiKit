using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [RequireComponent(typeof(GridObject))]
    [AddComponentMenu("Okapi/Movement/XY Grid Movement")]
    public class MovementGridXY : MovementGrid
    {
        public enum InputType { Axis = 0, Button = 1, Key = 2 };
        public enum Axis { UpAxis = 0, RightAxis = 1 };
        public enum FlipBehaviour
        {
            None = 0,
            VelocityFlipsSprite = 1, VelocityInvertsScale = 2,
            VelocityRotatesSprite = 3
        };

        [SerializeField]
        private Vector2 speed = new Vector2(100, 100);
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
        private FlipBehaviour flipBehaviour = FlipBehaviour.None;
        [SerializeField]
        private bool useAnimator = false;
        [SerializeField]
        private Animator animator;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string horizontalVelocityParameter;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string absoluteHorizontalVelocityParameter;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string verticalVelocityParameter;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string absoluteVerticalVelocityParameter;

        SpriteRenderer spriteRenderer;

        public override Vector2 GetSpeed() => speed;
        public override void SetSpeed(Vector2 speed) { this.speed = speed; }

        override public string GetTitle() => "XY Grid Movement";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            Vector2 gridSpeed = speed / gridSize;

            string desc = "";
            if (speed.x != 0.0f)
            {
                if (speed.y != 0.0f)
                {
                    desc += $"Dual axis grid movement, at {speed} units per second ({gridSpeed} cells/sec).\n";
                }
                else
                {
                    desc += $"Horizontal grid movement, at {speed.x} units per second ({gridSpeed.x} cells/sec).\n";
                }
            }
            else
            {
                if (speed.y != 0.0f)
                {
                    desc += $"Vertical grid movement, at {speed.y} units per second ({gridSpeed.y} cells/sec).\n";
                }
                else
                {
                    desc += $"No movement!\n";
                }
            }
            if (cooldown > 0.0f)
            {
                desc += $"The movement steps can only happen once every {cooldown} seconds.\n";
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

            if (pushStrength > 0)
            {
                desc += $"This object can push objects as long as their mass combined doesn't exceed {pushStrength}.\n";
            }
            else
            {
                desc += $"This object can not push objects.\n";
            }

            string animDesc = "";

            switch (flipBehaviour)
            {
                case FlipBehaviour.None:
                    break;
                case FlipBehaviour.VelocityFlipsSprite:
                    animDesc += "When the character is moving to the left, the sprite renderer will be flipped.\n";
                    break;
                case FlipBehaviour.VelocityInvertsScale:
                    animDesc += "When the character is moving to the left, the horizontal scale of this object will be inverted.\n";
                    break;
                case FlipBehaviour.VelocityRotatesSprite:
                    animDesc += "When the character is moving to the left, the object will be rotated 180 degrees around the Y axis.\n";
                    break;
                default:
                    break;
            }

            if (useAnimator)
            {
                Animator anim = animator;
                if (anim == null) anim = GetComponent<Animator>();
                if (anim)
                {
                    animDesc += $"Some values will be set on animator {anim.name}:\n";
                    if (horizontalVelocityParameter != "") animDesc += $"Horizontal velocity will be set to parameter {horizontalVelocityParameter}.\n";
                    if (absoluteHorizontalVelocityParameter != "") animDesc += $"Absolute horizontal velocity will be set to parameter {absoluteHorizontalVelocityParameter}.\n";
                    if (verticalVelocityParameter != "") animDesc += $"Vertical velocity will be set to parameter {verticalVelocityParameter}.\n";
                    if (absoluteVerticalVelocityParameter != "") animDesc += $"Absolute vertical velocity will be set to parameter {absoluteVerticalVelocityParameter}.\n";
                }
            }

            if (animDesc != "")
            {
                desc += animDesc;
            }

            return desc;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            Grid grid = GetComponentInParent<Grid>();
            if (grid == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No grid in parent", "Grid objects need to be a child of an object with a Grid component!"));
            }

            if (inputEnabled)
            {
                if (inputType == InputType.Button)
                {
                    CheckButton("Positive horizontal button", horizontalButtonNegative);
                    CheckButton("Negative horizontal button", horizontalButtonPositive);
                    CheckButton("Positive vertical button", verticalButtonNegative);
                    CheckButton("Negative vertical button", verticalButtonPositive);
                }
            }

            if (flipBehaviour == FlipBehaviour.VelocityFlipsSprite)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Flip behaviour is set to flip the sprite, but sprite doesn't exist!", "Flip behaviour acts on sprite, but no sprite is present!"));
                }
            }

            if (useAnimator)
            {
                Animator anm = animator;
                if (anm == null)
                {
                    anm = GetComponent<Animator>();
                    if (anm == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Animator not defined!", "If we want to drive an animator with the properties of the movement, we need to define which animator to use."));
                    }
                    else
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Warning, "Animator exists, but it should be linked explicitly!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                    }
                }
                if (anm != null)
                {
                    if (anm.runtimeAnimatorController == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Animator controller is not set!", "There's an animator, but it doesn't have a controller setup. Creat one and set it on the animator."));
                    }
                    else
                    {
                        Helpers.CheckErrorAnim(anm, "horizontal velocity", horizontalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                        Helpers.CheckErrorAnim(anm, "absolute horizontal velocity", absoluteHorizontalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                        Helpers.CheckErrorAnim(anm, "vertical velocity", verticalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                        Helpers.CheckErrorAnim(anm, "absolute vertical velocity", absoluteVerticalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponent<SpriteRenderer>(); 
        }

        protected override void Update()
        {
            base.Update();

            if (!isMovementActive()) return;

            if (gridObject.isMoving)
            {
                if ((!useRotation) && (turnToDirection))
                {
                    if (gridObject.lastDelta.sqrMagnitude > 1e-6)
                    {
                        Vector3 upAxis = gridObject.lastDelta.normalized;

                        if (axisToAlign == Axis.RightAxis) upAxis = new Vector3(-upAxis.y, upAxis.x);

                        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, upAxis);

                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnSpeed * Time.fixedDeltaTime);
                    }
                }
            }
            else if (moveCooldownTimer <= 0.0f)
            {
                Vector2 moveVector = Vector3.zero;
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

                MoveInDirection(moveVector, useRotation);
            }

            const float epsilonZero = 1e-3f;
            switch (flipBehaviour)
            {
                case FlipBehaviour.None:
                    break;
                case FlipBehaviour.VelocityFlipsSprite:
                    if (spriteRenderer)
                    {
                        if (currentVelocity.x > epsilonZero) spriteRenderer.flipX = false;
                        else if (currentVelocity.x < -epsilonZero) spriteRenderer.flipX = true;
                    }
                    break;
                case FlipBehaviour.VelocityInvertsScale:
                    if ((currentVelocity.x > epsilonZero) && (transform.localScale.x < 0.0f)) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    else if ((currentVelocity.x < -epsilonZero) && (transform.localScale.x > 0.0f)) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    break;
                case FlipBehaviour.VelocityRotatesSprite:
                    if ((currentVelocity.x > epsilonZero) && (transform.right.x < 0.0f)) transform.rotation *= Quaternion.Euler(0, 180, 0);
                    else if ((currentVelocity.x < -epsilonZero) && (transform.right.x > 0.0f)) transform.rotation *= transform.rotation *= Quaternion.Euler(0, 180, 0);
                    break;
                default:
                    break;
            }

            if ((useAnimator) && (animator))
            {
                if (horizontalVelocityParameter != "") animator.SetFloat(horizontalVelocityParameter, currentVelocity.x);
                if (absoluteHorizontalVelocityParameter != "") animator.SetFloat(absoluteHorizontalVelocityParameter, Mathf.Abs(currentVelocity.x));
                if (verticalVelocityParameter != "") animator.SetFloat(verticalVelocityParameter, currentVelocity.y);
                if (absoluteVerticalVelocityParameter != "") animator.SetFloat(absoluteVerticalVelocityParameter, Mathf.Abs(currentVelocity.y));
            }
        }
    }
}