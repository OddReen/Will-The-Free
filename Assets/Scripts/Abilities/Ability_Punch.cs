using UnityEngine;

public class Ability_Punch : Ability
{
    [Header("Hitbox")]
    public Hitbox hitbox;
    [SerializeField] public float hitBoxMaxTimer;
    private float hitBoxTimer = 0.0f;

    [Header("Animation")]
    [SerializeField] public float animationMaxCrossfadeTimer;
    private float animationCrossfadeTimer;
    private float animationMaxTimer;
    private float animationTimer;
    [SerializeField] public float animationFinishOffset;

    bool isHitboxEnabled;
    bool isRightPunch = true;

    enum State
    {
        NonTriggered,
        OnCooldown,
        OnCrossfade,
        OnAnimation
    }
    State state;

    public override void Enter(Character_Player character_Player)
    {
        cooldown = abilityData.cooldownTime;
    }

    public override void Update(Character_Player character_Player)
    {
        if (canUpdate)
        {
            OnPunch(character_Player);
        }
        TriggerHitbox();
    }

    public override void OnClickDownAbility(Character_Player character_Player)
    {
        isPressingAbility = true;
        if (!canUpdate)
        {
            canUpdate = true;
            state = State.OnCooldown;
        }
    }

    public override void OnClickUpAbility(Character_Player character_Player)
    {
        isPressingAbility = false;
    }

    public override void OnAbilityEvent(Character_Player character_Player)
    {
        hitbox.GetComponent<CapsuleCollider>().enabled = true;
        isHitboxEnabled = true;
    }

    public float punchMultiplier;
    void OnPunch(Character_Player character_Player)
    {
        punchMultiplier = Mathf.Min(punchMultiplier, 2.00f);
        character_Player.animator.SetFloat("PunchMultiplier", punchMultiplier);
        if (state == State.OnCooldown && AdvanceTimer(ref cooldown, abilityData.cooldownTime))
        {
            if (isRightPunch)
            {
                isRightPunch = false;
                character_Player.animator.CrossFade("Punch_Right", animationMaxCrossfadeTimer);
            }
            else
            {
                isRightPunch = true;
                character_Player.animator.CrossFade("Punch_Left", animationMaxCrossfadeTimer);
            }
            character_Player.animator.Update(0.0f);
            state = State.OnCrossfade;
        }

        if (state == State.OnCrossfade && AdvanceTimer(ref animationCrossfadeTimer, animationMaxCrossfadeTimer))
        {
            animationMaxTimer = character_Player.animator.GetCurrentAnimatorStateInfo(0).length - (animationMaxCrossfadeTimer + animationFinishOffset);
            state = State.OnAnimation;
        }

        if (state == State.OnAnimation && AdvanceTimer(ref animationTimer, animationMaxTimer))
        {
            if (abilityData.isAutomatic && isPressingAbility)
            {
                state = State.OnCooldown;
            }
            else
            {
                state = State.NonTriggered;
                isRightPunch = true;
                canUpdate = false;
            }
            character_Player.animator.CrossFade("Locomotion", 0.1f);
            character_Player.animator.Update(0.0f);
        }
    }

    void TriggerHitbox()
    {
        if (isHitboxEnabled && AdvanceTimer(ref hitBoxTimer, hitBoxMaxTimer))
        {
            hitbox.GetComponent<CapsuleCollider>().enabled = false;
            isHitboxEnabled = false;
        }
    }
}