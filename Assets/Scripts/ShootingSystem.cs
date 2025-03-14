using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [SerializeField] InputHandler inputHandler;

    public AudioClip[] shootingSounds;

    [SerializeField] GameObject bulletPref;
    [SerializeField] Transform barrelEnd;

    [SerializeField] float bulletSpeed;

    [SerializeField] float damage;

    [SerializeField] Animator animator;

    RaycastHit hit;

    Vector3 bulletDir;

    private void Start()
    {
        inputHandler.OnShoot += Shoot;

        inputHandler.OnAim += Aim;
        inputHandler.OnStopAim += StopAim;
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
    void Shoot()
    {
        animator.SetTrigger("Shoot");
        //SoundFXManager.instance.PlayerRandomSoundFXClip(shootingSounds, transform, 1, false);
        Quaternion bulletRotation = Quaternion.LookRotation(barrelEnd.right, barrelEnd.forward);
        GameObject NewBullet = Instantiate(bulletPref, barrelEnd.position, bulletRotation);
        NewBullet.GetComponent<BulletHandler>().damage = damage;
        NewBullet.GetComponent<Rigidbody>().linearVelocity = bulletDir * bulletSpeed;
    }
    void Aim()
    {
        animator.SetBool("Aim", true);
    }
    void StopAim()
    {
        animator.SetBool("Aim", false);
    }
}