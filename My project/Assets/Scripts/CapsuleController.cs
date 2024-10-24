using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float acceleration = 10.0f;
    public float deceleration = 8.0f;
    public float jumpForce = 10.0f;
    public Transform vrCameraTransform;
    public LayerMask groundLayers;
    public float groundCheckDistance = 0.5f;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 input;
    private Vector3 currentVelocity;
    private bool canMove = true;  // Control whether movement is allowed

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //if (vrCameraTransform == null)
        //{
        //    Debug.LogError("VR Camera Transform is not assigned.");
        //}

        // Subscribe to the GameOver event
        GameManager.Instance.OnGameOver.AddListener(DisableMovement);
    }

    void OnDestroy()
    {
        // Unsubscribe from the GameOver event to avoid potential memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver.RemoveListener(DisableMovement);
        }
    }

    void Update()
    {
        if (!canMove) return;  // Prevent movement if movement is disabled

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        input = new Vector2(horizontalInput, verticalInput);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;  // Prevent movement if movement is disabled

        isGrounded = CheckIfGrounded();
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 desiredVelocity = Vector3.zero;

        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 localInput = new Vector3(input.x, 0f, input.y);
            Vector3 forward = vrCameraTransform.forward;
            Vector3 right = vrCameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = (forward * localInput.z + right * localInput.x).normalized;
            float inputMagnitude = input.magnitude;
            desiredVelocity = moveDirection * moveSpeed * inputMagnitude;
        }

        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity,
            (desiredVelocity.magnitude > 0) ? acceleration * Time.fixedDeltaTime : deceleration * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayers);
    }

    // Disable movement when the game is over
    public void DisableMovement()
    {
        canMove = false;
    }

    // Enable movement when the game restarts (optional if needed)
    public void EnableMovement()
    {
        canMove = true;
    }
}
