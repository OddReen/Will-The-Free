using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    public float damage;
    Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
        Invoke(nameof(DestroyBullet), 3);
    }
    private void Update()
    {
        RaycastHit hit;
        Vector3 bulletDir = (transform.position - lastPosition).normalized;
        float bulletDistance = Vector3.Distance(transform.position, lastPosition);
        Debug.DrawLine(transform.position, transform.position + bulletDir * bulletDistance, Color.red);

        if (Physics.Raycast(lastPosition, bulletDir, out hit, bulletDistance))
        {
            if (hit.collider != null)
            {
                HealthSystem hitHealthSystem = hit.collider.gameObject.GetComponent<HealthSystem>();
                if (hitHealthSystem != null)
                {
                    hitHealthSystem.TakeDamage(damage);
                }
                DestroyBullet();
            }
        }
        lastPosition = transform.position;
    }
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
