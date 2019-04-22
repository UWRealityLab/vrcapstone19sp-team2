using UnityEngine;
using System.Collections;

// Does this script currently respond to input?
// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
// Very handy for organization!
// The current global direction we want the character to move in.
// Is the jump button held down? We use this interface instead of checking
// for the jump button directly so this script can also be used by AIs.
[System.Serializable]
public class CharacterMotorMovement : object
{
    // The maximum horizontal speed when moving
    public float maxForwardSpeed;

    public float maxSidewaysSpeed;

    public float maxBackwardsSpeed;

    // Curve for multiplying speed based on slope (negative = downwards)
    public AnimationCurve slopeSpeedMultiplier;

    // How fast does the character change speeds?  Higher is faster.
    public float maxGroundAcceleration;

    public float maxAirAcceleration;

    // The gravity for the character
    public float gravity;

    public float maxFallSpeed;

    // For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!
    // The last collision flags returned from controller.Move
    [System.NonSerialized]
    public CollisionFlags collisionFlags;

    // We will keep track of the character's current velocity,
    [System.NonSerialized]
    public Vector3 velocity;

    // This keeps track of our current velocity while we're not grounded
    [System.NonSerialized]
    public Vector3 frameVelocity;

    [System.NonSerialized]
    public Vector3 hitPoint;

    [System.NonSerialized]
    public Vector3 lastHitPoint;

    public CharacterMotorMovement()
    {
        this.maxForwardSpeed = 10f;
        this.maxSidewaysSpeed = 10f;
        this.maxBackwardsSpeed = 10f;
        this.slopeSpeedMultiplier = new AnimationCurve(new Keyframe[] {new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0)});
        this.maxGroundAcceleration = 30f;
        this.maxAirAcceleration = 20f;
        this.gravity = 10f;
        this.maxFallSpeed = 20f;
        this.frameVelocity = Vector3.zero;
        this.hitPoint = Vector3.zero;
        this.lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
    }

}
public enum MovementTransferOnJump
{
    None = 0, // The jump is not affected by velocity of floor at all.
    InitTransfer = 1, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
    PermaTransfer = 2, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
    PermaLocked = 3 // Jump is relative to the movement of the last touched floor and will move together with that floor.
}

// We will contain all the jumping related variables in one helper class for clarity.
[System.Serializable]
public class CharacterMotorJumping : object
{
    // Can the character jump?
    public bool enabled;

    // How high do we jump when pressing jump and letting go immediately
    public float baseHeight;

    // We add extraHeight units (meters) on top when holding the button down longer while jumping
    public float extraHeight;

    // How much does the character jump out perpendicular to the surface on walkable surfaces?
    // 0 means a fully vertical jump and 1 means fully perpendicular.
    public float perpAmount;

    // How much does the character jump out perpendicular to the surface on too steep surfaces?
    // 0 means a fully vertical jump and 1 means fully perpendicular.
    public float steepPerpAmount;

    // For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!
    // Are we jumping? (Initiated with jump button and not grounded yet)
    // To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
    [System.NonSerialized]
    public bool jumping;

    [System.NonSerialized]
    public bool holdingJumpButton;

    // the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
    [System.NonSerialized]
    public float lastStartTime;

    [System.NonSerialized]
    public float lastButtonDownTime;

    [System.NonSerialized]
    public Vector3 jumpDir;

    public CharacterMotorJumping()
    {
        this.enabled = true;
        this.baseHeight = 1f;
        this.extraHeight = 4.1f;
        this.steepPerpAmount = 0.5f;
        this.lastButtonDownTime = -100;
        this.jumpDir = Vector3.up;
    }

}
[System.Serializable]
public class CharacterMotorMovingPlatform : object
{
    public bool enabled;

    public MovementTransferOnJump movementTransfer;

    [System.NonSerialized]
    public Transform hitPlatform;

    [System.NonSerialized]
    public Transform activePlatform;

    [System.NonSerialized]
    public Vector3 activeLocalPoint;

    [System.NonSerialized]
    public Vector3 activeGlobalPoint;

    [System.NonSerialized]
    public Quaternion activeLocalRotation;

    [System.NonSerialized]
    public Quaternion activeGlobalRotation;

    [System.NonSerialized]
    public Matrix4x4 lastMatrix;

    [System.NonSerialized]
    public Vector3 platformVelocity;

