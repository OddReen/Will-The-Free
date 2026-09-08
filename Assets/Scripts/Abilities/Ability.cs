using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class Ability
{
    public float cooldown;
    public bool isPressingAbility;
    public bool canUpdate;

    [Serializable]
    public class AbilityData
    {
        public string name;
        public float damage;

        public float knockbackForce;
        public float knockbackDuration;

        public float cooldownTime;

        public bool isAutomatic;
        public float reloadTime;
        public int ammoAmount;
        public int magazineSize;

        public float projectileSpeed;
        public float recoil;
    };

    [Serializable]
    public class AbilityHUD
    {
        public Sprite sprite;
        public Image image;
        public float angle;
    };

    public AbilityData abilityData;
    public AbilityHUD abilityHUD;

    public virtual void Enter(Character_Player player) { }
    public virtual void Update(Character_Player player) { }
    public virtual void Exit(Character_Player player) { }
    public virtual void OnClickUpAbility(Character_Player player) { }
    public virtual void OnClickDownAbility(Character_Player player) { }
    public virtual void OnClickDownReload(Character_Player player) { }
    public virtual void OnAbilityEvent(Character_Player player) { }

    public bool AdvanceTimer(ref float InTimer, float InMaxTimer)
    {
        InTimer += Time.deltaTime;
        if (InTimer >= InMaxTimer)
        {
            InTimer = 0.0f;
            return true;
        }
        return false;
    }
}