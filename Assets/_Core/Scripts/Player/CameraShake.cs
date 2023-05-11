using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Variables for controlling the camera shake effect
    public float shakeMagnitude = 0.05f; // Magnitude of the shake effect
    public float smoothFactor = 3f;      // Smoothing factor for the shake effect

    private Vector3 originalPosition;    // Original position of the camera
    private Vector3 targetPosition;      // Target position for the camera shake effect

    private void Awake()
    {
        // Store the original position of the camera
        originalPosition = transform.localPosition;
    }

    public void ShakeCamera(float intensity)
    {
        // Calculate the target position based on the intensity and shake magnitude
        targetPosition = originalPosition + Random.insideUnitSphere * intensity * shakeMagnitude;
    }

    private void Update()
    {
        // Smoothly move the camera towards the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, smoothFactor * Time.deltaTime);
    }
}