    [System.NonSerialized]
    public bool newPlatform;

    public CharacterMotorMovingPlatform()
    {
        this.enabled = true;
        this.movementTransfer = MovementTransferOnJump.PermaTransfer;
    }

}
[System.Serializable]
public class CharacterMotorSliding : object
{
    // Does the character slide on too steep surfaces?
    public bool enabled;

    // How fast does the character slide on steep surfaces?
    public float slidingSpeed;

    // How much can the player control the sliding direction?
    // If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
    public float sidewaysControl;

    // How much can the player influence the sliding speed?
    // If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
    public float speedControl;

    public CharacterMotorSliding()
    {
        this.enabled = true;
        this.slidingSpeed = 15;
        this.sidewaysControl = 1f;
        this.speedControl = 0.4f;
    }

}
[System.Serializable]
// We copy the actual velocity into a temporary variable that we can manipulate.
// Update velocity based on input
// Apply gravity and jumping force
// Moving platform support
// Support moving platform rotation as well:
// Prevent rotation of the local up vector
// Save lastPosition for velocity calculation.
// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
// Find out how much we need to push towards the ground to avoid loosing grouning
// when walking down a step or over a sharp change in slope.
// Reset variables that will be set by collision function
// Move our character!
// Calculate the velocity based on the current and previous position.  
// This means our velocity will only be the amount the character actually moved as a result of collisions.
// The CharacterController can be moved in unwanted directions when colliding with things.
// We want to prevent this from influencing the recorded velocity.
// Something is forcing the CharacterController down faster than it should.
// Ignore this
// The upwards movement of the CharacterController has been blocked.
// This is treated like a ceiling collision - stop further jumping here.
// We were grounded but just loosed grounding
// Apply inertia from platform
// We pushed the character down to ensure it would stay on the ground if there was any.
// But there wasn't so now we cancel the downwards offset to make the fall smoother.
// We were not grounded but just landed on something
// Moving platforms support
// Use the center of the lower half sphere of the capsule as reference point.
// This works best when the character is standing on moving tilting platforms. 
// Support moving platform rotation as well:
// Find desired velocity
// The direction we're sliding in
// Find the input movement direction projected onto the sliding direction
// Add the sliding direction, the spped control, and the sideways control vectors
// Multiply with the sliding speed
// Enforce max velocity change
// If we're in the air and don't have control, don't apply any velocity change at all.
// If we're on the ground and don't have control we do apply it - it will correspond to friction.
// When going uphill, the CharacterController will automatically move up by the needed amount.
// Not moving it upwards manually prevent risk of lifting off from the ground.
// When going downhill, DO move down manually, as gravity is not enough on steep hills.
// When jumping up we don't apply gravity for some time when the user is holding the jump button.
// This gives more control over jump height by pressing the button longer.
// Calculate the duration that the extra jump force should have effect.
// If we're still less than that duration after the jumping time, apply the force.
// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
// Jump only if the jump button was pressed down in the last 0.2 seconds.
// We use this check instead of checking if it's pressed down right now
// because players will often try to jump in the exact moment when hitting the ground after a jump
// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
// it's confusing and it feels like the game is buggy.
// Calculate the jumping direction
// Apply the jumping force to the velocity. Cancel any vertical velocity first.
// Apply inertia from platform
// When landing, subtract the velocity of the new ground from the character's velocity
// since movement in ground is relative to the movement of the ground.
 // If we landed on a new platform, we have to wait for two FixedUpdates
 // before we know the velocity of the platform under the character
// Find desired velocity
// Modify max speed on slopes based on slope speed multiplier curve
// Maximum acceleration on ground and in air
// From the jump height and gravity we deduce the upwards speed 
// for the character to reach at the apex.
// Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
// The function returns the length of the resulting vector.
// Require a character controller to be attached to the same game object
[UnityEngine.RequireComponent(typeof(CharacterController))]
[UnityEngine.AddComponentMenu("Character/Character Motor")]
public partial class CharacterMotor : MonoBehaviour
{
    public bool canControl;

    public bool useFixedUpdate;

    [System.NonSerialized]
    public Vector3 inputMoveDirection;

    [System.NonSerialized]
    public bool inputJump;

    public CharacterMotorMovement movement;

    public CharacterMotorJumping jumping;

