using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
public enum CharacterState
{
    Idle = 0,
    Walking = 1,
    Trotting = 2,
    Running = 3,
    Jumping = 4
}

[System.Serializable]
[UnityEngine.RequireComponent(typeof(CharacterController))]
public partial class ThirdPersonController : MonoBehaviour
{
    public AnimationClip idleAnimation;

    public AnimationClip walkAnimation;

    public AnimationClip runAnimation;

    public AnimationClip jumpPoseAnimation;

    public float walkMaxAnimationSpeed;

    public float trotMaxAnimationSpeed;

    public float runMaxAnimationSpeed;

    public float jumpAnimationSpeed;

    public float landAnimationSpeed;

    private Animation _animation;

    private CharacterState _characterState;

    // The speed when walking
    public float walkSpeed;

    // after trotAfterSeconds of walking we trot with trotSpeed
    public float trotSpeed;

    // when pressing "Fire3" button (cmd) we start running
    public float runSpeed;

    public float inAirControlAcceleration;

    // How high do we jump when pressing jump and letting go immediately
    public float jumpHeight;

    // The gravity for the character
    public float gravity;

    // The gravity in controlled descent mode
    public float speedSmoothing;

    public float rotateSpeed;

    public float trotAfterSeconds;

    public bool canJump;

    private float jumpRepeatTime;

    private float jumpTimeout;

    private float groundedTimeout;

    // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
    private float lockCameraTimer;

    // The current move direction in x-z
    private Vector3 moveDirection;

    // The current vertical speed
    private float verticalSpeed;

    // The current x-z move speed
    private float moveSpeed;

    // The last collision flags returned from controller.Move
    private CollisionFlags collisionFlags;

    // Are we jumping? (Initiated with jump button and not grounded yet)
    private bool jumping;

    private bool jumpingReachedApex;

    // Are we moving backwards (This locks the camera to not do a 180 degree spin)
    private bool movingBack;

    // Is the user pressing any keys?
    private bool isMoving;

    // When did the user start walking (Used for going into trot after a while)
    private float walkTimeStart;

    // Last time the jump button was clicked down
    private float lastJumpButtonTime;

    // Last time we performed a jump
    private float lastJumpTime;

    // the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)


    private Vector3 inAirVelocity;

    private float lastGroundedTime;

    private bool isControllable;

    public virtual void Awake()
    {
        this.moveDirection = this.transform.TransformDirection(Vector3.forward);
        this._animation = (Animation) this.GetComponent(typeof(Animation));
        if (!this._animation)
        {
            Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
        }
        /*
public var idleAnimation : AnimationClip;
public var walkAnimation : AnimationClip;
public var runAnimation : AnimationClip;
public var jumpPoseAnimation : AnimationClip;	
	*/
        if (!this.idleAnimation)
        {
            this._animation = null;
            Debug.Log("No idle animation found. Turning off animations.");
        }
        if (!this.walkAnimation)
        {
            this._animation = null;
            Debug.Log("No walk animation found. Turning off animations.");
        }
        if (!this.runAnimation)
        {
            this._animation = null;
            Debug.Log("No run animation found. Turning off animations.");
        }
        if (!this.jumpPoseAnimation && this.canJump)
        {
            this._animation = null;
            Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
        }
    }

