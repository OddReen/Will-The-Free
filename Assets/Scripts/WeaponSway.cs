using System;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 5f;
    [SerializeField] private float multiplier = 1f;
    [SerializeField] private float maxAngle = 45f;

    private void LateUpdate()
    {
        // Get mouse input
        float mouseX = InputHandler.inputHandler.lookInputValue.x * multiplier;
        float mouseY = InputHandler.inputHandler.lookInputValue.y * multiplier;

        // Calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion targetRotation = rotationX * rotationY;

        // Convert to Euler angles
        Vector3 targetEuler = targetRotation.eulerAngles;

        // Convert Euler angles to range (-180, 180) for proper clamping
        targetEuler.x = (targetEuler.x > 180) ? targetEuler.x - 360 : targetEuler.x;
        targetEuler.y = (targetEuler.y > 180) ? targetEuler.y - 360 : targetEuler.y;

        // Clamp angles to max rotation
        targetEuler.x = Mathf.Clamp(targetEuler.x, -maxAngle, maxAngle);
        targetEuler.y = Mathf.Clamp(targetEuler.y, -maxAngle, maxAngle);

        // Convert back to quaternion
        targetRotation = Quaternion.Euler(targetEuler);

        // Apply rotation smoothly
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