    public CharacterMotorMovingPlatform movingPlatform;

    public CharacterMotorSliding sliding;

    [System.NonSerialized]
    public bool grounded;

    [System.NonSerialized]
    public Vector3 groundNormal;

    private Vector3 lastGroundNormal;

    private Transform tr;

    private CharacterController controller;

    public virtual void Awake()
    {
        this.controller = (CharacterController) this.GetComponent(typeof(CharacterController));
        this.tr = this.transform;
    }

    private void UpdateFunction()
    {
        Vector3 velocity = this.movement.velocity;
        velocity = this.ApplyInputVelocityChange(velocity);
        velocity = this.ApplyGravityAndJumping(velocity);
        Vector3 moveDistance = Vector3.zero;
        if (this.MoveWithPlatform())
        {
            Vector3 newGlobalPoint = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint);
            moveDistance = newGlobalPoint - this.movingPlatform.activeGlobalPoint;
            if (moveDistance != Vector3.zero)
            {
                this.controller.Move(moveDistance);
            }
            Quaternion newGlobalRotation = this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocalRotation;
            Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(this.movingPlatform.activeGlobalRotation);
            float yRotation = rotationDiff.eulerAngles.y;
            if (yRotation != 0)
            {
                this.tr.Rotate(0, yRotation, 0);
            }
        }
        Vector3 lastPosition = this.tr.position;
        Vector3 currentMovementOffset = velocity * Time.deltaTime;
        float pushDownOffset = Mathf.Max(this.controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
        if (this.grounded)
        {
            currentMovementOffset = currentMovementOffset - (pushDownOffset * Vector3.up);
        }
        this.movingPlatform.hitPlatform = null;
        this.groundNormal = Vector3.zero;
        this.movement.collisionFlags = this.controller.Move(currentMovementOffset);
        this.movement.lastHitPoint = this.movement.hitPoint;
        this.lastGroundNormal = this.groundNormal;
        if (this.movingPlatform.enabled && (this.movingPlatform.activePlatform != this.movingPlatform.hitPlatform))
        {
            if (this.movingPlatform.hitPlatform != null)
            {
                this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
                this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
                this.movingPlatform.newPlatform = true;
            }
        }
        Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
        this.movement.velocity = (this.tr.position - lastPosition) / Time.deltaTime;
        Vector3 newHVelocity = new Vector3(this.movement.velocity.x, 0, this.movement.velocity.z);
        if (oldHVelocity == Vector3.zero)
        {
            this.movement.velocity = new Vector3(0, this.movement.velocity.y, 0);
        }
        else
        {
            float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
            this.movement.velocity = (oldHVelocity * Mathf.Clamp01(projectedNewVelocity)) + (this.movement.velocity.y * Vector3.up);
        }
        if (this.movement.velocity.y < (velocity.y - 0.001f))
        {
            if (this.movement.velocity.y < 0)
            {
                this.movement.velocity.y = velocity.y;
            }
            else
            {
                this.jumping.holdingJumpButton = false;
            }
        }
        if (this.grounded && !this.IsGroundedTest())
        {
            this.grounded = false;
            if (this.movingPlatform.enabled && ((this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
            {
                this.movement.frameVelocity = this.movingPlatform.platformVelocity;
                this.movement.velocity = this.movement.velocity + this.movingPlatform.platformVelocity;
            }
            this.SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
            this.tr.position = this.tr.position + (pushDownOffset * Vector3.up);
        }
        else
        {
            if (!this.grounded && this.IsGroundedTest())
            {
                this.grounded = true;
                this.jumping.jumping = false;
                this.StartCoroutine(this.SubtractNewPlatformVelocity());
                this.SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
            }
        }
        if (this.MoveWithPlatform())
        {
            this.movingPlatform.activeGlobalPoint = this.tr.position + (Vector3.up * ((this.controller.center.y - (this.controller.height * 0.5f)) + this.controller.radius));
            this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
            this.movingPlatform.activeGlobalRotation = this.tr.rotation;
            this.movingPlatform.activeLocalRotation = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation) * this.movingPlatform.activeGlobalRotation;
        }
    }

    public virtual void FixedUpdate()
    {
        if (this.movingPlatform.enabled)
        {
            if (this.movingPlatform.activePlatform != null)
            {
                if (!this.movingPlatform.newPlatform)
                {

                    this.movingPlatform.platformVelocity = (this.movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint) - this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)) / Time.deltaTime;
                }
                this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
                this.movingPlatform.newPlatform = false;
            }
            else
            {
                this.movingPlatform.platformVelocity = Vector3.zero;
            }
        }
        if (this.useFixedUpdate)
        {
            this.UpdateFunction();
        }
    }

