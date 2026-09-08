using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void Steps()
    {
        Character_Player movementHandler = GetComponentInParent<Character_Player>();
        if (movementHandler)
        {
            if (movementHandler.characterController.isGrounded)
            {
                SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Footstep, transform, 1, true);
            }
        }
        else
        {
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Footstep, transform, 1, true);
        }
    }
    
    public void OnAttackAnimEvent()
    {
        Character_Enemy enemyBehaviour = GetComponentInParent<Character_Enemy>();
        if (enemyBehaviour)
        {
            enemyBehaviour.OnAttackAnimEvent();
        }
    }
    public void OnAbilityEvent()
    {
        Character_Player abilityWheel = GetComponentInParent<Character_Player>();
        if (abilityWheel)
        {
            abilityWheel.OnAbilityEvent();
        }
    }
}
