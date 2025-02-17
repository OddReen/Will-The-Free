using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayerSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, bool attached)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        if (attached)
        {
            audioSource.transform.SetParent(spawnTransform);
        }

        audioSource.clip = audioClip;

        audioSource.spatialBlend = 1;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayerRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume, bool attached)
    {
        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        if (attached)
        {
            audioSource.transform.SetParent(spawnTransform);
        }
        audioSource.clip = audioClip[rand];

        audioSource.spatialBlend = 1;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

}
