using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Ability;

public class AbilityWheel : MonoBehaviour
{
    Transform arrow;
    Transform abilityWheelHUD;
    [SerializeField] float abilityRadius;
    [SerializeField] Animator animator_FPSArms;

    [SerializeField] public Ability spitAbility;
    [SerializeField] public Ability punchAbility;

    [SerializeField] public List<Ability> wheelAbilities;

    public int currentAbilityIndex;

    bool isActivated = false;

    void Start()
    {
        InputHandler.instance.OnExecuteAbility += OnExecuteAbility;
        InputHandler.instance.OnCancelAbility += OnCancelAbility;
        InputHandler.instance.OnExecuteReload += OnExecuteReload;
        InputHandler.instance.OnAbilityWheel += ActivateAbilityWheel;
        InputHandler.instance.OnAbilityWheelCancel += DeactivateAbilityWheel;

        arrow = GameManager.instance.arrow;
        abilityWheelHUD = GameManager.instance.abilityWheel;

        AddAbility(punchAbility);
        AddAbility(spitAbility);

        UpdateAbilityWheel();
    }

    public void AddAbility(Ability InAbility)
    {
        wheelAbilities.Add(InAbility);
    }

    private void Update()
    {
        ChoosingAbility();
    }

    public void OnCancelAbility()
    {
        wheelAbilities[currentAbilityIndex].OnCancelAbility();
    }
    public void OnExecuteAbility()
    {
        wheelAbilities[currentAbilityIndex].OnExecuteAbility();
    }
    public void OnExecuteReload()
    {
        wheelAbilities[currentAbilityIndex].OnExecuteReload();
    }
    public void OnAbilityEvent()
    {
        wheelAbilities[currentAbilityIndex].OnAbilityEvent();
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