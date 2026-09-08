using Unity.Cinemachine;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] Ability_Punch ability;
    [SerializeField] CinemachineImpulseDefinition.ImpulseShapes impulseShape;
    [SerializeField] float impulseForce;
    [SerializeField] Vector3 impulseDirection;
    CinemachineImpulseSource cinemachineImpulseSource;

    private void Start()
    {
        cinemachineImpulseSource = GetComponentInParent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Character_Enemy enemy = other.gameObject.GetComponent<Character_Enemy>();
        if (enemy != null)
        {
            ability.punchMultiplier += 0.1f;
            GameManager.instance.CameraShake(cinemachineImpulseSource, impulseForce, impulseDirection.normalized, impulseShape);
            other.GetComponent<HealthSystem>().TakeDamage(ability.abilityData.damage, Vector3.zero);
            enemy.ApplyKnockback(GetComponentInParent<Character_Player>().orientation.transform.position, ability.abilityData.knockbackForce, ability.abilityData.knockbackDuration);
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Punch, transform, 1.0f, true);
        }
    }
}