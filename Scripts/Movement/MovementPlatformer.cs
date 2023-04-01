using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementPlatformer : Movement
    {
        public enum InputType { Axis = 0, Button = 1, Key = 2 };
        public enum FlipBehaviour { None = 0, VelocityFlipsSprite = 1, VelocityInvertsScale = 2, InputFlipsSprite = 3, InputInvertsScale = 4 };
        public enum JumpBehaviour { None = 0, Fixed = 1, Variable = 2 };
        public enum GlideBehaviour { None = 0, Enabled = 1, Timer = 2 };

        [SerializeField]
        private Vector2 speed = new Vector2(100, 100);
        [SerializeField]
        private InputType horizontalInputType;
        [SerializeField, InputAxis]
        private string horizontalAxis = "Horizontal";
        [SerializeField]
        private string horizontalButtonPositive = "Right";
        [SerializeField]
        private string horizontalButtonNegative = "Left";
        [SerializeField]
        private KeyCode horizontalKeyPositive = KeyCode.RightArrow;
        [SerializeField]
        private KeyCode horizontalKeyNegative = KeyCode.LeftArrow;
        [SerializeField]
        private float gravityScale = 0;
        [SerializeField]
        private bool useTerminalVelocity = false;
        [SerializeField]
        private float terminalVelocity = 100.0f;
        [SerializeField]
        private float coyoteTime = 0.0f;
        [SerializeField]
        private JumpBehaviour jumpBehaviour = JumpBehaviour.None;
        [SerializeField]
        private int maxJumpCount = 1;
        [SerializeField]
        private float jumpBufferingTime = 0.1f;
        [SerializeField]
        private float jumpHoldMaxTime = 0.1f;
        [SerializeField]
        private InputType jumpInputType;
        [SerializeField, InputAxis]
        private string jumpAxis = "Vertical";
        [SerializeField]
        private string jumpButton = "Jump";
        [SerializeField]
        private KeyCode jumpKey = KeyCode.Space;
        [SerializeField]
        private bool enableAirControl = true;
        [SerializeField]
        private Collider2D airCollider;
        [SerializeField]
        private Collider2D groundCollider;
        [SerializeField]
        private GlideBehaviour glideBehaviour = GlideBehaviour.None;
        [SerializeField]
        private float glideMaxTime = float.MaxValue;
        [SerializeField]
        private float maxGlideSpeed = 50.0f;
        [SerializeField]
        private InputType glideInputType;
        [SerializeField, InputAxis]
        private string glideAxis = "Vertical";
        [SerializeField]
        private string glideButton = "Jump";
        [SerializeField]
        private KeyCode glideKey = KeyCode.Space;
        [SerializeField]
        private Collider2D groundCheckCollider;
        [SerializeField]
        private LayerMask groundLayerMask;
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
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Bool)]
        private string isGroundedParameter;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Bool)]
        private string isGlidingParameter;

        public bool isGrounded { get; private set; }
        private SpriteRenderer spriteRenderer;
        private int currentJumpCount;
        private bool prevJumpKey = false;
        private float jumpBufferingTimer = 0.0f;
        private float jumpTime;
        private float coyoteTimer;
        private bool actualIsGrounded;
        private float glideTimer = 0.0f;
        public bool isGliding { get; private set; }

        const float epsilonZero = 1e-3f;

        public override Vector2 GetSpeed() => speed;
        public override void SetSpeed(Vector2 speed) { this.speed = speed; }

        public void SetGravityScale(float v) { gravityScale = v; }
        public float GetGravityScale() => gravityScale;

        public void SetMaxJumpCount(int v) { maxJumpCount = v; }

        public void SetJumpHoldTime(float v) { jumpHoldMaxTime = v; }
        public float GetJumpHoldTime() => jumpHoldMaxTime;
        public void SetGlideMaxTime(float v) { glideMaxTime = v; }
        public float GetGlideMaxTime() => glideMaxTime;

        override public string GetTitle() => "Platformer Movement";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";

            desc += $"Platformer movement; horizontal speed is {speed.x} units per second.\n";
            if (horizontalInputType == InputType.Axis)
            {
                if ((horizontalAxis != "") && (horizontalAxis != "None"))
                {
                    desc += $"Horizontal movement will be controlled by the [{horizontalAxis}] axis.\n";
                }
            }
            else if (horizontalInputType == InputType.Button)
            {
                if ((horizontalButtonPositive != "") || (horizontalButtonNegative != ""))
                {
                    desc += $"Horizontal movement will be controlled by the [{horizontalButtonNegative}] and [{horizontalButtonPositive}] buttons.\n";
                }
            }
            else if (horizontalInputType == InputType.Key)
            {
                if ((horizontalKeyPositive != KeyCode.None) || (horizontalKeyNegative != KeyCode.None))
                {
                    desc += $"Horizontal movement will be controlled by the [{horizontalKeyNegative}] and [{horizontalKeyPositive}] keys.\n";
                }
            }
            string groundCheckColliderName = (groundCheckCollider) ? (groundCheckCollider.name) : ("[UNDEFINED]");
            desc += $"Ground check is going to use collider {groundCheckColliderName}, the ground layer mask defines what is considered solid ground.\n";

            if (coyoteTime > 0)
            {
                desc += $"When there's no solid ground below character, the character will start falling after {coyoteTime}s, with a gravity scale of {gravityScale}.\n";
            }
            else
            {
                desc += $"When there's no solid ground below character, the character will fall with a gravity scale of {gravityScale}.\n";
            }
            if (useTerminalVelocity) desc += $"The character's vertical fall speed will never exceed {terminalVelocity} units per second.\n";
            if (enableAirControl) desc += $"The character's trajectory can be modified mid-air.\n";
            if ((airCollider) && (groundCollider)) desc += $"When in the air, the character's collider will be {airCollider.name}({airCollider}), and on the ground will be {groundCollider.name}({groundCollider}).\n";
            else if (airCollider) desc += $"When in the air, the character's collider will be {airCollider.name}({airCollider}), and will not collide when on the ground.\n";
            else if (groundCollider) desc += $"When in the air, the character's will not collider, and on the ground will collide using {groundCollider.name}({groundCollider}).\n";
            else desc += "The character has no collision controlled by this movement system!\n";
            if ((jumpBehaviour == JumpBehaviour.None) || (maxJumpCount == 0))
            {
                desc += "The character will not jump (it can still fall).\n";
            }
            else
            {
                if (jumpBehaviour == JumpBehaviour.Fixed)
                {
                    if (jumpInputType == InputType.Axis)
                    {
                        if ((horizontalAxis != "") && (horizontalAxis != "None"))
                        {
                            desc += $"The character will jump at a fixed height by using the [{jumpAxis}] axis.\n";
                        }
                    }
                    else if (jumpInputType == InputType.Button)
                    {
                        if (jumpButton != "")
                        {
                            desc += $"The character will jump at a fixed height by using the [{jumpButton}] button.\n";
                        }
                    }
                    else if (jumpInputType == InputType.Key)
                    {
                        if (jumpKey != KeyCode.None)
                        {
                            desc += $"The character will jump at a fixed height by using the [{jumpKey}] button.\n";
                        }
                    }

                    desc += $"The initial vertical velocity will be {speed.y} units/second.\n";

                    float timeToSpeedEqualZero = Mathf.Abs(speed.y / (Physics2D.gravity.y * gravityScale));
                    float minJumpHeight = speed.y * timeToSpeedEqualZero + 0.5f * (Physics2D.gravity.y * gravityScale) * timeToSpeedEqualZero * timeToSpeedEqualZero;

                    desc += $"Jump height will be {(int)minJumpHeight} units ({timeToSpeedEqualZero.ToString("0.##")} seconds to top of trajectory).\n";
                }
                else if (jumpBehaviour == JumpBehaviour.Variable)
                {
                    if (jumpInputType == InputType.Axis)
                    {
                        if ((jumpAxis != "") && (jumpAxis != "None"))
                        {
                            desc += $"The character will jump by holding the [{jumpAxis}] axis.\n";
                        }
                    }
                    else if (jumpInputType == InputType.Button)
                    {
                        if (jumpButton != "")
                        {
                            desc += $"The character will jump by holding the [{jumpButton}] button.\n";
                        }
                    }
                    else if (jumpInputType == InputType.Key)
                    {
                        if (jumpKey != KeyCode.None)
                        {
                            desc += $"The character will jump by holding the [{jumpKey}] button.\n";
                        }
                    }

                    desc += $"The vertical velocity will be {speed.y} units/second while jump is pressed, during a maximum of {jumpHoldMaxTime} seconds.\n";

                    float timeToSpeedEqualZero = Mathf.Abs(speed.y / (Physics2D.gravity.y * gravityScale));
                    float minJumpHeight = speed.y * timeToSpeedEqualZero + 0.5f * (Physics2D.gravity.y * gravityScale) * timeToSpeedEqualZero * timeToSpeedEqualZero;
                    float maxJumpHeight = jumpHoldMaxTime * speed.y + minJumpHeight;

                    desc += $"Jump height will be between {(int)minJumpHeight} to {(int)maxJumpHeight} units (from {timeToSpeedEqualZero.ToString("0.##")} to {(timeToSpeedEqualZero + jumpHoldMaxTime).ToString("0.##")} seconds to top of trajectory).\n";
                }

                if (maxJumpCount == 2) desc += $"The character will be able to double jump.\n";
                else if (maxJumpCount == 3) desc += $"The character will be able to triple jump.\n";
                else if (maxJumpCount > 3) desc += $"The character will be able to jump up to {maxJumpCount} times without touching the ground.\n";

                if (jumpBufferingTime > 0)
                {
                    desc += $"If the character presses the jump button up to {jumpBufferingTime}s before hitting the ground, it will jump automatically again.\n";
                }
            }

            if (glideBehaviour != GlideBehaviour.None)
            {
                if (glideInputType == InputType.Axis)
                {
                    if ((glideAxis != "") && (glideAxis != "None"))
                    {
                        desc += $"The character will glide by holding the [{glideAxis}] axis, reducing it's vertical speed to {maxGlideSpeed} units/s when falling.\n";
                    }
                }
                else if (glideInputType == InputType.Button)
                {
                    if (glideButton != "")
                    {
                        desc += $"The character will glide by holding the [{glideButton}] button, reducing it's vertical speed to {maxGlideSpeed} units/s when falling.\n";
                    }
                }
                else if (glideInputType == InputType.Key)
                {
                    if (glideKey != KeyCode.None)
                    {
                        desc += $"The character will glide by holding the [{glideKey}] key, reducing it's vertical speed to {maxGlideSpeed} units/s when falling.\n";
                    }
                }
            }
            if (glideBehaviour == GlideBehaviour.Timer)
            {
                desc += $"Character can only glide a maximum of {glideMaxTime} seconds.\n";
            }

            desc += "This controller also controls some visuals:\n";
            switch (flipBehaviour)
            {
                case FlipBehaviour.None:
                    break;
                case FlipBehaviour.VelocityFlipsSprite:
                    desc += "When the character is moving to the left, the sprite renderer will be flipped.\n";
                    break;
                case FlipBehaviour.VelocityInvertsScale:
                    desc += "When the character is moving to the left, the horizontal scale of this object will be inverted.\n";
                    break;
                case FlipBehaviour.InputFlipsSprite:
                    desc += "When the player intent is to go left, the sprite renderer will be flipped.\n";
                    break;
                case FlipBehaviour.InputInvertsScale:
                    desc += "When the player intent is to go left, the horizontal scale of this object will be inverted.\n";
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
                    desc += $"Some values will be set on animator {anim.name}:\n";
                    if (horizontalVelocityParameter != "") desc += $"Horizontal velocity will be set to parameter {horizontalVelocityParameter}.\n";
                    if (absoluteHorizontalVelocityParameter != "") desc += $"Absolute horizontal velocity will be set to parameter {absoluteHorizontalVelocityParameter}.\n";
                    if (verticalVelocityParameter != "") desc += $"Vertical velocity will be set to parameter {verticalVelocityParameter}.\n";
                    if (absoluteVerticalVelocityParameter != "") desc += $"Absolute vertical velocity will be set to parameter {absoluteVerticalVelocityParameter}.\n";
                    if (isGroundedParameter != "") desc += $"Grounded state will be set to parameter {isGroundedParameter}.\n";
                    if (isGlidingParameter != "") desc += $"Gliding state will be set to parameter {isGroundedParameter}.\n";
                }
            }

            return desc;
        }

        protected override void Start()
        {
            base.Start();

            if (rb)
            {
                rb.gravityScale = 0.0f;
            }
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        void FixedUpdate()
        {
            UpdateGroundState();

            // Jump buffering
            if ((jumpBehaviour != JumpBehaviour.None) && (jumpBufferingTimer > 0))
            {
                jumpBufferingTimer -= Time.fixedDeltaTime;
                if (isGrounded)
                {
                    Jump();
                }
            }

            // Fixed height jump
            if (jumpBehaviour == JumpBehaviour.Fixed)
            {
                bool isJumpPressed = GetJumpPressed();
                if ((isJumpPressed) && (!prevJumpKey))
                {
                    jumpBufferingTimer = jumpBufferingTime;

                    if ((isJumpPressed) && (!prevJumpKey))
                    {
                        if ((isGrounded) && (currentJumpCount == maxJumpCount))
                        {
                            Jump();
                        }
                        else if (currentJumpCount > 0)
                        {
                            Jump();
                        }
                    }
                }
                prevJumpKey = isJumpPressed;
            }
            else
            {
                bool isJumpPressed = GetJumpPressed();
                if (isJumpPressed)
                {
                    if (!prevJumpKey)
                    {
                        jumpBufferingTimer = jumpBufferingTime;

                        if ((isGrounded) && (currentJumpCount == maxJumpCount))
                        {
                            Jump();
                        }
                        else if (currentJumpCount > 0)
                        {
                            Jump();
                        }
                    }
                    else if ((Time.time - jumpTime) < jumpHoldMaxTime)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, speed.y);
                    }
                }
                else
                {
                    // Jump button was released, so it doesn't count anymore as being pressed
                    jumpTime = -float.MaxValue;
                }
                prevJumpKey = isJumpPressed;
            }

            bool limitFallSpeed = false;
            float maxFallSpeed = float.MaxValue;

            if (useTerminalVelocity)
            {
                limitFallSpeed = true;
                maxFallSpeed = terminalVelocity;
            }

            isGliding = false;
            if (glideBehaviour != GlideBehaviour.None)
            {
                if ((GetGlidePressed()) && ((glideTimer >= 0.0f) || (glideBehaviour == GlideBehaviour.Enabled)))
                {
                    glideTimer -= Time.fixedDeltaTime;
                    limitFallSpeed = true;
                    maxFallSpeed = maxGlideSpeed;
                    isGliding = true;
                }
                else
                {
                    isGliding = false;
                }
            }
            else isGliding = false;

            if (limitFallSpeed)
            {
                var currentVelocity = rb.velocity;
                if (currentVelocity.y < -maxFallSpeed)
                {
                    currentVelocity.y = -maxFallSpeed;
                    rb.velocity = currentVelocity;
                }
            }
        }

        void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, speed.y);
            jumpBufferingTimer = 0.0f;
            jumpTime = Time.time;
            currentJumpCount--;
        }

        bool GetJumpPressed()
        {
            switch (jumpInputType)
            {
                case InputType.Axis:
                    if (jumpAxis != "") return Input.GetAxis(jumpAxis) > epsilonZero;
                    break;
                case InputType.Button:
                    if ((jumpButton != "") && (Input.GetButton(jumpButton))) return true;
                    break;
                case InputType.Key:
                    if ((jumpKey != KeyCode.None) && (Input.GetKey(jumpKey))) return true;
                    break;
                default:
                    break;
            }

            return false;
        }

        bool GetGlidePressed()
        {
            switch (glideInputType)
            {
                case InputType.Axis:
                    if (glideAxis != "") return Input.GetAxis(glideAxis) > epsilonZero;
                    break;
                case InputType.Button:
                    if ((glideButton != "") && (Input.GetButton(glideButton))) return true;
                    break;
                case InputType.Key:
                    if ((glideKey != KeyCode.None) && (Input.GetKey(glideKey))) return true;
                    break;
                default:
                    break;
            }

            return false;
        }
        void Update()
        {
            if (coyoteTimer > 0)
            {
                coyoteTimer -= Time.deltaTime;
            }

            float deltaX = 0.0f;

            UpdateGroundState();

            if ((enableAirControl) || (isGrounded))
            {
                switch (horizontalInputType)
                {
                    case InputType.Axis:
                        if (horizontalAxis != "") deltaX = Input.GetAxis(horizontalAxis) * speed.x;
                        break;
                    case InputType.Button:
                        if ((horizontalButtonPositive != "") && (Input.GetButton(horizontalButtonPositive))) deltaX = speed.x;
                        if ((horizontalButtonNegative != "") && (Input.GetButton(horizontalButtonNegative))) deltaX = -speed.x;
                        break;
                    case InputType.Key:
                        if ((horizontalKeyPositive != KeyCode.None) && (Input.GetKey(horizontalKeyPositive))) deltaX = speed.x;
                        if ((horizontalKeyNegative != KeyCode.None) && (Input.GetKey(horizontalKeyNegative))) deltaX = -speed.x;
                        break;
                    default:
                        break;
                }

                rb.velocity = new Vector2(deltaX, rb.velocity.y);
            }

            // Need to check with actual is grounded or else coyote time will make the jump count reset immediately after flying off
            if (actualIsGrounded)
            {
                rb.gravityScale = 0.0f;
                currentJumpCount = maxJumpCount;
                if (airCollider) airCollider.enabled = false;
                if (groundCollider) groundCollider.enabled = true;
                glideTimer = glideMaxTime;
            }
            else
            {
                rb.gravityScale = gravityScale;
                if (airCollider) airCollider.enabled = true;
                if (groundCollider) groundCollider.enabled = false;
            }

            var currentVelocity = rb.velocity;

            if ((useAnimator) && (animator))
            {
                if (horizontalVelocityParameter != "") animator.SetFloat(horizontalVelocityParameter, currentVelocity.x);
                if (absoluteHorizontalVelocityParameter != "") animator.SetFloat(absoluteHorizontalVelocityParameter, Mathf.Abs(currentVelocity.x));
                if (verticalVelocityParameter != "") animator.SetFloat(verticalVelocityParameter, currentVelocity.y);
                if (absoluteVerticalVelocityParameter != "") animator.SetFloat(absoluteVerticalVelocityParameter, Mathf.Abs(currentVelocity.y));
                if (isGroundedParameter != "") animator.SetBool(isGroundedParameter, actualIsGrounded);
                if (isGlidingParameter != "") animator.SetBool(isGlidingParameter, isGliding);
            }

            switch (flipBehaviour)
            {
                case FlipBehaviour.None:
                    break;
                case FlipBehaviour.VelocityFlipsSprite:
                    if (currentVelocity.x > epsilonZero) spriteRenderer.flipX = false;
                    else if (currentVelocity.x < -epsilonZero) spriteRenderer.flipX = true;
                    break;
                case FlipBehaviour.VelocityInvertsScale:
                    if ((currentVelocity.x > epsilonZero) && (transform.localScale.x < 0.0f)) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    else if ((currentVelocity.x < -epsilonZero) && (transform.localScale.x > 0.0f)) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    break;
                case FlipBehaviour.InputFlipsSprite:
                    if (deltaX > epsilonZero) spriteRenderer.flipX = false;
                    else if (deltaX < -epsilonZero) spriteRenderer.flipX = true;
                    break;
                case FlipBehaviour.InputInvertsScale:
                    if ((deltaX > epsilonZero) && (transform.localScale.x < 0.0f)) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    else if ((deltaX < -epsilonZero) && (transform.localScale.x > 0.0f)) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    break;
                default:
                    break;
            }
        }

        void UpdateGroundState()
        {
            if (groundCheckCollider)
            {
                ContactFilter2D contactFilter = new ContactFilter2D();
                contactFilter.useTriggers = true;
                contactFilter.useLayerMask = true;
                contactFilter.layerMask = groundLayerMask;

                Collider2D[] results = new Collider2D[128];

                int n = Physics2D.OverlapCollider(groundCheckCollider, contactFilter, results);
                if (n > 0)
                {
                    actualIsGrounded = true;
                    isGrounded = true;
                    return;
                }
            }

            if (actualIsGrounded)
            {
                coyoteTimer = coyoteTime;
            }

            actualIsGrounded = false;

            if (coyoteTimer > 0)
            {
                isGrounded = true;
                return;
            }

            isGrounded = false;
        }
    }
}