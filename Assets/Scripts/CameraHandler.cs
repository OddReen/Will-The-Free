using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraHandler : NetworkBehaviour
{
    [SerializeField] InputHandler inputHandler;

    [SerializeField] CinemachineCamera cinemachineCamera;

    [SerializeField, Range(0, 100)]
    public float sensibility = 25;

    public Transform orientation;

    float Pitch;
    float Yaw;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            cinemachineCamera.gameObject.SetActive(false);
        }
        else
        {
            cinemachineCamera.transform.SetParent(null, false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Rotation();
    }
    void Rotation()
    {
        float moveX = inputHandler.lookInputValue.x * Time.deltaTime * sensibility;
        float moveY = inputHandler.lookInputValue.y * Time.deltaTime * sensibility;

        Yaw += moveX;

        Pitch -= moveY;

        Pitch = Mathf.Clamp(Pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, Yaw, 0);

        orientation.localRotation = Quaternion.Euler(Pitch, 0, 0);
    }
}
