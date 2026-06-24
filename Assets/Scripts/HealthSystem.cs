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
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
            Destroy(gameObject);
        }
    }
    public void Die()
    {
        GameManager.instance.OnEnemyDeath();
    }
}
