using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;
    
    public void OnSpawn()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
            Destroy(gameObject);
        }
    }
    public virtual void Die()
    {
    }
}
