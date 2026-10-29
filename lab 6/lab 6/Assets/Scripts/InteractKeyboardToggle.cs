using UnityEngine;
using UnityEngine.InputSystem; // necesar pentru Keyboard.current

public class InteractKeyboardToggle : MonoBehaviour
{
    [Header("Target")]
    public ObjectInteractUI target;           // trage aici componenta ObjectInteractUI (pian/gramofon)
    public Transform playerHead;              // Main Camera (XROrigin/Camera Offset/Main Camera)
    [Header("Rules")]
    public float maxDistance = 3f;            // optional: permit toggle doar aproape

    private void Awake()
    {
        if (!target) target = GetComponent<ObjectInteractUI>();
        if (!playerHead && Camera.main) playerHead = Camera.main.transform;
    }

    private void Update()
    {
        // asigură-te că folosești Input System (Project Settings → Player → Active Input Handling = Input System)
        if (Keyboard.current == null) return;
        if (!Keyboard.current.xKey.wasPressedThisFrame) return;
        if (!target) return;

        if (playerHead && Vector3.Distance(playerHead.position, target.transform.position) > maxDistance)
            return;

        target.ToggleDetail();
    }
}
