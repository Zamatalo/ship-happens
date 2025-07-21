using UnityEngine;
using Env;

[RequireComponent(typeof(CharacterController))]
public class CustomGravityController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float gravityStrength = 9.81f;
    public LayerMask groundMask;

    private CharacterController characterController;
    private Vector3 moveInput;
    private Vector3 gravityDirection;
    private float verticalVelocity;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        AlignToGravity();
    }

    private void HandleMovementInput()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        moveInput = transform.right * inputX + transform.forward * inputZ;
        moveInput = moveInput.normalized * moveSpeed;

        // Apply horizontal movement
        Vector3 velocity = moveInput;
        velocity += gravityDirection * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        Asteroid closest = GetClosestAsteroid();
        if (closest != null)
        {
            gravityDirection = -closest.GetGravityDirection(transform.position);
            verticalVelocity += gravityStrength * Time.deltaTime;
        }
        else
        {
            gravityDirection = Vector3.down;
            verticalVelocity += gravityStrength * Time.deltaTime;
        }
    }

    private void AlignToGravity()
    {
        if (gravityDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityDirection) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private Asteroid GetClosestAsteroid()
    {
        Asteroid closest = null;
        float minDist = float.MaxValue;

        foreach (var asteroid in Asteroid.All)
        {
            float dist = Vector3.Distance(transform.position, asteroid.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = asteroid;
            }
        }

        return closest;
    }
}
