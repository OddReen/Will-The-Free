using Unity.Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;
    Rigidbody rb;

    [SerializeField, Range(0, 100)]
    public float sensibility = 25;

    public Transform orientation;

    public bool canRotate = true;

    public float xRotation = 0.0f;
    public float yRotation = 0.0f;

    private void Awake()
    {
        OnSpawn();
    }
    public void OnSpawn()
    {
        rb = GetComponent<Rigidbody>();
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
        if (canRotate)
        {
            yRotation += InputHandler.instance.lookInputValue.x * Time.deltaTime * sensibility;
            rb.MoveRotation(Quaternion.Euler(0, yRotation, 0));

            xRotation -= InputHandler.instance.lookInputValue.y * Time.deltaTime * sensibility;
            xRotation = Mathf.Clamp(xRotation, -89.0f, 89.0f);
            orientation.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
    }
}
