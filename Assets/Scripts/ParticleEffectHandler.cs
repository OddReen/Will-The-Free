using UnityEngine;

public class ParticleEffectHandler : MonoBehaviour
{
    ParticleSystem _particleSystem;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        _particleSystem.Play();
    }
}
