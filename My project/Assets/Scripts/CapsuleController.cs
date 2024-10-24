using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    public float moveSpeed = 5.0f;  // Max movement speed
    public float acceleration = 10.0f;  // How fast the capsule reaches max speed
    public float deceleration = 8.0f;  // How fast the capsule slows down
    public float jumpForce = 10.0f;  // Force applied when jumping
    public Transform vrCameraTransform;  // Reference to the VR camera (headset)
    public LayerMask groundLayers;  // Layers considered as ground
    public float groundCheckDistance = 0.5f;  // Distance to check for ground

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 input;  // Input from the left analog stick
    private Vector3 currentVelocity;  // Track the current velocity

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Ensure the vrCameraTransform is assigned
        if (vrCameraTransform == null)
        {
            Debug.LogError("VR Camera Transform is not assigned. Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        // Get analog stick input from the left joystick
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Store the input vector
        input = new Vector2(horizontalInput, verticalInput);

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Update grounded status
        isGrounded = CheckIfGrounded();

        // Handle movement with inertia and smoothing
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 desiredVelocity = Vector3.zero;

        if (input.sqrMagnitude > 0.01f)  // Threshold to prevent jitter when the stick is near the center
        {
            // Create the input vector in local space (joystick direction)
            Vector3 localInput = new Vector3(input.x, 0f, input.y);

            // Use the VR camera's (headset's) forward direction for movement
            Vector3 forward = vrCameraTransform.forward;
            Vector3 right = vrCameraTransform.right;

            // Ignore vertical tilt (y-component)
            forward.y = 0f;
            right.y = 0f;

            // Normalize directions
            forward.Normalize();
            right.Normalize();

            // Calculate the movement direction based on the joystick input and headset direction
            Vector3 moveDirection = (forward * localInput.z + right * localInput.x).normalized;

            // Calculate the desired velocity
            desiredVelocity = moveDirection * moveSpeed;
        }

        // Smoothly adjust velocity based on input
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity,
            (desiredVelocity.magnitude > 0) ? acceleration * Time.fixedDeltaTime : deceleration * Time.fixedDeltaTime);

        // Move the capsule by applying the calculated velocity
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void Jump()
    {
        // Apply upward force for jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    bool CheckIfGrounded()
    {
        // Raycast down to check if the capsule is grounded
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayers);
    }
}
