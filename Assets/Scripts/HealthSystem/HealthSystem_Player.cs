using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthSystem_Player : HealthSystem
{
    [SerializeField] GameObject bloodScreen;
    public override void UpdateHealthBar()
    {
        GameManager.instance.HealthBarUpdate(currentHealth, maxHealth);
    }
    public override void TakeDamage(float damage, Vector3 hit)
    {
        base.TakeDamage(damage, hit);
        StartCoroutine(BloodScreenOnDamage());
    }
    public override void Die()
    {
        GameManager.instance.OnDeath();
    }
    public virtual IEnumerator BloodScreenOnDamage()
    {
        bloodScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        bloodScreen.SetActive(false);
    }
}
