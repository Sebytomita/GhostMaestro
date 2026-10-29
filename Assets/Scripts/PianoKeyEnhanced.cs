using UnityEngine;

/// <summary>
/// Enhanced Piano Key - Desktop version (no VR/XR).
/// Triggered by:
///   - Mouse click (raycast)
///   - KeyboardPianoInput script calling PressKey()/ReleaseKey() directly
/// Notifies GhostMaestroManager when pressed.
/// </summary>
public class PianoKeyEnhanced : MonoBehaviour
{
    [Header("=== KEY INFO ===")]
    public int noteNumber = 60;
    public string noteName = "C4";

    [Header("=== AUDIO ===")]
    public AudioSource audioSource;
    public AudioClip keySound;
    [Range(0f, 1f)] public float velocity = 1f;

    [Header("=== VISUALS ===")]
    public Renderer keyRenderer;
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material pressedMaterial;

    [Header("=== ANIMATION ===")]
    public Transform keyTransform;
    public float pressDepth = 0.01f;
    public float pressSpeed = 10f;

    [Header("=== INPUT ===")]
    [Tooltip("Allow mouse click to play this key (raycast). Disable to use only keyboard.")]
    public bool allowMouseClick = true;

    // References
    private GhostMaestroManager ghostManager;

    // State
    private bool isPressed = false;
    private Vector3 originalPosition;
    private float lastPressTime;
    private float pressCooldown = 0.1f;

    void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!keyRenderer) keyRenderer = GetComponent<Renderer>();
        if (!keyTransform) keyTransform = transform;

        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.clip = keySound;
        }

        originalPosition = keyTransform.localPosition;
    }

    void Start()
    {
        ghostManager = FindObjectOfType<GhostMaestroManager>();
    }

    void Update()
    {
        if (allowMouseClick)
            HandleMouseInput();
        AnimateKey();
    }

    void HandleMouseInput()
    {
        if (Camera.main == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                    PressKey();
            }
        }

        if (Input.GetMouseButtonUp(0) && isPressed)
            ReleaseKey();
    }

    /// <summary>
    /// Press the key (called by mouse, keyboard input, or external scripts).
    /// </summary>
    public void PressKey()
    {
        if (isPressed) return;
        if (Time.time - lastPressTime < pressCooldown) return;

        isPressed = true;
        lastPressTime = Time.time;

        PlaySound();

        if (keyRenderer && pressedMaterial)
            keyRenderer.material = pressedMaterial;

        NotifyManager();

        Debug.Log($"[PianoKey] ♪ {noteName} (MIDI {noteNumber})");
    }

    /// <summary>
    /// Release the key (called by mouse, keyboard input, or external scripts).
    /// </summary>
    public void ReleaseKey()
    {
        if (!isPressed) return;

        isPressed = false;

        if (keyRenderer && defaultMaterial)
            keyRenderer.material = defaultMaterial;

        if (audioSource && audioSource.isPlaying)
            StartCoroutine(FadeOutSound(0.2f));
    }

    void PlaySound()
    {
        if (!audioSource) return;

        if (keySound)
        {
            audioSource.clip = keySound;
            audioSource.volume = velocity;
            audioSource.Play();
        }
    }

    void NotifyManager()
    {
        if (ghostManager)
            ghostManager.OnNotePlayed(noteNumber, Time.time, velocity, this);
    }

    void AnimateKey()
    {
        Vector3 targetPos = isPressed
            ? originalPosition - Vector3.up * pressDepth
            : originalPosition;

        keyTransform.localPosition = Vector3.Lerp(
            keyTransform.localPosition,
            targetPos,
            Time.deltaTime * pressSpeed
        );
    }

    public void Highlight(bool active)
    {
        if (!keyRenderer) return;

        if (active && highlightMaterial)
            keyRenderer.material = highlightMaterial;
        else if (defaultMaterial)
            keyRenderer.material = defaultMaterial;
    }

    System.Collections.IEnumerator FadeOutSound(float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    void OnValidate()
    {
        if (string.IsNullOrEmpty(noteName))
            noteName = GetNoteName(noteNumber);
    }

    public static string GetNoteName(int midiNote)
    {
        string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        int octave = (midiNote / 12) - 1;
        int noteIndex = midiNote % 12;
        return noteNames[noteIndex] + octave;
    }
}