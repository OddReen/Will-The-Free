using System;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float targetSpeed;
    public float groundDrag;

    public Vector3 lastVelocity;
    public Vector3 currentVelocity;

    public Vector3 playerVelocity;

    public float movingAlpha = 0.0f;
    public float alphaMultiplier = 0.0f;
    public float gravity = -9.84f;
    public float currentFallSpeed = 0.0f;
    public float maxFallSpeed = 9.8f;

    public float currentDamp;
    public float dampForce;

    public float jumpHeight = 1.5f;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField] bool readyToJump = true;
    [SerializeField] bool isSprinting = false;

    public Transform orientation;

    Vector3 moveDirection;

    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    public CharacterController characterController;

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
    //[SerializeField] bool isGrounded;

    public void OnSpawn()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        characterController = GetComponent<CharacterController>();
        //rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true;

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
        MovePlayerCC();
    }

    private void UpdateAnimationState()
    {
        if (!characterController.isGrounded)
        {
            state = AnimationState.Falling;
        }
        else if (isSprinting && InputHandler.instance.moveInputValue.magnitude != 0.0f)
        {
            state = AnimationState.Sprint;
        }
        else if (InputHandler.instance.moveInputValue.magnitude != 0.0f)
        {
            state = AnimationState.Walk;
        }
        else
        {
            state = AnimationState.Idle;
        }
    }

    private void FixedUpdate()
    {
        //MovePlayer();
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
        //float horizontalInput = InputHandler.instance.moveInputValue.x;
        //float verticalInput = InputHandler.instance.moveInputValue.y;
        //moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        //moveDirection.Normalize();

        //animator.SetFloat("Moving", targetSpeed / sprintSpeed);

        //RaycastHit hitInfo;
        //isGrounded = Physics.SphereCast(transform.position + Vector3.up * (.5f - 0.02f), capsuleCollider.radius, Vector3.down, out hitInfo, capsuleCollider.radius, groundLayer);
        //if (isGrounded)
        //{
        //    rb.linearDamping = groundDrag;
        //    rb.AddForce((moveDirection * targetSpeed) * Time.deltaTime);
        //}
        //else
        //{
        //    rb.linearDamping = 0;
        //    rb.AddForce((moveDirection * targetSpeed * airMultiplier) * Time.deltaTime);
        //}
    }
    private void MovePlayerCC()
    {
        targetSpeed = 0.0f;
        switch (state)
        {
            case AnimationState.Falling:
                targetSpeed = walkSpeed;
                break;
            case AnimationState.Sprint:
                targetSpeed = sprintSpeed;
                break;
            case AnimationState.Walk:
                targetSpeed = walkSpeed;
                break;
            case AnimationState.Idle:
                break;
            default:
                break;
        }

        bool isGrounded = characterController.isGrounded;
        if (isGrounded)
        {
            if (playerVelocity.y < -2f)
            {
                playerVelocity.y = -2f;
            }
        }

        playerVelocity.y += gravity * Time.deltaTime;

        float HorizontalInput = InputHandler.instance.moveInputValue.x;
        float VerticalInput = InputHandler.instance.moveInputValue.y;
        Vector3 WorldDirection = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        moveDirection = (transform.TransformDirection(WorldDirection)).normalized;

        float Alpha = characterController.velocity.magnitude / sprintSpeed;
        movingAlpha = Mathf.Lerp(movingAlpha, Alpha, Time.deltaTime * alphaMultiplier);

        animator.SetFloat("Moving", movingAlpha);

        Vector3 finalMove = moveDirection * targetSpeed + Vector3.up * playerVelocity.y;
        characterController.Move(finalMove * Time.deltaTime);
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    //private void Jump()
    //{
        //if (isGrounded && readyToJump)
        //{
        //    readyToJump = false;

        //    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        //    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        //    Invoke(nameof(ResetJump), jumpCooldown);
        //}
    //}
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void OnDrawGizmos()
    {
        //RaycastHit hitInfo;
        //isGrounded = Physics.SphereCast(transform.position + Vector3.up * (.5f - 0.02f), capsuleCollider.radius, Vector3.down, out hitInfo, capsuleCollider.radius, groundLayer);
        //Gizmos.color = isGrounded ? Color.green : Color.red;
        //Gizmos.DrawWireSphere(transform.position + Vector3.up * (capsuleCollider.radius - 0.02f), capsuleCollider.radius);
    }
}
