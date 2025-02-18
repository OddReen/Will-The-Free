using UnityEngine;

public class AnimEventSounds : MonoBehaviour
{
    [SerializeField] AudioClip[] steps;
    [SerializeField] Transform stepPos;
    public void Steps()
    {
        SoundFXManager.instance.PlayerRandomSoundFXClip(steps, stepPos, 1, true);
    }
}