    public virtual void Update()
    {
        if (!this.useFixedUpdate)
        {
            this.UpdateFunction();
        }
    }

    private Vector3 ApplyInputVelocityChange(Vector3 velocity)
    {
        Vector3 desiredVelocity = default(Vector3);
        if (!this.canControl)
        {
            this.inputMoveDirection = Vector3.zero;
        }
        if (this.grounded && this.TooSteep())
        {
            desiredVelocity = new Vector3(this.groundNormal.x, 0, this.groundNormal.z).normalized;
            Vector3 projectedMoveDir = Vector3.Project(this.inputMoveDirection, desiredVelocity);
            desiredVelocity = (desiredVelocity + (projectedMoveDir * this.sliding.speedControl)) + ((this.inputMoveDirection - projectedMoveDir) * this.sliding.sidewaysControl);
            desiredVelocity = desiredVelocity * this.sliding.slidingSpeed;
        }
        else
        {
            desiredVelocity = this.GetDesiredHorizontalVelocity();
        }
        if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
        {
            desiredVelocity = desiredVelocity + this.movement.frameVelocity;
            desiredVelocity.y = 0;
        }
        if (this.grounded)
        {
            desiredVelocity = this.AdjustGroundVelocityToNormal(desiredVelocity, this.groundNormal);
        }
        else
        {
            velocity.y = 0;
        }
        float maxVelocityChange = this.GetMaxAcceleration(this.grounded) * Time.deltaTime;
        Vector3 velocityChangeVector = desiredVelocity - velocity;
        if (velocityChangeVector.sqrMagnitude > (maxVelocityChange * maxVelocityChange))
        {
            velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
        }
        if (this.grounded || this.canControl)
        {
            velocity = velocity + velocityChangeVector;
        }
        if (this.grounded)
        {
            velocity.y = Mathf.Min(velocity.y, 0);
        }
        return velocity;
    }

    private Vector3 ApplyGravityAndJumping(Vector3 velocity)
    {
        if (!this.inputJump || !this.canControl)
        {
            this.jumping.holdingJumpButton = false;
            this.jumping.lastButtonDownTime = -100;
        }
        if ((this.inputJump && (this.jumping.lastButtonDownTime < 0)) && this.canControl)
        {
            this.jumping.lastButtonDownTime = Time.time;
        }
        if (this.grounded)
        {
            velocity.y = Mathf.Min(0, velocity.y) - (this.movement.gravity * Time.deltaTime);
        }
        else
        {
            velocity.y = this.movement.velocity.y - (this.movement.gravity * Time.deltaTime);
            if (this.jumping.jumping && this.jumping.holdingJumpButton)
            {
                if (Time.time < (this.jumping.lastStartTime + (this.jumping.extraHeight / this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))))
                {
                    velocity = velocity + ((this.jumping.jumpDir * this.movement.gravity) * Time.deltaTime);
                }
            }
            velocity.y = Mathf.Max(velocity.y, -this.movement.maxFallSpeed);
        }
        if (this.grounded)
        {
            if ((this.jumping.enabled && this.canControl) && ((Time.time - this.jumping.lastButtonDownTime) < 0.2f))
            {
                this.grounded = false;
                this.jumping.jumping = true;
                this.jumping.lastStartTime = Time.time;
                this.jumping.lastButtonDownTime = -100;
                this.jumping.holdingJumpButton = true;
                if (this.TooSteep())
                {
                    this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.steepPerpAmount);
                }
                else
                {
                    this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.perpAmount);
                }
                velocity.y = 0;
                velocity = velocity + (this.jumping.jumpDir * this.CalculateJumpVerticalSpeed(this.jumping.baseHeight));
                if (this.movingPlatform.enabled && ((this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
                {
                    this.movement.frameVelocity = this.movingPlatform.platformVelocity;
                    velocity = velocity + this.movingPlatform.platformVelocity;
                }
                this.SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                this.jumping.holdingJumpButton = false;
            }
        }
        return velocity;
    }

