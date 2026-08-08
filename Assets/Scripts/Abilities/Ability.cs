using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ability : MonoBehaviour
{
    [Serializable]
    public struct AbilityData
    {
        public float damage;
        public float knockback;
        public bool inCooldown;
        public float cooldownTimer;
        public bool isReloading;
        public bool isAutomatic;
        public float reloadTime;
        public int ammoAmount;
        public int magazineSize;
        public float projectileSpeed;
        public float recoil;
    };
    [Serializable]
    public struct AbilityHUD
    {
        public string name;
        public Sprite sprite;
        public Image image;
        public float angle;
    };

    public AbilityData abilityData;
    public AbilityHUD abilityHUD;

    public virtual void Start()
    {

    }

    public virtual void OnCancelAbility()
    {

    }

    public virtual void OnExecuteAbility()
    {

    }

    public virtual void OnExecuteReload()
    {

    }

    public virtual void OnAbilityEvent()
    {

    }
}