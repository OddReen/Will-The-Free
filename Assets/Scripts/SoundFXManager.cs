using UnityEngine;
using static SoundFXManager;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXPrefab;
    [SerializeField] private AudioClip[] footStepClips;
    [SerializeField] private AudioClip[] gunshotClips;
    [SerializeField] private AudioClip[] reloadClips;
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip[] punchClips;
    [SerializeField] private AudioClip[] williamArrivingClips;
    [SerializeField] private AudioClip[] enemyMoans;

    public enum SoundCategory
    {
        Footstep,
        Gunshot,
        Reload,
        Hit,
        Punch,
        WilliamArriving,
        EnemyMoans
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void TriggerRandomSoundFX(SoundCategory category, Transform spawnTransform, float volume, bool attached)
    {
        int rand = 0;
        AudioClip[] clipArray = new AudioClip[0];
        AudioClip clip = null;
        switch (category)
        {
            case SoundCategory.Footstep:
                clipArray = footStepClips;
                break;
            case SoundCategory.Gunshot:
                clipArray = gunshotClips;
                break;
            case SoundCategory.Reload:
                clipArray = reloadClips;
                break;
            case SoundCategory.Hit:
                clipArray = hitClips;
                break;
            case SoundCategory.Punch:
                clipArray = punchClips;
                break;
            case SoundCategory.WilliamArriving:
                clipArray = williamArrivingClips;
                break;
            case SoundCategory.EnemyMoans:
                clipArray = enemyMoans;
                break;
        }

        if (clipArray.Length > 0)
        {
            rand = Random.Range(0, clipArray.Length);
            if (rand >= 0 && rand < clipArray.Length)
            {
                clip = clipArray[rand];
            }
        }

        if (clip == null)
        {
            return;
        }

        SpawnSoundFX(clip, spawnTransform.position, volume);
    }

    private void SpawnSoundFX(AudioClip clip, Vector3 spawnPosition, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXPrefab, spawnPosition, Quaternion.identity);

        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.spatialBlend = 1f;
        audioSource.spatialize = false;

        audioSource.priority = 200;
        audioSource.dopplerLevel = 0f;
        audioSource.pitch = 1f;

        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
    }
}
