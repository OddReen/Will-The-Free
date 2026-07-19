using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    EnemyBehaviour enemyBehaviour;

    [SerializeField] AudioClip[] steps;
    [SerializeField] Transform stepPos;

    private void Awake()
    {
        enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
    }

    public void Steps()
    {
        SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Footstep, stepPos, 1, true);
    }

    public void OnAttackAnimEvent()
    {
        enemyBehaviour.OnAttackAnimEvent();
    }
}
