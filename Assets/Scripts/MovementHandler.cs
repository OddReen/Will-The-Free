using Unity.Netcode;
using UnityEngine;

public class MovementHandler : NetworkBehaviour
{
    [SerializeField] InputHandler inputHandler;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField] bool readyToJump = true;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [SerializeField] Animator animator;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    [SerializeField] bool grounded;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        inputHandler.OnJump += Jump;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.1f, whatIsGround);

        Input();
        SpeedControl();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }
    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        MovePlayer();
    }
    private void Input()
    {
        horizontalInput = inputHandler.moveInputValue.x;
        verticalInput = inputHandler.moveInputValue.y;
    }
    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        animator.SetFloat("Move", flatVel.magnitude / moveSpeed);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }


    private void Jump()
    {
        if (grounded && readyToJump)
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
}
