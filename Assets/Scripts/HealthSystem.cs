using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;

    private void Start()
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
            Destroy(gameObject);
        }
    }
}
