using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] Ability ability;

    private void OnTriggerEnter(Collider other)
    {
        EnemyBehaviour enemy = other.gameObject.GetComponent<EnemyBehaviour>();
        if (enemy != null)
        {
            Debug.Log("Punched Enemy");
            other.GetComponent<HealthSystem>().TakeDamage(ability.abilityData.damage);
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Punch, transform, 1.0f, true);
        }
    }
}
