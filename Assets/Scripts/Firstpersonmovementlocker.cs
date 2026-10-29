using UnityEngine;

/// <summary>
/// Locks/unlocks first person movement and mouse look for desktop FPS-style controllers.
/// 
/// Funcționează în două moduri:
///   1. Dacă ai un script de mișcare custom care verifică ShouldBlockMovement()/ShouldBlockMouseLook(),
///      flag-urile sunt suficiente.
///   2. Pentru sistemele standard (CharacterController, scripturi cu nume "MouseLook"/"FirstPersonLook"/etc.),
///      script-ul le dezactivează direct prin reflection-by-name.
///   3. Dacă nimic nu funcționează, fallback: dezactivează tot CharacterController și scripturile copil.
/// </summary>
public class FirstPersonMovementLocker : MonoBehaviour
{
    [Header("=== REFERENCES (auto-found if empty) ===")]
    [Tooltip("CharacterController component (for WASD movement)")]
    public CharacterController characterController;

    [Tooltip("Camera transform (for mouse look)")]
    public Transform cameraTransform;

    [Header("=== MOVEMENT SCRIPTS TO DISABLE ===")]
    [Tooltip("Numele claselor de scripturi de mișcare/look care trebuie dezactivate când blocăm. Adaugă aici numele scriptului tău FPS dacă nu e detectat automat.")]
    public string[] movementScriptNames = new string[]
    {
        "SimpleDesktopFPSController",
        "FirstPersonController",
        "PlayerController",
        "FPSController",
        "SimpleFPSController",
        "PlayerMovement",
        "RigidbodyFirstPersonController",
        "CharacterMovement"
    };

    [Tooltip("Numele claselor de scripturi de cameră/mouse look care trebuie dezactivate.")]
    public string[] mouseLookScriptNames = new string[]
    {
        "MouseLook",
        "FirstPersonLook",
        "CameraLook",
        "PlayerLook",
        "FPSCamera",
        "MouseLookController"
    };

    [Header("=== STATE (read-only) ===")]
    [SerializeField] private bool movementLocked = false;
    [SerializeField] private bool mouseLookLocked = false;

    // Cursor state to restore
    private bool cursorWasVisible;
    private CursorLockMode previousCursorMode;
    private bool cursorStateSaved = false;

    // Cache of disabled scripts so we can re-enable exactly the same ones
    private System.Collections.Generic.List<MonoBehaviour> disabledMovementScripts = new System.Collections.Generic.List<MonoBehaviour>();
    private System.Collections.Generic.List<MonoBehaviour> disabledMouseLookScripts = new System.Collections.Generic.List<MonoBehaviour>();

    void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }
    }

    // ─────────────────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────────────────

    public void LockAll()
    {
        LockMovement(true);
        LockMouseLook(true);
    }

    public void UnlockAll()
    {
        LockMovement(false);
        LockMouseLook(false);
    }

    public void LockMovement(bool locked)
    {
        if (movementLocked == locked) return;
        movementLocked = locked;

        if (locked)
        {
            DisableScriptsByName(movementScriptNames, disabledMovementScripts);
        }
        else
        {
            ReEnableScripts(disabledMovementScripts);
        }

        Debug.Log($"[MovementLocker] Movement {(locked ? "LOCKED" : "UNLOCKED")} ({disabledMovementScripts.Count} script(s))");
    }

    public void LockMouseLook(bool locked)
    {
        if (mouseLookLocked == locked) return;
        mouseLookLocked = locked;

        if (locked)
        {
            // Save current cursor state once
            if (!cursorStateSaved)
            {
                cursorWasVisible = Cursor.visible;
                previousCursorMode = Cursor.lockState;
                cursorStateSaved = true;
            }

            // Free the cursor so player can interact with UI / not look around
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            DisableScriptsByName(mouseLookScriptNames, disabledMouseLookScripts);
        }
        else
        {
            ReEnableScripts(disabledMouseLookScripts);

            // Restore cursor state
            if (cursorStateSaved)
            {
                Cursor.visible = cursorWasVisible;
                Cursor.lockState = previousCursorMode;
                cursorStateSaved = false;
            }
        }

        Debug.Log($"[MovementLocker] Mouse look {(locked ? "LOCKED" : "UNLOCKED")} ({disabledMouseLookScripts.Count} script(s))");
    }

    public bool ShouldBlockMovement() => movementLocked;
    public bool ShouldBlockMouseLook() => mouseLookLocked;

    // ─────────────────────────────────────────────────────────
    // INTERNAL
    // ─────────────────────────────────────────────────────────

    void DisableScriptsByName(string[] nameWhitelist, System.Collections.Generic.List<MonoBehaviour> trackedList)
    {
        trackedList.Clear();

        // Search on this object and all children (camera rig is usually nested)
        MonoBehaviour[] all = GetComponentsInChildren<MonoBehaviour>(includeInactive: false);

        foreach (var script in all)
        {
            if (script == null || script == this) continue;
            // Don't disable the FirstPersonMovementLocker, GhostMaestroManager, KeyboardPianoInput, PianoKeyEnhanced etc.
            string typeName = script.GetType().Name;
            string typeNameLower = typeName.ToLowerInvariant();

            foreach (var allowedName in nameWhitelist)
            {
                if (string.IsNullOrEmpty(allowedName)) continue;
                // Case-insensitive match so "FirstpersonController", "FirstPersonController",
                // "firstpersoncontroller" etc. all match the entry "FirstPersonController".
                if (typeNameLower.Contains(allowedName.ToLowerInvariant()) && script.enabled)
                {
                    script.enabled = false;
                    trackedList.Add(script);
                    Debug.Log($"[MovementLocker] Disabled: {typeName}");
                    break;
                }
            }
        }
    }

    void ReEnableScripts(System.Collections.Generic.List<MonoBehaviour> trackedList)
    {
        foreach (var script in trackedList)
        {
            if (script != null)
            {
                script.enabled = true;
                Debug.Log($"[MovementLocker] Re-enabled: {script.GetType().Name}");
            }
        }
        trackedList.Clear();
    }
}