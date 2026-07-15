using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public class ShootingSystem : MonoBehaviour
{
    public AudioClip[] shootingSounds;

    [SerializeField] GameObject bulletPref;
    [SerializeField] Transform barrelEnd;

    [SerializeField] Animator animator;

    RaycastHit hit;

    Vector3 bulletDir;

    bool isShootingCooldown = false;
    bool isReloading = false;
    bool isAiming = false;

    [Serializable]
    public struct Gun
    {
        public bool isAutomatic;
        public float reloadTime;
        public int ammoAmount;
        public int magazineSize;
        public float shootingCooldown;
        public float damage;
        public float projectileSpeed;
        public float recoil;
    }

    [SerializeField]
    public Gun defaultGun;

    private void Start()
    {
        InputHandler.instance.OnShootDown += OnShoot;
        InputHandler.instance.OnAim += Aim;
        InputHandler.instance.OnStopAim += StopAim;
        InputHandler.instance.OnReload += StartReload;

        defaultGun.ammoAmount = defaultGun.magazineSize;
        GameManager.instance.AmmoUpdate(defaultGun.ammoAmount, defaultGun.magazineSize);
    }
    private void Update()
    {
        DetectTargetedPoint();
    }

    void DetectTargetedPoint()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, float.MaxValue))
        {
            Debug.DrawLine(hit.point, hit.point + Vector3.up, Color.red);
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.blue);

            bulletDir = (hit.point - barrelEnd.position).normalized;
        }
        else
        {
            bulletDir = (Camera.main.transform.forward * 1000 - barrelEnd.position).normalized;
        }
    }

    void OnShoot()
    {
        if (!isShootingCooldown)
        {
            Shoot();
            isShootingCooldown = true;
            Invoke(nameof(ShootingUnrestrict), defaultGun.shootingCooldown);
        }
    }

    void Recoil()
    {
        float XRot = GetComponent<CameraHandler>().orientation.localRotation.eulerAngles.x;
        GetComponent<CameraHandler>().orientation.localRotation = Quaternion.Euler(XRot - defaultGun.recoil, 0, 0);
    }

    void Shoot()
    {
        if (defaultGun.ammoAmount > 0)
        {
            Recoil();
            isShootingCooldown = false;

            string shootAnim = isAiming ? "Gun_AimShoot" : "Gun_Shoot";
            //animator.CrossFade(shootAnim, 0.0f);
            animator.Play(shootAnim);

            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Gunshot, transform, 1, false);
            Quaternion bulletRotation = Quaternion.LookRotation(barrelEnd.right, barrelEnd.forward);
            GameObject NewBullet = Instantiate(bulletPref, barrelEnd.position, bulletRotation);
            NewBullet.GetComponent<BulletHandler>().damage = defaultGun.damage;
            NewBullet.GetComponent<Rigidbody>().linearVelocity = bulletDir * defaultGun.projectileSpeed;

            defaultGun.ammoAmount--;
            GameManager.instance.AmmoUpdate(defaultGun.ammoAmount, defaultGun.magazineSize);
        }
        else
        {
            StartReload();
        }
    }

    void ShootingUnrestrict()
    {
        isShootingCooldown = false;
    }

    void StartReload()
    {
        if (!isReloading && defaultGun.ammoAmount < defaultGun.magazineSize)
        {
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Reload, transform, 1, false);
            isReloading = true;
            Invoke(nameof(Reload), defaultGun.reloadTime);
        }
    }

    void Reload()
    {
        defaultGun.ammoAmount = defaultGun.magazineSize;
        GameManager.instance.AmmoUpdate(defaultGun.ammoAmount, defaultGun.magazineSize);
        isReloading = false;
    }
    
    void Aim()
    {
        isAiming = true;
        animator.CrossFade("Gun_AimIdle", 0.0f);
    }

    void StopAim()
    {
        isAiming = false;
        animator.CrossFade("Gun_Idle", 0.0f);
    }
}