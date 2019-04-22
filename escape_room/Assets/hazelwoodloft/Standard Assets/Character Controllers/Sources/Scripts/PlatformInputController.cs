using UnityEngine;
using System.Collections;

[System.Serializable]
// This makes the character turn to face the current movement speed per default.
// Use this for initialization
// Update is called once per frame
// Get the input vector from kayboard or analog stick
// Get the length of the directon vector and then normalize it
// Dividing by the length is cheaper than normalizing when we already have the length anyway
// Make sure the length is no bigger than 1
// Make the input vector more sensitive towards the extremes and less sensitive in the middle
// This makes it easier to control slow speeds when using analog sticks
// Multiply the normalized direction vector by the modified length
// Rotate the input vector into camera space so up is camera's up and right is camera's right
// Rotate input vector to be perpendicular to character's up vector
// Apply the direction to the CharacterMotor
// Set rotation to the move direction	
// Require a character controller to be attached to the same game object
[UnityEngine.RequireComponent(typeof(CharacterMotor))]
[UnityEngine.AddComponentMenu("Character/Platform Input Controller")]
public partial class PlatformInputController : MonoBehaviour
{
    public bool autoRotate;

    public float maxRotationSpeed;

    private CharacterMotor motor;

    public virtual void Awake()
    {
        this.motor = (CharacterMotor) this.GetComponent(typeof(CharacterMotor));
    }

    public virtual void Update()
    {
        Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if (directionVector != Vector3.zero)
        {
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;
            directionLength = Mathf.Min(1, directionLength);
            directionLength = directionLength * directionLength;
            directionVector = directionVector * directionLength;
        }
        directionVector = Camera.main.transform.rotation * directionVector;
        Quaternion camToCharacterSpace = Quaternion.FromToRotation(-Camera.main.transform.forward, this.transform.up);
        directionVector = camToCharacterSpace * directionVector;
        this.motor.inputMoveDirection = directionVector;
        this.motor.inputJump = Input.GetButton("Jump");
        if (this.autoRotate && (directionVector.sqrMagnitude > 0.01f))
        {
            Vector3 newForward = this.ConstantSlerp(this.transform.forward, directionVector, this.maxRotationSpeed * Time.deltaTime);
            newForward = this.ProjectOntoPlane(newForward, this.transform.up);
            this.transform.rotation = Quaternion.LookRotation(newForward, this.transform.up);
        }
    }

    public virtual Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
    {
        return v - Vector3.Project(v, normal);
    }

    public virtual Vector3 ConstantSlerp(Vector3 from, Vector3 to, float angle)
    {
        float value = Mathf.Min(1, angle / Vector3.Angle(from, to));
        return Vector3.Slerp(from, to, value);
    }

    public PlatformInputController()
    {
        this.autoRotate = true;
        this.maxRotationSpeed = 360;
    }

}