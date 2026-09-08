using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Character_Player : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float targetSpeed;

    public Vector3 playerVelocity;

    public float movingAlpha = 0.0f;
    public float alphaMultiplier = 0.0f;
    public float gravity = -9.84f;

    public float jumpHeight = 1.5f;
    [SerializeField] bool isSprinting = false;

    public Transform orientation;

    Vector3 moveDirection;

    public CharacterController characterController;

    [SerializeField] public Animator animator;

    Transform arrow;
    Transform abilityWheelHUD;
    [SerializeField] float abilityRadius;

    public int currentAbilityIndex;

    bool isActivated = false;

    [SerializeField] public Ability ability_;

    [SerializeField] public List<Ability> wheelAbilities;

    enum MoveAnimationState
    {
        Falling,
        Sprint,
        Walk,
        Idle
    }

    [SerializeField] MoveAnimationState state;

    [Header("Ground Check")]
    public LayerMask groundLayer;


    private void Awake()
    {
        OnSpawn();
    }
    private void Update()
    {
        UpdateMoveAnimationState();
        Move();
        ChoosingAbility();
        ability_.Update(this);
    }
    public void OnSpawn()
    {
        characterController = GetComponent<CharacterController>();

        InputHandler.instance.OnJump += Jump;
        InputHandler.instance.OnSprint += () => OnSprint(true);
        InputHandler.instance.OnSprintCancel += () => OnSprint(false);
        InputHandler.instance.OnExecuteAbility += OnClickDownAbility;
        InputHandler.instance.OnCancelAbility += OnClickUpAbility;
        InputHandler.instance.OnExecuteReload += OnClickDownReload;
        InputHandler.instance.OnAbilityWheel += ActivateAbilityWheel;
        InputHandler.instance.OnAbilityWheelCancel += DeactivateAbilityWheel;

        arrow = GameManager.instance.arrow;
        abilityWheelHUD = GameManager.instance.abilityWheel;
        wheelAbilities.Add(new Ability_Punch());
        wheelAbilities.Add(new Ability_Spit());

        UpdateAbilityWheel();
    }

    private void UpdateMoveAnimationState()
    {
        if (!characterController.isGrounded)
        {
            state = MoveAnimationState.Falling;
        }
        else if (isSprinting && InputHandler.instance.moveInputValue.magnitude != 0.0f)
        {
            state = MoveAnimationState.Sprint;
        }
        else if (InputHandler.instance.moveInputValue.magnitude != 0.0f)
        {
            state = MoveAnimationState.Walk;
        }
        else
        {
            state = MoveAnimationState.Idle;
        }
    }

    private void OnSprint(bool IsSprinting)
    {
        isSprinting = IsSprinting;
    }
    private void Move()
    {
        targetSpeed = 0.0f;
        switch (state)
        {
            case MoveAnimationState.Falling:
                targetSpeed = walkSpeed;
                break;
            case MoveAnimationState.Sprint:
                targetSpeed = sprintSpeed;
                break;
            case MoveAnimationState.Walk:
                targetSpeed = walkSpeed;
                break;
            case MoveAnimationState.Idle:
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

        animator.SetFloat("Moving", movingAlpha <= 0.01f ? 0.0f : movingAlpha);

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

    public void OnClickUpAbility() => ability_.OnClickUpAbility(this);
    public void OnClickDownAbility() => ability_.OnClickDownAbility(this);
    public void OnClickDownReload() => ability_.OnClickDownReload(this);
    public void OnAbilityEvent() => ability_.OnAbilityEvent(this);

    public void ChangeAbility(Ability ability)
    {
        ability_?.Exit(this);
        ability_ = ability;
        ability_?.Enter(this);
    }
    public void ChoosingAbility()
    {
        if (isActivated)
        {
            if (wheelAbilities.Count == 1)
            {
                currentAbilityIndex = wheelAbilities.Count - 1;
                wheelAbilities[0].abilityHUD.image.color = Color.yellowGreen;

                return;
            }
            Vector2 mouseDirection = Mouse.current.position.ReadValue();
            if (mouseDirection != Vector2.zero)
            {
                mouseDirection = new Vector2(mouseDirection.x - arrow.position.x, mouseDirection.y - arrow.position.y);

                mouseDirection.Normalize();
                float slotAngle = 360.0f / wheelAbilities.Count;
                if (wheelAbilities[currentAbilityIndex].abilityHUD.image != null)
                {
                    wheelAbilities[currentAbilityIndex].abilityHUD.image.color = Color.white;
                }

                float mouseAngle = (Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg);
                arrow.rotation = Quaternion.Euler(new Vector3(0, 0, mouseAngle - 90.0f));

                mouseAngle = (mouseAngle + 360f) % 360f;

                for (int i = 0; i < wheelAbilities.Count; i++)
                {
                    float minAngle = wheelAbilities[i].abilityHUD.angle - (slotAngle * 0.5f);
                    float maxAngle = wheelAbilities[i].abilityHUD.angle + (slotAngle * 0.5f);

                    if (mouseAngle > minAngle && mouseAngle < maxAngle)
                    {
                        currentAbilityIndex = i;
                        wheelAbilities[currentAbilityIndex].abilityHUD.image.color = Color.yellowGreen;
                        return;
                    }
                    if (mouseAngle < slotAngle * 0.5f)
                    {
                        currentAbilityIndex = wheelAbilities.Count - 1;
                        wheelAbilities[currentAbilityIndex].abilityHUD.image.color = Color.yellowGreen;
                        return;
                    }
                }
            }
        }
    }
    public void ActivateAbilityWheel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.instance.player.GetComponent<CameraHandler>().canRotate = false;
        abilityWheelHUD.gameObject.SetActive(true);
        isActivated = true;
        Time.timeScale = 0.1f;
    }
    public void DeactivateAbilityWheel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.instance.player.GetComponent<CameraHandler>().canRotate = true;
        abilityWheelHUD.gameObject.SetActive(false);
        isActivated = false;
        Time.timeScale = 1.0f;
    }
    public void UpdateAbilityWheel()
    {
        if (wheelAbilities.Count > 0)
        {
            if (wheelAbilities.Count == 1)
            {
                if (arrow.gameObject.activeSelf)
                {
                    arrow.gameObject.SetActive(false);
                }

                GameObject abilityIcon = new GameObject();
                abilityIcon.transform.SetParent(abilityWheelHUD, false);
                wheelAbilities[0].abilityHUD.image = abilityIcon.AddComponent<Image>();
                wheelAbilities[0].abilityHUD.image.sprite = wheelAbilities[0].abilityHUD.sprite;

                wheelAbilities[0].abilityHUD.angle = 0.0f;
                return;
            }
            else
            {
                for (int i = 0; i < wheelAbilities.Count; i++)
                {
                    if (!arrow.gameObject.activeSelf)
                    {
                        arrow.gameObject.SetActive(true);
                    }

                    GameObject abilityIcon = new GameObject();
                    abilityIcon.transform.SetParent(abilityWheelHUD, false);
                    wheelAbilities[i].abilityHUD.image = abilityIcon.AddComponent<Image>();
                    wheelAbilities[i].abilityHUD.image.sprite = wheelAbilities[i].abilityHUD.sprite;

                    float sliceIndex = (float)i + 1.0f;
                    float sliceAngle = Mathf.PI * ((((sliceIndex / (float)wheelAbilities.Count) * 360.0f) / 180.0f));
                    wheelAbilities[i].abilityHUD.angle = sliceAngle * Mathf.Rad2Deg;

                    float xCos = Mathf.Cos(sliceAngle);
                    float yCos = Mathf.Sin(sliceAngle);
                    Vector2 abilityPosition = new Vector2(xCos, yCos) * abilityRadius;
                    abilityIcon.transform.localPosition = abilityPosition;
                }
            }
        }
    }
}
