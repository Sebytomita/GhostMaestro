using UnityEngine;

/// <summary>
/// Simple Desktop First Person Controller for Ghost Maestro.
/// 
/// Two modes:
///   1. NORMAL (default): cursor blocat în centru, mouse roti camera mereu, WASD merge
///   2. LOOK ON RIGHT-CLICK: cursor liber pentru UI, mouse roti camera doar când ții click dreapta.
///      Folosit la pian, ca să poți da click pe butoane DAR și să te uiți în jur când vrei.
/// 
/// Setup în Unity:
///   1. Pe player root: Add Component → CharacterController
///      Settings: Center Y=1, Height=2, Radius=0.4
///   2. Add Component → SimpleDesktopFPSController
///   3. Sub player root, Main Camera (la Y≈1.7, înălțime ochi)
///   4. Camera Transform → trage Main Camera
/// 
/// Compatibil cu FirstPersonMovementLocker (care îl dezactivează la așezare).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SimpleDesktopFPSController : MonoBehaviour
{
    [Header("=== MOVEMENT ===")]
    [Tooltip("Viteza de mers (m/s)")]
    public float moveSpeed = 4f;

    [Tooltip("Forța gravitației (m/s²)")]
    public float gravity = -20f;

    [Header("=== MOUSE LOOK ===")]
    [Tooltip("Camera (copil al acestui GameObject) care se rotește pe verticală")]
    public Transform cameraTransform;

    [Tooltip("Sensibilitate mouse")]
    public float mouseSensitivity = 2f;

    [Tooltip("Limită rotire verticală (sus/jos) în grade")]
    public float verticalLookLimit = 85f;

    [Header("=== CURSOR ===")]
    [Tooltip("Blochează cursorul în centrul ecranului la start")]
    public bool lockCursorOnStart = true;

    [Header("=== LOOK MODE ===")]
    [Tooltip("Dacă e bifat: mouse roti camera DOAR când ții click dreapta. Cursor liber altfel. Folosit la pian.")]
    public bool lookOnRightClickOnly = false;

    [Tooltip("Dacă e bifat: WASD nu mai mișcă player-ul. Folosit la pian.")]
    public bool movementBlocked = false;

    // ─── Internal state ─────────────────────────────────
    private CharacterController controller;
    private float verticalRotation = 0f;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
        }

        if (cameraTransform == null)
            Debug.LogError("[FPSController] Nu am găsit camera! Setează Camera Transform în Inspector.");
    }

    void Start()
    {
        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        HandleCursorMode();
        HandleMouseLook();
        HandleMovement();
    }

    /// <summary>
    /// În modul "look on right-click", cursorul se schimbă dinamic:
    /// click dreapta jos = blochez cursor, eliberat = cursor vizibil.
    /// </summary>
    void HandleCursorMode()
    {
        if (!lookOnRightClickOnly) return;

        if (Input.GetMouseButtonDown(1)) // right-click pressed
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetMouseButtonUp(1)) // right-click released
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMouseLook()
    {
        if (cameraTransform == null) return;
        // Regulă: cursor blocat = mouse look. Cursor liber = nu rotim camera.
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // Dacă mișcarea e blocată (la pian), tot aplicăm gravitația dar nu mai citim WASD
        float horizontal = movementBlocked ? 0f : Input.GetAxis("Horizontal");
        float vertical = movementBlocked ? 0f : Input.GetAxis("Vertical");

        Vector3 moveDir = transform.right * horizontal + transform.forward * vertical;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontalVelocity = moveDir * moveSpeed;

        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalVelocity + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    // ─── PUBLIC API ─────────────────────────────────────

    public void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    /// <summary>
    /// Activează/dezactivează modul "look on right-click only".
    /// La pian: enable=true. În mers normal: enable=false.
    /// </summary>
    public void SetLookOnRightClickOnly(bool enable)
    {
        lookOnRightClickOnly = enable;
        if (enable)
        {
            // Începe cu cursor liber (utilizatorul ține click dreapta să privească)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Revine la mers normal: cursor blocat
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Blochează / deblochează WASD. Gravitația și camera rămân active.
    /// </summary>
    public void SetMovementBlocked(bool blocked)
    {
        movementBlocked = blocked;
    }

    /// <summary>
    /// Setează totul deodată: la pian (true) sau mers normal (false).
    /// </summary>
    public void SetPianoMode(bool atPiano)
    {
        SetMovementBlocked(atPiano);
        SetLookOnRightClickOnly(atPiano);
    }
}