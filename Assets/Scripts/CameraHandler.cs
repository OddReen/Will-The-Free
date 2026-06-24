using Unity.Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] InputHandler inputHandler;

    [SerializeField] CinemachineCamera cinemachineCamera;

    [SerializeField, Range(0, 100)]
    public float sensibility = 25;

    public Transform orientation;

    float Pitch;
    float Yaw;

    private void Awake()
    {
        OnSpawn();
    }
    public void OnSpawn()
    {
        cinemachineCamera.transform.SetParent(null, false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
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
