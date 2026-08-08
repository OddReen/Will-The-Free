using System;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway With Camera")]
    [SerializeField] private float smooth = 5f;
    [SerializeField] private float multiplier = 1f;
    [SerializeField] private float maxAngle = 45f;

    [Header("Sway With Movement")]
    [SerializeField] MovementHandler movementHandler;

    private void Awake()
    {
        movementHandler = GetComponentInParent<MovementHandler>();
    }

    private void LateUpdate()
    {
        //SwayWithCamera();
        SwayWithMovement();
    }

    private void SwayWithMovement()
    {
        Debug.Log(movementHandler.rb.linearVelocity);

        Vector3 localVelocity = transform.InverseTransformDirection(movementHandler.rb.linearVelocity);
        float moveX = localVelocity.x * multiplier;
        float moveZ = localVelocity.z * multiplier;

        Quaternion targetRotationX = Quaternion.AngleAxis(moveZ, Vector3.right);
        Quaternion targetRotationZ = Quaternion.AngleAxis(moveX, Vector3.up);
        Quaternion targetRotation = targetRotationX * targetRotationZ;

        Vector3 targetEuler = targetRotation.eulerAngles;

        targetEuler.x = (targetEuler.x > 180) ? targetEuler.x - 360 : targetEuler.x;
        targetEuler.z = (targetEuler.z > 180) ? targetEuler.z - 360 : targetEuler.z;

        targetEuler.x = Mathf.Clamp(targetEuler.x, -maxAngle, maxAngle);
        targetEuler.z = Mathf.Clamp(targetEuler.z, -maxAngle, maxAngle);

        targetRotation = Quaternion.Euler(targetEuler);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    private void SwayWithCamera()
    {
        float mouseX = InputHandler.instance.lookInputValue.x * multiplier;
        float mouseY = InputHandler.instance.lookInputValue.y * multiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion targetRotation = rotationX * rotationY;

        Vector3 targetEuler = targetRotation.eulerAngles;

        targetEuler.x = (targetEuler.x > 180) ? targetEuler.x - 360 : targetEuler.x;
        targetEuler.y = (targetEuler.y > 180) ? targetEuler.y - 360 : targetEuler.y;

        targetEuler.x = Mathf.Clamp(targetEuler.x, -maxAngle, maxAngle);
        targetEuler.y = Mathf.Clamp(targetEuler.y, -maxAngle, maxAngle);

        targetRotation = Quaternion.Euler(targetEuler);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
