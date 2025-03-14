using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;
    
    public override void OnNetworkSpawn()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Is Dead");
            Die();
            OnNetworkDespawn();
            Destroy(gameObject);
        }
    }
    public void Die()
    {
        GameManager.instance.EnemyKilled();
    }
}
