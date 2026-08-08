using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float targetSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField] bool readyToJump = true;
    [SerializeField] bool isSprinting = false;

    public Transform orientation;

    Vector3 moveDirection;

    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;

    [SerializeField] Animator animator;

    enum AnimationState
    {
        Falling,
        Sprint,
        Walk,
        Idle
    }

    [SerializeField] AnimationState state;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    [SerializeField] bool isGrounded;

    public void OnSpawn()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        InputHandler.instance.OnJump += Jump;
        InputHandler.instance.OnSprint += OnSprint;
        InputHandler.instance.OnSprintCancel += OnSprintCancel;
    }

    private void Awake()
    {
        OnSpawn();
    }
    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        targetSpeed = 0.0f;
        if (!isGrounded)
        {
            state = AnimationState.Falling;
        }
        else if (isSprinting && InputHandler.instance.moveInputValue.magnitude != 0.0f)
        {
            state = AnimationState.Sprint;
            targetSpeed = sprintSpeed;
        }
        else if (InputHandler.instance.moveInputValue.magnitude != 0.0f)
        {
            state = AnimationState.Walk;
            targetSpeed = walkSpeed;
        }
        else
        {
            state = AnimationState.Idle;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnSprint()
    {
        isSprinting = true;
    }
    private void OnSprintCancel()
    {
        isSprinting = false;
    }

    private void MovePlayer()
    {
        float horizontalInput = InputHandler.instance.moveInputValue.x;
        float verticalInput = InputHandler.instance.moveInputValue.y;
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDirection.Normalize();

        animator.SetFloat("Moving", targetSpeed / sprintSpeed);

        RaycastHit hitInfo;
        isGrounded = Physics.SphereCast(transform.position + Vector3.up * (.5f - 0.02f), capsuleCollider.radius, Vector3.down, out hitInfo, capsuleCollider.radius, groundLayer);
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
            rb.AddForce((moveDirection * targetSpeed) * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearDamping = 0;
            rb.AddForce((moveDirection * targetSpeed * airMultiplier) * Time.fixedDeltaTime);
        }
    }

    private void Jump()
    {
        if (isGrounded && readyToJump)
        {
            readyToJump = false;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void OnDrawGizmos()
    {
        RaycastHit hitInfo;
        isGrounded = Physics.SphereCast(transform.position + Vector3.up * (.5f - 0.02f), capsuleCollider.radius, Vector3.down, out hitInfo, capsuleCollider.radius, groundLayer);
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (capsuleCollider.radius - 0.02f), capsuleCollider.radius);
    }
}
