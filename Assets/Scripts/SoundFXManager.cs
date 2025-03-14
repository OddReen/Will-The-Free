using Unity.Netcode;
using UnityEngine;

public class SoundFXManager : NetworkBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXPrefab; // Prefab with a NetworkObject component
    [SerializeField] private AudioClip[] footStepClips;
    [SerializeField] private AudioClip[] gunshotClips;

    public enum SoundCategory
    {
        Footstep,
        Gunshot
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // This method is called on the client to trigger a random sound effect from a category.
    public void TriggerRandomSoundFX(SoundCategory category, Transform spawnTransform, float volume, bool attached)
    {
        int rand = 0;
        switch (category)
        {
            case SoundCategory.Footstep:
                if (footStepClips.Length > 0)
                    rand = Random.Range(0, footStepClips.Length);
                break;
            case SoundCategory.Gunshot:
                if (gunshotClips.Length > 0)
                    rand = Random.Range(0, gunshotClips.Length);
                break;
        }
        // Call the ServerRpc to handle spawning.
        PlaySoundFXServerRpc((int)category, rand, spawnTransform.position, volume, attached);
    }

    // The ServerRpc runs on the server (or host) so that only the server spawns NetworkObjects.
    [ServerRpc]
    private void PlaySoundFXServerRpc(int category, int clipIndex, Vector3 spawnPosition, float volume, bool attached)
    {
        AudioClip clip = null;
        SoundCategory soundCategory = (SoundCategory)category;
        switch (soundCategory)
        {
            case SoundCategory.Footstep:
                if (clipIndex >= 0 && clipIndex < footStepClips.Length)
                    clip = footStepClips[clipIndex];
                break;
            case SoundCategory.Gunshot:
                if (clipIndex >= 0 && clipIndex < gunshotClips.Length)
                    clip = gunshotClips[clipIndex];
                break;
        }
        if (clip == null)
        {
            Debug.LogError("Invalid clip index for category: " + soundCategory);
            return;
        }
        SpawnSoundFX(clip, spawnPosition, volume, attached);
    }

    private void SpawnSoundFX(AudioClip clip, Vector3 spawnPosition, float volume, bool attached)
    {
        // Instantiate the prefab at the given position.
        AudioSource audioSource = Instantiate(soundFXPrefab, spawnPosition, Quaternion.identity);

        // If you want to attach the sound effect to a parent transform (e.g., the caller), do it here.
        // (Note: you'll need to pass a Transform if you want to attach it; here we only have the position.)
        if (attached)
        {
            // Example: attach to a global container or some specific transform.
            // audioSource.transform.SetParent(someParentTransform);
        }

        // Set up the AudioSource.
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1;  // Make it 3D
        audioSource.Play();

        // Spawn the network object so that the spawned sound is replicated to all clients.
        NetworkObject netObj = audioSource.GetComponent<NetworkObject>();
        netObj.Spawn();

        // Destroy the AudioSource after the clip has finished playing.
        Destroy(audioSource.gameObject, clip.length);
    }
}
