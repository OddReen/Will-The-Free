using UnityEngine;

public class HealthSystem_Player : HealthSystem
{
    public override void Die()
    {
        GameManager.instance.OnDeath();
    }
}
