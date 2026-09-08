using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] GameObject bloodPref;

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

    public virtual void TakeDamage(float InDamage, Vector3 InHit)
    {
        if (InHit.magnitude != 0.0f && bloodPref)
        {
            Instantiate(bloodPref, InHit, Quaternion.identity);
        }
        SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Hit, GameManager.instance.player.transform, 3, false);
        if (isInvincible)
        {
            return;
        }
        currentHealth -= InDamage;
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
