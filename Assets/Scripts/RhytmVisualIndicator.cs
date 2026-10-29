using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Visual rhythm indicator - shows a moving arrow that indicates when to press keys
/// Arrow moves from left to right, player should press when arrow reaches the target zone
/// </summary>
public class RhythmVisualIndicator : MonoBehaviour
{
    [Header("=== UI REFERENCES ===")]
    [Tooltip("The panel containing all rhythm UI elements")]
    public GameObject rhythmPanel;

    [Tooltip("The background bar where the arrow moves")]
    public RectTransform trackBar;

    [Tooltip("The target zone (center rectangle where player should hit)")]
    public RectTransform targetZone;

    [Tooltip("The moving arrow/indicator")]
    public RectTransform arrow;

    [Tooltip("Text showing the note name to play")]
    public TextMeshProUGUI noteNameText;

    [Tooltip("Text showing note progress (e.g., 3/8)")]
    public TextMeshProUGUI noteProgressText;

    [Tooltip("Text showing sequence progress (e.g., 1/2)")]
    public TextMeshProUGUI sequenceProgressText;

    [Tooltip("Optional: Image component of the arrow for color feedback")]
    public Image arrowImage;

    [Header("=== TEXT COLORS ===")]
    public Color noteNameColor = new Color(0.3f, 1f, 0.5f);        // Verde lime pentru notă
    public Color noteProgressColor = new Color(1f, 0.85f, 0.3f);   // Galben-auriu pentru progres notă
    public Color sequenceProgressColor = new Color(0.5f, 0.8f, 1f); // Albastru deschis pentru secvență

    [Header("=== SETTINGS ===")]
    [Tooltip("Time in seconds for arrow to travel across the bar")]
    public float travelTime = 4f;

    [Tooltip("Color when arrow is in target zone")]
    public Color targetColor = Color.green;

    [Tooltip("Color when arrow is outside target zone")]
    public Color normalColor = Color.white;

    [Tooltip("Color flash on successful hit")]
    public Color successColor = new Color(0.3f, 1f, 0.3f);

    [Tooltip("Color flash on miss")]
    public Color missColor = new Color(1f, 0.3f, 0.3f);

    // Internal state
    private float arrowProgress = 0f; // 0 = left, 1 = right
    private bool isPlaying = false;
    private float barWidth;
    private float arrowStartX;
    private float arrowEndX;
    private float targetZoneStart;
    private float targetZoneEnd;

    // Callback when arrow reaches target zone
    public System.Action OnTargetReached;
    public System.Action OnBeatMissed;

    // Current note being displayed
    private string currentNoteName = "";
    private int currentNoteNumber = 0;

    // Sequence tracking
    private string[] sequenceNoteNames;
    private int[] sequenceNoteNumbers;
    private int currentSequenceIndex = 0;
    private int totalSequenceNotes = 0;
    private int currentCompletions = 0;
    private int totalRequiredCompletions = 1;

    // Sync with metronome
    private RhythmJudge rhythmJudge;
    private bool syncWithMetronome = false;

    void Awake()
    {
        if (rhythmPanel != null)
            rhythmPanel.SetActive(false);
    }

    void Start()
    {
        CalculatePositions();
    }

    void CalculatePositions()
    {
        if (trackBar == null || arrow == null) return;

        barWidth = trackBar.rect.width;
        arrowStartX = -barWidth / 2f;
        arrowEndX = barWidth / 2f;

        if (targetZone != null)
        {
            // Target zone position relative to track bar center
            float targetCenterX = targetZone.anchoredPosition.x;
            float targetWidth = targetZone.rect.width;
            targetZoneStart = (targetCenterX - targetWidth / 2f + barWidth / 2f) / barWidth;
            targetZoneEnd = (targetCenterX + targetWidth / 2f + barWidth / 2f) / barWidth;
        }
        else
        {
            // Default: target zone is center 20%
            targetZoneStart = 0.4f;
            targetZoneEnd = 0.6f;
        }
    }

    void Update()
    {
        if (!isPlaying) return;

        if (syncWithMetronome && rhythmJudge != null)
        {
            // Sync arrow with metronome beat
            float beatInterval = 60f / rhythmJudge.bpm;
            float timeSinceLastBeat = rhythmJudge.GetTimeSinceLastBeat();

            // We want arrow to be at TARGET ZONE CENTER (0.5) exactly when beat happens (timeSinceLastBeat = 0)
            // Arrow travels from 0 to 1 during one beat interval
            // So we offset: when timeSinceLastBeat=0, arrowProgress should be 0.5 (center)
            float targetCenter = 0.5f;
            arrowProgress = (timeSinceLastBeat / beatInterval) + targetCenter;

            // Wrap around if > 1
            if (arrowProgress > 1f)
                arrowProgress = arrowProgress - 1f;
        }
        else
        {
            // Original behavior - move independently
            arrowProgress += Time.deltaTime / travelTime;

            if (arrowProgress >= 1f)
                arrowProgress = 0f;
        }

        // Update arrow position
        float xPos = Mathf.Lerp(arrowStartX, arrowEndX, arrowProgress);
        arrow.anchoredPosition = new Vector2(xPos, arrow.anchoredPosition.y);

        // Update arrow color based on position
        if (arrowImage != null)
        {
            if (IsInTargetZone())
                arrowImage.color = targetColor;
            else
                arrowImage.color = normalColor;
        }
    }

