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

    public enum SoundCategory
    {
        Footstep,
        Gunshot,
        Reload,
        Hit,
        Punch
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

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1;
        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
    }
}
