using Unity.Cinemachine;
using UnityEngine;
using static Unity.Cinemachine.CinemachineImpulseDefinition;

public class Ability_Spit : Ability
{
    CameraHandler cameraHandler;
    CinemachineImpulseSource cinemachineImpulseSource;

    public GameObject spitPref;
    public Transform spitSpawner;

    [SerializeField] float impulseForce;
    [SerializeField] Vector3 impulseDirection;
    [SerializeField] ImpulseShapes impulseShape;

    bool isReloading;

    public override void Enter(Character_Player character_Player)
    {
        cinemachineImpulseSource = character_Player.GetComponentInParent<CinemachineImpulseSource>();
        cameraHandler = character_Player.GetComponentInParent<CameraHandler>();
        abilityData.ammoAmount = abilityData.magazineSize;
        GameManager.instance.AmmoUpdate(abilityData.ammoAmount, abilityData.magazineSize);
        cooldown = abilityData.cooldownTime;
    }

    public override void Update(Character_Player character_Player)
    {
        if (canUpdate)
        {
            Shoot(character_Player);
        }

    }

    public override void OnClickDownAbility(Character_Player character_Player)
    {
        isPressingAbility = true;
    }
    public override void OnClickUpAbility(Character_Player character_Player)
    {
        isPressingAbility = false;
    }

    void Recoil()
    {
        cameraHandler.pitchTarget -= abilityData.recoil;
    }

    void Shoot(Character_Player character_Player)
    {
        if (abilityData.ammoAmount > 0 && AdvanceTimer(ref cooldown, abilityData.cooldownTime))
        {
            abilityData.ammoAmount--;

            GameManager.instance.AmmoUpdate(abilityData.ammoAmount, abilityData.magazineSize);
            Recoil();
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Gunshot, character_Player.transform, 1, false);
            GameManager.instance.CameraShake(cinemachineImpulseSource, impulseForce, impulseDirection.normalized, impulseShape);

            Quaternion bulletRotation = Quaternion.LookRotation(spitSpawner.right, spitSpawner.forward);
            GameObject NewBullet = GameObject.Instantiate(spitPref, spitSpawner.position, bulletRotation);
            NewBullet.GetComponent<BulletHandler>().damage = abilityData.damage;

            bool hasDetectedCollision = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, float.MaxValue);
            Vector3 hitDirection = (hit.point - spitSpawner.position).normalized;
            Vector3 forwardDirection = (Camera.main.transform.forward * 1000 - spitSpawner.position).normalized;
            Vector3 direction;
            direction = hasDetectedCollision ? hitDirection : forwardDirection;
            NewBullet.GetComponent<Rigidbody>().linearVelocity = direction * abilityData.projectileSpeed;
        }
        else
        {
            StartReload(character_Player);
        }
    }

    void StartReload(Character_Player character_Player)
    {
        if (isReloading && abilityData.ammoAmount < abilityData.magazineSize)
        {
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.Reload, character_Player.transform, 1, false);
            isReloading = true;
            character_Player.Invoke(nameof(Reload), abilityData.reloadTime);
        }
    }

    void Reload(Character_Player character_Player)
    {
        abilityData.ammoAmount = abilityData.magazineSize;
        GameManager.instance.AmmoUpdate(abilityData.ammoAmount, abilityData.magazineSize);
        isReloading = false;
    }
}
