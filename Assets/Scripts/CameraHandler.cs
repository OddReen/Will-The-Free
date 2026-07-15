using Unity.Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;

    [SerializeField, Range(0, 100)]
    public float sensibility = 25;

    public Transform orientation;

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
    private void LateUpdate()
    {
        Rotation();
    }
    void Rotation()
    {
        float moveX = InputHandler.instance.lookInputValue.x * Time.deltaTime * sensibility;
        float Yaw = GetComponent<Rigidbody>().rotation.eulerAngles.y + moveX;
        GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, Yaw, 0));

        float moveY = InputHandler.instance.lookInputValue.y * Time.deltaTime * sensibility;
        float Pitch = orientation.localRotation.eulerAngles.x - moveY;
        //Pitch = Mathf.Clamp(Pitch, -90f, 90f);
        orientation.localRotation = Quaternion.Euler(Pitch, 0, 0);
    }
}
