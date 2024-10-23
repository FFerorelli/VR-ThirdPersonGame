using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    // Movement speed of the capsule
    public float moveSpeed = 2.0f;

    // Jump force applied to the capsule
    public float jumpForce = 5.0f;

    // Reference to the VR camera (player's headset)
    public Transform vrCamera;

    // Ground check variables
    public LayerMask groundLayers;
    public float groundCheckDistance = 0.1f;

    // Private variables
    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 input; // Input from joystick

    void Start()
    {
        // Get the Rigidbody component attached to the capsule
        rb = GetComponent<Rigidbody>();

        // Ensure the VR camera is assigned
        if (vrCamera == null)
        {
            vrCamera = Camera.main.transform; // Try to find the main camera
        }
    }

    void Update()
    {
        // Get movement input from the Unity Input Manager (default joystick mapping)
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Check for jump input and if the capsule is grounded
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
        // Get the forward and right vectors relative to the VR camera
        Vector3 forward = vrCamera.forward;
        Vector3 right = vrCamera.right;

        // Project forward and right vectors onto the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate the desired movement direction
        Vector3 desiredMoveDirection = (forward * input.y + right * input.x).normalized;

        // Move the capsule using Rigidbody
        Vector3 movement = desiredMoveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    void Jump()
    {
        // Apply upward force to the capsule for jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    bool CheckIfGrounded()
    {
        // Raycast down to check if the capsule is grounded
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayers);
    }
}
