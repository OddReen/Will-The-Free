using UnityEngine;

public class HealthSystem_Player : HealthSystem
{
    public override void UpdateHealthBar()
    {
        GameManager.instance.HealthBarUpdate(currentHealth, maxHealth);
    }
    public override void Die()
    {
        GameManager.instance.OnDeath();
    }
}
