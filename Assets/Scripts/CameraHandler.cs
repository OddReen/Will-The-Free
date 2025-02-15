using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    public float sensibility = 25;

    public Transform orientation;

    float Pitch;
    float Yaw;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        Rotation();
    }
    void Rotation()
    {
        float moveX = InputHandler.inputHandler.lookInputValue.x * Time.deltaTime * sensibility;
        float moveY = InputHandler.inputHandler.lookInputValue.y * Time.deltaTime * sensibility;

        Yaw += moveX;

        Pitch -= moveY;

        Pitch = Mathf.Clamp(Pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, Yaw, 0);

        orientation.localRotation = Quaternion.Euler(Pitch, 0, 0);
    }
}
