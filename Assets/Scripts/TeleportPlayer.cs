using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportPlayer : MonoBehaviour
{
    void LateUpdate()
    {
        if (Keyboard.current.f11Key.wasPressedThisFrame)
        {
            GameManager.instance.player.transform.position = transform.position;
        }
    }
}