    public virtual void UpdateSmoothedMovementDirection()
    {
        Transform cameraTransform = Camera.main.transform;
        bool grounded = this.IsGrounded();
        // Forward vector relative to the camera along the x-z plane	
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        // Right vector relative to the camera
        // Always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        // Are we moving backwards or looking backwards
        if (v < -0.2f)
        {
            this.movingBack = true;
        }
        else
        {
            this.movingBack = false;
        }
        bool wasMoving = this.isMoving;
        this.isMoving = (Mathf.Abs(h) > 0.1f) || (Mathf.Abs(v) > 0.1f);
        // Target direction relative to the camera
        Vector3 targetDirection = (h * right) + (v * forward);
        // Grounded controls
        if (grounded)
        {
             // Lock camera for short period when transitioning moving & standing still
            this.lockCameraTimer = this.lockCameraTimer + Time.deltaTime;
            if (this.isMoving != wasMoving)
            {
                this.lockCameraTimer = 0f;
            }
            // We store speed and direction seperately,
            // so that when the character stands still we still have a valid forward direction
            // moveDirection is always normalized, and we only update it if there is user input.
            if (targetDirection != Vector3.zero)
            {
                 // If we are really slow, just snap to the target direction
                if ((this.moveSpeed < (this.walkSpeed * 0.9f)) && grounded)
                {
                    this.moveDirection = targetDirection.normalized;
                }
                else
                {
                    // Otherwise smoothly turn towards it
                    this.moveDirection = Vector3.RotateTowards(this.moveDirection, targetDirection, (this.rotateSpeed * Mathf.Deg2Rad) * Time.deltaTime, 1000);
                    this.moveDirection = this.moveDirection.normalized;
                }
            }
            // Smooth the speed based on the current target direction
            float curSmooth = this.speedSmoothing * Time.deltaTime;
            // Choose target speed
            //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
            float targetSpeed = Mathf.Min(targetDirection.magnitude, 1f);
            this._characterState = CharacterState.Idle;
            // Pick speed modifier
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                targetSpeed = targetSpeed * this.runSpeed;
                this._characterState = CharacterState.Running;
            }
            else
            {
                if ((Time.time - this.trotAfterSeconds) > this.walkTimeStart)
                {
                    targetSpeed = targetSpeed * this.trotSpeed;
                    this._characterState = CharacterState.Trotting;
                }
                else
                {
                    targetSpeed = targetSpeed * this.walkSpeed;
                    this._characterState = CharacterState.Walking;
                }
            }
            this.moveSpeed = Mathf.Lerp(this.moveSpeed, targetSpeed, curSmooth);
            // Reset walk time start when we slow down
            if (this.moveSpeed < (this.walkSpeed * 0.3f))
            {
                this.walkTimeStart = Time.time;
            }
        }
        else
        {
            // In air controls
             // Lock camera while in air
            if (this.jumping)
            {
                this.lockCameraTimer = 0f;
            }
            if (this.isMoving)
            {
                this.inAirVelocity = this.inAirVelocity + ((targetDirection.normalized * Time.deltaTime) * this.inAirControlAcceleration);
            }
        }
    }

    public virtual void ApplyJumping()
    {
         // Prevent jumping too fast after each other
        if ((this.lastJumpTime + this.jumpRepeatTime) > Time.time)
        {
            return;
        }
        if (this.IsGrounded())
        {
            // Jump
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing		
            if (this.canJump && (Time.time < (this.lastJumpButtonTime + this.jumpTimeout)))
            {
                this.verticalSpeed = this.CalculateJumpVerticalSpeed(this.jumpHeight);
                this.SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public virtual void ApplyGravity()
    {
        if (this.isControllable) // don't move player at all if not controllable.
        {
             // Apply gravity
          
            // When we reach the apex of the jump we send out a message
            if ((this.jumping && !this.jumpingReachedApex) && (this.verticalSpeed <= 0f))
            {
                this.jumpingReachedApex = true;
                this.SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
            }
            if (this.IsGrounded())
            {
                this.verticalSpeed = 0f;
            }
            else
            {
                this.verticalSpeed = this.verticalSpeed - (this.gravity * Time.deltaTime);
            }
        }
    }

    public virtual float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
         // From the jump height and gravity we deduce the upwards speed 
         // for the character to reach at the apex.
        return Mathf.Sqrt((2 * targetJumpHeight) * this.gravity);
    }

    public virtual void DidJump()
    {
        this.jumping = true;
        this.jumpingReachedApex = false;
        this.lastJumpTime = Time.time;

        this.lastJumpButtonTime = -10;
        this._characterState = CharacterState.Jumping;
    }

    public virtual void Update()
    {
        if (!this.isControllable)
        {
             // kill all inputs if not controllable.
            Input.ResetInputAxes();
        }
        if (Input.GetButtonDown("Jump"))
        {
            this.lastJumpButtonTime = Time.time;
        }
        this.UpdateSmoothedMovementDirection();
        // Apply gravity
        // - extra power jump modifies gravity
        // - controlledDescent mode modifies gravity
        this.ApplyGravity();
        // Apply jumping logic
        this.ApplyJumping();
        // Calculate actual motion
        Vector3 movement = ((this.moveDirection * this.moveSpeed) + new Vector3(0, this.verticalSpeed, 0)) + this.inAirVelocity;
        movement = movement * Time.deltaTime;
        // Move the controller
        CharacterController controller = (CharacterController) this.GetComponent(typeof(CharacterController));
        this.collisionFlags = controller.Move(movement);
        // ANIMATION sector
        if (this._animation)
        {
            if (this._characterState == CharacterState.Jumping)
            {
                if (!this.jumpingReachedApex)
                {
                    this._animation[this.jumpPoseAnimation.name].speed = this.jumpAnimationSpeed;
                    this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                    this._animation.CrossFade(this.jumpPoseAnimation.name);
                }
                else
                {
                    this._animation[this.jumpPoseAnimation.name].speed = -this.landAnimationSpeed;
                    this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                    this._animation.CrossFade(this.jumpPoseAnimation.name);
                }
            }
            else
            {
                if (controller.velocity.sqrMagnitude < 0.1f)
                {
                    this._animation.CrossFade(this.idleAnimation.name);
                }
                else
                {
                    if (this._characterState == CharacterState.Running)
                    {
                        this._animation[this.runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, this.runMaxAnimationSpeed);
                        this._animation.CrossFade(this.runAnimation.name);
                    }
                    else
                    {
                        if (this._characterState == CharacterState.Trotting)
                        {
                            this._animation[this.walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, this.trotMaxAnimationSpeed);
                            this._animation.CrossFade(this.walkAnimation.name);
                        }
                        else
                        {
                            if (this._characterState == CharacterState.Walking)
                            {
                                this._animation[this.walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, this.walkMaxAnimationSpeed);
                                this._animation.CrossFade(this.walkAnimation.name);
                            }
                        }
                    }
                }
            }
        }
        // ANIMATION sector
        // Set rotation to the move direction
        if (this.IsGrounded())
        {
            this.transform.rotation = Quaternion.LookRotation(this.moveDirection);
        }
        else
        {
            Vector3 xzMove = movement;
            xzMove.y = 0;
            if (xzMove.sqrMagnitude > 0.001f)
            {
                this.transform.rotation = Quaternion.LookRotation(xzMove);
            }
        }
        // We are in jump mode but just became grounded
        if (this.IsGrounded())
        {
            this.lastGroundedTime = Time.time;
            this.inAirVelocity = Vector3.zero;
            if (this.jumping)
            {
                this.jumping = false;
                this.SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public virtual void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //	Debug.DrawRay(hit.point, hit.normal);
        if (hit.moveDirection.y > 0.01f)
        {
            return;
        }
    }

    public virtual float GetSpeed()
    {
        return this.moveSpeed;
    }

    public virtual bool IsJumping()
    {
        return this.jumping;
    }

    public virtual bool IsGrounded()
    {
        return (this.collisionFlags & CollisionFlags.CollidedBelow) != (CollisionFlags) 0;
    }

    public virtual Vector3 GetDirection()
    {
        return this.moveDirection;
    }

    public virtual bool IsMovingBackwards()
    {
        return this.movingBack;
    }

    public virtual float GetLockCameraTimer()
    {
        return this.lockCameraTimer;
    }

    public virtual bool IsMoving()
    {
        return (Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal"))) > 0.5f;
    }

    public virtual bool HasJumpReachedApex()
    {
        return this.jumpingReachedApex;
    }

    public virtual bool IsGroundedWithTimeout()
    {
        return (this.lastGroundedTime + this.groundedTimeout) > Time.time;
    }

    public virtual void Reset()
    {
        this.gameObject.tag = "Player";
    }

    public ThirdPersonController()
    {
        this.walkMaxAnimationSpeed = 0.75f;
        this.trotMaxAnimationSpeed = 1f;
        this.runMaxAnimationSpeed = 1f;
        this.jumpAnimationSpeed = 1.15f;
        this.landAnimationSpeed = 1f;
        this.walkSpeed = 2f;
        this.trotSpeed = 4f;
        this.runSpeed = 6f;
        this.inAirControlAcceleration = 3f;
        this.jumpHeight = 0.5f;
        this.gravity = 20f;
        this.speedSmoothing = 10f;
        this.rotateSpeed = 500f;
        this.trotAfterSeconds = 3f;
        this.canJump = true;
        this.jumpRepeatTime = 0.05f;
        this.jumpTimeout = 0.15f;
        this.groundedTimeout = 0.25f;
        this.moveDirection = Vector3.zero;
        this.lastJumpButtonTime = -10f;
        this.lastJumpTime = -1f;
        this.inAirVelocity = Vector3.zero;
        this.isControllable = true;
    }

}