    /// <summary>
    /// Show the rhythm indicator for a specific note
    /// </summary>
    public void ShowForNote(string noteName, int noteNumber, int currentIndex = 0, int totalNotes = 1)
    {
        currentNoteName = noteName;
        currentNoteNumber = noteNumber;

        UpdateNoteDisplay(noteName);
        UpdateNoteProgressDisplay(0, totalNotes);
        HideSequenceProgress();

        if (rhythmPanel != null)
            rhythmPanel.SetActive(true);

        CalculatePositions();
        RestartArrow();
        isPlaying = true;

        Debug.Log($"[RhythmVisual] Showing: {noteName} ({currentIndex + 1}/{totalNotes})");
    }

    /// <summary>
    /// Show the rhythm indicator for a sequence - shows first note
    /// </summary>
    public void ShowForSequence(string[] noteNames, int[] noteNumbers, int requiredCompletions)
    {
        if (noteNames == null || noteNames.Length == 0) return;

        sequenceNoteNames = noteNames;
        sequenceNoteNumbers = noteNumbers;
        currentSequenceIndex = 0;
        totalSequenceNotes = noteNames.Length;
        totalRequiredCompletions = requiredCompletions;
        currentCompletions = 0;

        // Show first note
        UpdateNoteDisplay(noteNames[0]);
        UpdateNoteProgressDisplay(1, noteNames.Length);
        UpdateSequenceProgressDisplay(0, requiredCompletions);

        if (rhythmPanel != null)
            rhythmPanel.SetActive(true);

        CalculatePositions();
        RestartArrow();
        isPlaying = true;
    }

    /// <summary>
    /// Update just the note name display
    /// </summary>
    void UpdateNoteDisplay(string noteName)
    {
        if (noteNameText != null)
        {
            noteNameText.text = noteName;
            noteNameText.color = noteNameColor;
        }
    }

    /// <summary>
    /// Update the note progress display (e.g., "3/8")
    /// </summary>
    public void UpdateNoteProgressDisplay(int current, int total)
    {
        if (noteProgressText != null)
        {
            noteProgressText.text = $"Nota: {current}/{total}";
            noteProgressText.color = noteProgressColor;
        }
    }