    public virtual void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((hit.normal.y > 0) && (hit.normal.y > this.groundNormal.y)) && (hit.moveDirection.y < 0))
        {
            if (((hit.point - this.movement.lastHitPoint).sqrMagnitude > 0.001f) || (this.lastGroundNormal == Vector3.zero))
            {
                this.groundNormal = hit.normal;
            }
            else
            {
                this.groundNormal = this.lastGroundNormal;
            }
            this.movingPlatform.hitPlatform = hit.collider.transform;
            this.movement.hitPoint = hit.point;
            this.movement.frameVelocity = Vector3.zero;
        }
    }

    private IEnumerator SubtractNewPlatformVelocity()
    {
        if (this.movingPlatform.enabled && ((this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
        {
            if (this.movingPlatform.newPlatform)
            {
                Transform platform = this.movingPlatform.activePlatform;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                if (this.grounded && (platform == this.movingPlatform.activePlatform))
                {
                    yield return 1;
                }
            }
            this.movement.velocity = this.movement.velocity - this.movingPlatform.platformVelocity;
        }
    }

    private bool MoveWithPlatform()
    {
        return (this.movingPlatform.enabled && (this.grounded || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked))) && (this.movingPlatform.activePlatform != null);
    }

    private Vector3 GetDesiredHorizontalVelocity()
    {
        Vector3 desiredLocalDirection = this.tr.InverseTransformDirection(this.inputMoveDirection);
        float maxSpeed = this.MaxSpeedInDirection(desiredLocalDirection);
        if (this.grounded)
        {
            float movementSlopeAngle = Mathf.Asin(this.movement.velocity.normalized.y) * Mathf.Rad2Deg;
            maxSpeed = maxSpeed * this.movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
        }
        return this.tr.TransformDirection(desiredLocalDirection * maxSpeed);
    }

    private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
    {
        Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
        return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
    }

    private bool IsGroundedTest()
    {
        return this.groundNormal.y > 0.01f;
    }

    public virtual float GetMaxAcceleration(bool grounded)
    {
        if (grounded)
        {
            return this.movement.maxGroundAcceleration;
        }
        else
        {
            return this.movement.maxAirAcceleration;
        }
    }

    public virtual float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        return Mathf.Sqrt((2 * targetJumpHeight) * this.movement.gravity);
    }

    public virtual bool IsJumping()
    {
        return this.jumping.jumping;
    }

    public virtual bool IsSliding()
    {
        return (this.grounded && this.sliding.enabled) && this.TooSteep();
    }

    public virtual bool IsTouchingCeiling()
    {
        return (this.movement.collisionFlags & CollisionFlags.CollidedAbove) != (CollisionFlags) 0;
    }

    public virtual bool IsGrounded()
    {
        return this.grounded;
    }

    public virtual bool TooSteep()
    {
        return this.groundNormal.y <= Mathf.Cos(this.controller.slopeLimit * Mathf.Deg2Rad);
    }

    public virtual Vector3 GetDirection()
    {
        return this.inputMoveDirection;
    }

    public virtual void SetControllable(bool controllable)
    {
        this.canControl = controllable;
    }

    public virtual float MaxSpeedInDirection(Vector3 desiredMovementDirection)
    {
        if (desiredMovementDirection == Vector3.zero)
        {
            return 0;
        }
        else
        {
            float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? this.movement.maxForwardSpeed : this.movement.maxBackwardsSpeed) / this.movement.maxSidewaysSpeed;
            Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
            float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * this.movement.maxSidewaysSpeed;
            return length;
        }
    }

    public virtual void SetVelocity(Vector3 velocity)
    {
        this.grounded = false;
        this.movement.velocity = velocity;
        this.movement.frameVelocity = Vector3.zero;
        this.SendMessage("OnExternalVelocity");
    }

    public CharacterMotor()
    {
        this.canControl = true;
        this.useFixedUpdate = true;
        this.inputMoveDirection = Vector3.zero;
        this.movement = new CharacterMotorMovement();
        this.jumping = new CharacterMotorJumping();
        this.movingPlatform = new CharacterMotorMovingPlatform();
        this.sliding = new CharacterMotorSliding();
        this.grounded = true;
        this.groundNormal = Vector3.zero;
        this.lastGroundNormal = Vector3.zero;
    }

}