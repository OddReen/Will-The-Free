using System;
using Unity.VisualScripting;
using UnityEngine;
using static Ability;

public class Ability_Spit : Ability
{
    RaycastHit hit;
    Vector3 bulletDir;

    public GameObject spitPref;
    public GameObject spitSpawner;

    public override void Start()
    {
        base.Start();
        abilityData.ammoAmount = abilityData.magazineSize;
        GameManager.instance.AmmoUpdate(abilityData.ammoAmount, abilityData.magazineSize);
    }

    public override void OnExecuteAbility()
    {
        OnShoot();
    }

    void OnShoot()
    {
        if (!abilityData.inCooldown)
        {
            Shoot();
            abilityData.inCooldown = true;
            Invoke(nameof(ResetCooldown), abilityData.cooldownTimer);
        }
    }

    void Recoil()
    {
        float XRot = GetComponent<CameraHandler>().orientation.localRotation.eulerAngles.x;
        GetComponent<CameraHandler>().orientation.localRotation = Quaternion.Euler(XRot - abilityData.recoil, 0, 0);
    }

    void Shoot()
    {
        if (abilityData.ammoAmount > 0)
        {
            Recoil();
            abilityData.inCooldown = false;

            Transform SpitSpawner = spitSpawner.transform;
            GameObject SpitPref = spitPref;

            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Gunshot, transform, 1, false);
            Quaternion bulletRotation = Quaternion.LookRotation(SpitSpawner.right, SpitSpawner.forward);
            GameObject NewBullet = Instantiate(SpitPref, SpitSpawner.position, bulletRotation);
            NewBullet.GetComponent<BulletHandler>().damage = abilityData.damage;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, float.MaxValue))
            {
                bulletDir = (hit.point - SpitSpawner.position).normalized;
            }
            else
            {
                bulletDir = (Camera.main.transform.forward * 1000 - SpitSpawner.position).normalized;
            }

            NewBullet.GetComponent<Rigidbody>().linearVelocity = bulletDir * abilityData.projectileSpeed;

            abilityData.ammoAmount--;
            GameManager.instance.AmmoUpdate(abilityData.ammoAmount, abilityData.magazineSize);
        }
        else
        {
            StartReload();
        }
    }

    void ResetCooldown()
    {
        abilityData.inCooldown = false;
    } 

    void StartReload()
    {
        if (!abilityData.isReloading && abilityData.ammoAmount < abilityData.magazineSize)
        {
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Reload, transform, 1, false);
            abilityData.isReloading = true;
            Invoke(nameof(Reload), abilityData.reloadTime);
        }
    }

    void Reload()
    {
        abilityData.ammoAmount = abilityData.magazineSize;
        GameManager.instance.AmmoUpdate(abilityData.ammoAmount, abilityData.magazineSize);
        abilityData.isReloading = false;
    }
}
