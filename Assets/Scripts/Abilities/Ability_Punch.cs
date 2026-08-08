using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Ability_Punch : Ability
{
    public Animator animator_FPSArms;
    public Hitbox hitbox;

    public override void Start()
    {
        base.Start();
    }

    public override void OnExecuteAbility()
    {
        OnPunch();
    }

    public override void OnAbilityEvent()
    {
        StartCoroutine(AbilityEvent());
    }

    void OnPunch()
    {
        if (!abilityData.inCooldown)
        {
            abilityData.inCooldown = true;
            StartCoroutine(Punch());
        }
    }
    void ResetCooldown()
    {
        abilityData.inCooldown = false;
    }

    IEnumerator AbilityEvent()
    {
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(.1f);
        hitbox.gameObject.SetActive(false);
    }

    IEnumerator Punch()
    {
        animator_FPSArms.Play("Punch");
        animator_FPSArms.Update(0);
        float sec = animator_FPSArms.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(sec);
        Invoke(nameof(ResetCooldown), abilityData.cooldownTimer);
        animator_FPSArms.Play("Locomotion");
    }
}