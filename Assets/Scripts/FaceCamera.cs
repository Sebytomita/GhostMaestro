using UnityEngine;

/// <summary>
/// Makes the UI canvas always face the player camera
/// Perfect for VR lesson UI
/// </summary>
public class FaceCamera : MonoBehaviour
{
    [Header("Settings")]
    public bool faceCamera = true;
    public Vector3 offset = new Vector3(0, 0, 2.5f); // 2.5m in front
    public bool followPosition = false; // Set true if you want UI to follow player

    private Transform cameraTransform;

    void Start()
    {
        // Find main camera (VR camera)
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("[FaceCamera] No main camera found!");
        }
    }

    void LateUpdate()
    {
        if (!cameraTransform || !faceCamera) return;

        // Follow player position (optional)
        if (followPosition)
        {
            Vector3 targetPos = cameraTransform.position + cameraTransform.forward * offset.z;
            targetPos.y = cameraTransform.position.y + offset.y;
            transform.position = targetPos;
        }

        // Always face the camera
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0); // Flip to face player
    }
}