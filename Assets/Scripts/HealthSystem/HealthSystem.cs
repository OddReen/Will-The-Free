using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public bool isInvincible = false;

    public float currentHealth;
    public float maxHealth = 100;

    private void Awake()
    {
        OnSpawn();
    }

    public void OnSpawn()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Hit, GameManager.instance.player.transform, 3, false);
        if (isInvincible)
        {
            return;
        }
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            Die();
            Destroy(gameObject);
        }
    }
    public virtual void UpdateHealthBar()
    {

    }

    public virtual void Die()
    {
    }
}
