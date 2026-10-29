using UnityEngine;

/// <summary>
/// Revert Key Rotation - Undo the 180° rotation
/// </summary>
public class RevertKeyRotation : MonoBehaviour
{
    [ContextMenu("Revert All Keys Rotation (Undo Fix)")]
    public void RevertRotation()
    {
        PianoKeyEnhanced[] allKeys = FindObjectsOfType<PianoKeyEnhanced>();

        int revertedCount = 0;

        foreach (var key in allKeys)
        {
            Transform keyTransform = key.transform;
            Vector3 rotation = keyTransform.localEulerAngles;

            // Subtract 180° from Y rotation (undo the fix)
            rotation.y -= 180f;

            // Normalize angle
            if (rotation.y < 0) rotation.y += 360f;

            keyTransform.localEulerAngles = rotation;
            revertedCount++;

            Debug.Log($"[Revert] {key.name} rotation: {rotation}");
        }

        Debug.Log($"[Revert] ✅ Reverted {revertedCount} piano keys rotation");
    }

    [ContextMenu("Reset All Keys to Zero Rotation")]
    public void ResetToZero()
    {
        PianoKeyEnhanced[] allKeys = FindObjectsOfType<PianoKeyEnhanced>();

        int resetCount = 0;

        foreach (var key in allKeys)
        {
            key.transform.localEulerAngles = Vector3.zero;
            resetCount++;
        }

        Debug.Log($"[Revert] ✅ Reset {resetCount} piano keys to zero rotation");
    }
}