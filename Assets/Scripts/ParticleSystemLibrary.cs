using UnityEngine;

public class ParticleSystemLibrary : MonoBehaviour
{
    public static ParticleSystemLibrary instance;
    public GameObject bulletHitEffect;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