    /// <summary>
    /// Update the sequence progress display (e.g., "1/2")
    /// </summary>
    void UpdateSequenceProgressDisplay(int current, int total)
    {
        if (sequenceProgressText != null)
        {
            sequenceProgressText.text = $"Secvență: {current}/{total}";
            sequenceProgressText.color = sequenceProgressColor;
            sequenceProgressText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hide sequence progress (for single note exercises)
    /// </summary>
    void HideSequenceProgress()
    {
        if (sequenceProgressText != null)
        {
            sequenceProgressText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Update the displayed note (for sequences) - called from GhostMaestroManager
    /// </summary>
    public void UpdateCurrentNote(string noteName, int noteNumber, int currentIndex, int totalNotes, int completions, int requiredCompletions)
    {
        currentNoteName = noteName;
        currentNoteNumber = noteNumber;
        currentSequenceIndex = currentIndex;
        currentCompletions = completions;

        UpdateNoteDisplay(noteName);
        UpdateNoteProgressDisplay(currentIndex + 1, totalNotes);
        UpdateSequenceProgressDisplay(completions, requiredCompletions);

        // Restart arrow for new note
        RestartArrow();
    }

    /// <summary>
    /// Hide the rhythm indicator
    /// </summary>
    public void Hide()
    {
        isPlaying = false;

        if (rhythmPanel != null)
            rhythmPanel.SetActive(false);

        Debug.Log("[RhythmVisual] Hidden");
    }

    /// <summary>
    /// Check if the arrow is currently in the target zone
    /// </summary>
    public bool IsInTargetZone()
    {
        return arrowProgress >= targetZoneStart && arrowProgress <= targetZoneEnd;
    }

    /// <summary>
    /// Get how close to perfect the timing is (0 = perfect, 1 = edge of zone)
    /// </summary>
    public float GetTimingAccuracy()
    {
        float targetCenter = (targetZoneStart + targetZoneEnd) / 2f;
        float distanceFromCenter = Mathf.Abs(arrowProgress - targetCenter);
        float zoneHalfWidth = (targetZoneEnd - targetZoneStart) / 2f;

        return Mathf.Clamp01(distanceFromCenter / zoneHalfWidth);
    }

    /// <summary>
    /// Called when player successfully hits on beat
    /// </summary>
    public void ShowSuccess()
    {
        if (arrowImage != null)
        {
            StartCoroutine(FlashColor(successColor));
        }
    }

    /// <summary>
    /// Called when player misses the beat
    /// </summary>
    public void ShowMiss()
    {
        if (arrowImage != null)
        {
            StartCoroutine(FlashColor(missColor));
        }
    }

    /// <summary>
    /// Restart arrow from the left
    /// </summary>
    public void RestartArrow()
    {
        arrowProgress = 0f;
        if (arrow != null)
        {
            arrow.anchoredPosition = new Vector2(arrowStartX, arrow.anchoredPosition.y);
        }
    }

    /// <summary>
    /// Set the tempo (adjusts travel time)
    /// </summary>
    public void SetTempo(float bpm)
    {
        // Travel time = time for one beat
        // At 60 BPM, one beat = 1 second
        travelTime = 60f / bpm;
    }

    /// <summary>
    /// Sync with external metronome
    /// </summary>
    public void SyncWithBeat(float beatProgress)
    {
        // beatProgress: 0 = beat just happened, 1 = next beat about to happen
        // We want arrow in target zone at beatProgress = 0
        // So arrow progress should be at target center when beatProgress wraps

        float targetCenter = (targetZoneStart + targetZoneEnd) / 2f;
        arrowProgress = (beatProgress + targetCenter) % 1f;

        float xPos = Mathf.Lerp(arrowStartX, arrowEndX, arrowProgress);
        if (arrow != null)
        {
            arrow.anchoredPosition = new Vector2(xPos, arrow.anchoredPosition.y);
        }
    }

    /// <summary>
    /// Link to RhythmJudge for beat synchronization
    /// </summary>
    public void SetRhythmJudge(RhythmJudge judge)
    {
        rhythmJudge = judge;
        syncWithMetronome = (judge != null);

        if (syncWithMetronome)
        {
            // Calculate travel time to match beat interval
            // Arrow should travel full width in exactly one beat
            travelTime = 60f / judge.bpm;
            Debug.Log($"[RhythmVisual] Synced with metronome at {judge.bpm} BPM, travelTime={travelTime:F2}s");
        }
    }

    private System.Collections.IEnumerator FlashColor(Color flashColor)
    {
        if (arrowImage == null) yield break;

        Color originalColor = arrowImage.color;
        arrowImage.color = flashColor;

        yield return new WaitForSeconds(0.15f);

        arrowImage.color = originalColor;
    }

    /// <summary>
    /// Create the UI programmatically if not set up in scene
    /// Call this from GhostMaestroManager if rhythmPanel is null
    /// </summary>
    public void CreateUI(Canvas parentCanvas)
    {
        if (parentCanvas == null) return;

        // Load overlay material for rendering in front of 3D objects
        Material overlayMat = Resources.Load<Material>("UI_Overlay");
        if (overlayMat == null)
            Debug.LogWarning("[RhythmVisual] UI_Overlay material not found in Resources folder!");

        // Create main panel
        GameObject panel = new GameObject("RhythmIndicatorPanel");
        panel.transform.SetParent(parentCanvas.transform, false);
        rhythmPanel = panel;

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.3f);
        panelRect.anchorMax = new Vector2(0.5f, 0.3f);
        panelRect.sizeDelta = new Vector2(600, 200);
        panelRect.anchoredPosition = Vector2.zero;

        // Create track bar (background)
        GameObject track = new GameObject("TrackBar");
        track.transform.SetParent(panel.transform, false);
        trackBar = track.AddComponent<RectTransform>();
        trackBar.anchorMin = new Vector2(0.5f, 0.5f);
        trackBar.anchorMax = new Vector2(0.5f, 0.5f);
        trackBar.sizeDelta = new Vector2(500, 20);
        trackBar.anchoredPosition = new Vector2(0, 0);

        Image trackImage = track.AddComponent<Image>();
        trackImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        if (overlayMat != null) trackImage.material = overlayMat;

        // Create target zone (center rectangle)
        GameObject target = new GameObject("TargetZone");
        target.transform.SetParent(track.transform, false);
        targetZone = target.AddComponent<RectTransform>();
        targetZone.anchorMin = new Vector2(0.5f, 0.5f);
        targetZone.anchorMax = new Vector2(0.5f, 0.5f);
        targetZone.sizeDelta = new Vector2(80, 30);
        targetZone.anchoredPosition = Vector2.zero;

        Image targetImage = target.AddComponent<Image>();
        targetImage.color = new Color(0.3f, 0.8f, 0.3f, 0.5f);
        if (overlayMat != null) targetImage.material = overlayMat;

        // Create arrow
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.SetParent(track.transform, false);
        arrow = arrowObj.AddComponent<RectTransform>();
        arrow.anchorMin = new Vector2(0.5f, 0.5f);
        arrow.anchorMax = new Vector2(0.5f, 0.5f);
        arrow.sizeDelta = new Vector2(30, 40);
        arrow.anchoredPosition = new Vector2(-250, 0);

        arrowImage = arrowObj.AddComponent<Image>();
        arrowImage.color = Color.white;
        if (overlayMat != null) arrowImage.material = overlayMat;

        // ═══════════════════════════════════════════════════════════════
        // TEXT ELEMENTS - POSITIONED BELOW THE BAR
        // ═══════════════════════════════════════════════════════════════

        // Create NOTE NAME text (biggest, most prominent) - e.g., "DO"
        GameObject noteNameObj = new GameObject("NoteNameText");
        noteNameObj.transform.SetParent(panel.transform, false);
        RectTransform noteNameRect = noteNameObj.AddComponent<RectTransform>();
        noteNameRect.anchorMin = new Vector2(0.5f, 0.5f);
        noteNameRect.anchorMax = new Vector2(0.5f, 0.5f);
        noteNameRect.sizeDelta = new Vector2(300, 70);
        noteNameRect.anchoredPosition = new Vector2(0, -50); // Below bar

        noteNameText = noteNameObj.AddComponent<TextMeshProUGUI>();
        noteNameText.text = "DO";
        noteNameText.fontSize = 56;
        noteNameText.fontStyle = FontStyles.Bold;
        noteNameText.alignment = TextAlignmentOptions.Center;
        noteNameText.color = noteNameColor;

        // Create NOTE PROGRESS text - e.g., "Nota: 3/8"
        GameObject noteProgressObj = new GameObject("NoteProgressText");
        noteProgressObj.transform.SetParent(panel.transform, false);
        RectTransform noteProgressRect = noteProgressObj.AddComponent<RectTransform>();
        noteProgressRect.anchorMin = new Vector2(0.5f, 0.5f);
        noteProgressRect.anchorMax = new Vector2(0.5f, 0.5f);
        noteProgressRect.sizeDelta = new Vector2(300, 40);
        noteProgressRect.anchoredPosition = new Vector2(0, -100); // Below note name

        noteProgressText = noteProgressObj.AddComponent<TextMeshProUGUI>();
        noteProgressText.text = "Nota: 1/8";
        noteProgressText.fontSize = 32;
        noteProgressText.alignment = TextAlignmentOptions.Center;
        noteProgressText.color = noteProgressColor;

        // Create SEQUENCE PROGRESS text - e.g., "Secvență: 1/2"
        GameObject seqProgressObj = new GameObject("SequenceProgressText");
        seqProgressObj.transform.SetParent(panel.transform, false);
        RectTransform seqProgressRect = seqProgressObj.AddComponent<RectTransform>();
        seqProgressRect.anchorMin = new Vector2(0.5f, 0.5f);
        seqProgressRect.anchorMax = new Vector2(0.5f, 0.5f);
        seqProgressRect.sizeDelta = new Vector2(300, 40);
        seqProgressRect.anchoredPosition = new Vector2(0, -140); // Below note progress

        sequenceProgressText = seqProgressObj.AddComponent<TextMeshProUGUI>();
        sequenceProgressText.text = "Secvență: 0/2";
        sequenceProgressText.fontSize = 28;
        sequenceProgressText.alignment = TextAlignmentOptions.Center;
        sequenceProgressText.color = sequenceProgressColor;

        // Calculate positions
        CalculatePositions();

        // Hide by default
        panel.SetActive(false);

        Debug.Log("[RhythmVisual] UI created programmatically with overlay materials");
    }

    /// <summary>
    /// Apply font and material from a reference TMP text (call after CreateUI)
    /// </summary>
    public void ApplyFontFromReference(TextMeshProUGUI referenceText)
    {
        if (referenceText == null) return;

        TMP_FontAsset font = referenceText.font;
        Material mat = referenceText.fontSharedMaterial;

        if (noteNameText != null)
        {
            noteNameText.font = font;
            if (mat != null) noteNameText.fontSharedMaterial = mat;
        }

        if (noteProgressText != null)
        {
            noteProgressText.font = font;
            if (mat != null) noteProgressText.fontSharedMaterial = mat;
        }

        if (sequenceProgressText != null)
        {
            sequenceProgressText.font = font;
            if (mat != null) sequenceProgressText.fontSharedMaterial = mat;
        }

        Debug.Log("[RhythmVisual] Applied font and material from reference text");
    }
}