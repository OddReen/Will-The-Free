using Unity.Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;

    [SerializeField, Range(0, 100)]
    public float sensibility = 25;

    public Transform orientation;
    public Transform lookAtTarget;
    public float lookAtSpeed;

    public bool canRotate = true;

    public float pitchTarget = 0.0f;
    public float yawTarget = 0.0f;

    enum State
    {
        InputRotation,
        LookAt
    }
    State state;

    private void Start()
    {
        OnSpawn();
    }
    public void OnSpawn()
    {
        cinemachineCamera = GameManager.instance.cinemachineCamera;
        cinemachineCamera.Follow = orientation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        switch (state)
        {
            case State.InputRotation:
                if (canRotate) InputRotation();
                break;
            case State.LookAt:
                LookAt(lookAtTarget);
                break;
        }
    }

    void InputRotation()
    {
        yawTarget += InputHandler.instance.lookInputValue.x * Time.deltaTime * sensibility;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0.0f, yawTarget, 0.0f), Time.deltaTime * sensibility);

        pitchTarget -= InputHandler.instance.lookInputValue.y * Time.deltaTime * sensibility;
        pitchTarget = Mathf.Clamp(pitchTarget, -89.0f, 89.0f);
        orientation.localRotation = Quaternion.Slerp(orientation.localRotation, Quaternion.Euler(pitchTarget, 0.0f, 0.0f), Time.deltaTime * sensibility);
    }

    public void InitLookAt(Transform InTarget)
    {
        lookAtTarget = InTarget;
        cinemachineCamera.Lens.FieldOfView = 40.0f;
        state = State.LookAt;
    }
    public void LookAt(Transform InTarget)
    {
        Quaternion lastRotation = orientation.rotation;
        orientation.LookAt(InTarget, Vector3.up);
        Quaternion NewRotation = orientation.rotation;
        orientation.rotation = lastRotation;

        orientation.rotation = Quaternion.Slerp(orientation.rotation, NewRotation, Time.deltaTime * lookAtSpeed);
        cinemachineCamera.Lens.FieldOfView = 40.0f;
    }
}
