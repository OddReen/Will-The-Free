using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void Steps()
    {
        SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Footstep, transform, 1, true);
    }

    public void OnAttackAnimEvent()
    {
        EnemyBehaviour enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        enemyBehaviour.OnAttackAnimEvent();
    }
    public void OnAbilityEvent()
    {
        AbilityWheel abilityWheel = GetComponentInParent<AbilityWheel>();
        abilityWheel.OnAbilityEvent();
    }
}
