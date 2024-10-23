using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    public float moveSpeed = 5.0f;  // Base movement speed
    public float jumpForce = 10.0f;  // Force applied when jumping
    public Transform vrCameraTransform;  // Reference to the VR camera (headset)
    public LayerMask groundLayers;  // Layers considered as ground
    public float groundCheckDistance = 0.5f;  // Distance to check for ground

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 input;  // Input from the left analog stick

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

        // Handle movement
        HandleMovement();
    }

    void HandleMovement()
    {
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

            // Normalize to keep the directions consistent
            forward.Normalize();
            right.Normalize();

            // Calculate the movement direction based on the joystick input and headset direction
            Vector3 moveDirection = (forward * localInput.z + right * localInput.x).normalized;

            // Scale movement based on the joystick input magnitude
            float inputMagnitude = input.magnitude;

            // Calculate movement vector
            Vector3 movement = moveDirection * moveSpeed * inputMagnitude * Time.fixedDeltaTime;

            // Move the capsule using Rigidbody
            rb.MovePosition(rb.position + movement);
        }
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
