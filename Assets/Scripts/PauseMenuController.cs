using UnityEngine;

/// <summary>
/// Toggle panou de pauză cu Tab. Blochează mișcarea + camera și eliberează cursorul,
/// dar nu îngheață timpul (ca să meargă scroll-ul în UI).
/// În timpul lecției, Tab e dezactivat.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("=== REFERINȚE ===")]
    [Tooltip("Panoul de controale (ControllersPanel din scena Game)")]
    public GameObject pausePanel;

    [Tooltip("Tasta de toggle")]
    public KeyCode toggleKey = KeyCode.Tab;

    [Tooltip("FPS Controller (pentru a-l dezactiva când e pauză)")]
    public SimpleDesktopFPSController fpsController;

    [Tooltip("GhostMaestroManager (ca să nu deschidem pauza când ești la pian)")]
    public GhostMaestroManager ghostMaestro;

    private bool isPaused = false;
    private CursorLockMode previousCursorMode;
    private bool previousCursorVisible;

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);

        if (!fpsController) fpsController = FindObjectOfType<SimpleDesktopFPSController>();
        if (!ghostMaestro) ghostMaestro = FindObjectOfType<GhostMaestroManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            // Nu permitem deschiderea pauzei dacă ești la pian (lecție activă)
            if (!isPaused && ghostMaestro != null &&
                (ghostMaestro.currentPhase == GamePhase.Seated ||
                 ghostMaestro.currentPhase == GamePhase.Lessons ||
                 ghostMaestro.currentPhase == GamePhase.SittingDown ||
                 ghostMaestro.currentPhase == GamePhase.StandingUp))
            {
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel) pausePanel.SetActive(isPaused);

        if (isPaused)
        {
            // Salvăm starea cursorului și îl eliberăm pentru scroll/click
            previousCursorMode = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Dezactivăm FPS controller-ul ca să nu mai mișcăm camera
            if (fpsController) fpsController.enabled = false;
        }
        else
        {
            // Restaurăm cursorul
            Cursor.lockState = previousCursorMode;
            Cursor.visible = previousCursorVisible;

            // Reactivăm FPS
            if (fpsController) fpsController.enabled = true;
        }
    }
}