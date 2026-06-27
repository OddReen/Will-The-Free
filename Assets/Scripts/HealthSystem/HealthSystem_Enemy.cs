using UnityEngine;

public class HealthSystem_Enemy : HealthSystem
{
    public override void Die()
    {
        GameManager.instance.OnEnemyDeath();
    }
}
