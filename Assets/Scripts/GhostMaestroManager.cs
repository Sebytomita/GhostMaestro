using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UNIFIED Story + Lesson Manager with SCORING SYSTEM
/// Ghost Maestro guides you through story AND lessons
/// Tracks accuracy, timing, combos, and star ratings
/// </summary>
public class GhostMaestroManager : MonoBehaviour
{
    // ═══════════════════════════════════════════════════════════════
    // STORY SETTINGS
    // ═══════════════════════════════════════════════════════════════

    [Header("=== STORY PHASES ===")]
    public GamePhase currentPhase = GamePhase.Welcome;

    [Header("Story Timing")]
    public float welcomeDelay = 2f;
    public float storyTextDuration = 5f;
    public float fadeSpeed = 1f;

    [Header("Camera & Player")]
    public Transform playerCamera;
    public Transform sitPosition;
    public Transform standPosition;

    [Header("Piano Reference")]
    public Transform pianoTransform;
    public float pianoApproachDistance = 2f;

    // ═══════════════════════════════════════════════════════════════
    // DESKTOP MODE (keyboard piano + movement lock)
    // ═══════════════════════════════════════════════════════════════

    [Header("=== DESKTOP MODE ===")]
    [Tooltip("Script care mapează tastele Q-M pe clapele pianului (auto-found if empty)")]
    public KeyboardPianoInput keyboardPianoInput;

    [Tooltip("Script care blochează WASD + mouse când ești la pian (auto-found if empty)")]
    public FirstPersonMovementLocker movementLocker;

    [Tooltip("Dacă e bifat, când apeși ESC ești teleportat înapoi la StandPosition")]
    public bool teleportToStandOnExit = true;

    // ═══════════════════════════════════════════════════════════════
    // UI REFERENCES (ONE UI FOR EVERYTHING)
    // ═══════════════════════════════════════════════════════════════

    [Header("=== UI REFERENCES ===")]
    public TextMeshProUGUI mainText;           // For story AND lesson instructions
    public TextMeshProUGUI progressText;       // For lesson progress
    public TextMeshProUGUI feedbackText;       // For correct/wrong feedback
    public GameObject sitDownPrompt;
    public CanvasGroup fadePanel;

    [Header("Next Button")]
    public Button nextButton;
    public TextMeshProUGUI nextButtonText;
    public GameObject nextButtonPanel;

    [Header("Completion Panel")]
    public GameObject lessonCompletePanel;
    public TextMeshProUGUI completionMessageText;

    // ═══════════════════════════════════════════════════════════════
    // SCORING SYSTEM UI
    // ═══════════════════════════════════════════════════════════════

    [Header("=== SCORING UI ===")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI timingFeedbackText;

    [Header("Star Display")]
    public GameObject[] starObjects;           // 5 star GameObjects for during lesson
    public GameObject[] endScreenStars;        // 5 stars for completion panel
    public Color starActiveColor = new Color(1f, 0.84f, 0f);
    public Color starInactiveColor = new Color(0.3f, 0.3f, 0.3f);

    [Header("End Screen Stats")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI statsBreakdownText;

    // ═══════════════════════════════════════════════════════════════
    // VISUAL & AUDIO FEEDBACK
    // ═══════════════════════════════════════════════════════════════

    [Header("=== VISUAL FEEDBACK ===")]
    public Color hintColor = new Color(1f, 0.85f, 0.3f);
    public Color correctColor = new Color(0.3f, 1f, 0.3f);
    public Color wrongColor = new Color(1f, 0.3f, 0.3f);
    public Color normalColor = new Color(0.95f, 0.95f, 0.9f);
    public Color storyColor = new Color(0.9f, 0.85f, 0.7f);

    [Header("Timing Rating Colors")]
    public Color perfectColor = new Color(1f, 0.84f, 0f);   // Gold
    public Color greatColor = new Color(0f, 1f, 0f);        // Green
    public Color goodColor = new Color(0f, 1f, 1f);         // Cyan
    public Color okColor = new Color(1f, 1f, 0f);           // Yellow
    public Color missColor = new Color(1f, 0f, 0f);         // Red

    [Header("=== AUDIO ===")]
    public AudioSource feedbackAudio;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip lessonCompleteSound;
    public AudioClip perfectSound;
    public AudioClip comboBreakSound;
    public AudioClip milestoneSound;

    [Header("=== METRONOME ===")]
    public RhythmJudge rhythmJudge;

    [Header("=== RHYTHM VISUAL ===")]
    public RhythmVisualIndicator rhythmVisual;
    public bool createRhythmVisualAutomatically = true;

    [Header("=== TIMING ZONE INDICATOR ===")]
    public TimingZoneIndicator timingZoneIndicator;

    // ═══════════════════════════════════════════════════════════════
    // SCORING SETTINGS
    // ═══════════════════════════════════════════════════════════════

    [Header("=== SCORING SETTINGS ===")]
    [Tooltip("Perfect: ±50ms from beat")]
    public float perfectWindow = 50f;
    [Tooltip("Great: ±100ms from beat")]
    public float greatWindow = 100f;
    [Tooltip("Good: ±175ms from beat")]
    public float goodWindow = 175f;
    [Tooltip("OK: ±250ms from beat")]
    public float okWindow = 250f;

    [Header("Point Values")]
    public int perfectPoints = 100;
    public int greatPoints = 75;
    public int goodPoints = 50;
    public int okPoints = 25;
    public int wrongNotePoints = -10;

    [Header("Combo Settings")]
    public float comboMultiplierStep = 0.1f;
    public float maxComboMultiplier = 3.0f;
    public int comboMilestones = 10;

    // ═══════════════════════════════════════════════════════════════
    // SCORING STATE
    // ═══════════════════════════════════════════════════════════════

    private int currentScore = 0;
    private int combo = 0;
    private int maxCombo = 0;
    private int perfectCount = 0;
    private int greatCount = 0;
    private int goodCount = 0;
    private int okCount = 0;
    private int missCount = 0;
    private int wrongNoteCount = 0;
    private int totalNotesInLesson = 0;
    private int lessonMaxPossibleScore = 0;
    private int displayedScore = 0;
    private Coroutine scoreAnimCoroutine;
    private Coroutine timingFeedbackCoroutine;

    // ═══════════════════════════════════════════════════════════════
    // LESSON STATE
    // ═══════════════════════════════════════════════════════════════

    private List<Lesson> allLessons;
    private int currentLessonIndex = 0;
    private int currentPhaseIndex = 0;
    private Lesson currentLesson;
    private LessonPhase currentLessonPhase;

    private int notesPlayedInPhase = 0;
    private int sequenceIndex = 0;
    private int correctNotesInPhase = 0;
    private int sequenceCompletions = 0;
    private bool phaseComplete = false;
    private bool waitingForNextButton = false;
    private bool lessonsActive = false;

    // Story state
#pragma warning disable 0414
    private bool isSeated = false;
#pragma warning restore 0414
    private bool canSitDown = false;
    private Coroutine currentStoryCoroutine;

    // Position lock for seated state
    private Transform lockedCameraOffset;
    private Vector3 lockedPosition;
    private bool positionLocked = false;

    // Hint system
    private Coroutine hintCoroutine;
    private PianoKeyEnhanced currentHintedKey;

    // Piano keys cache
    private Dictionary<int, PianoKeyEnhanced> pianoKeys = new Dictionary<int, PianoKeyEnhanced>();

    // ═══════════════════════════════════════════════════════════════
    // TIMING RATING ENUM
    // ═══════════════════════════════════════════════════════════════

    public enum TimingRating
    {
        Perfect,
        Great,
        Good,
        OK,
        Miss,
        WrongNote
    }

    // ═══════════════════════════════════════════════════════════════
    // INITIALIZATION
    // ═══════════════════════════════════════════════════════════════

    void Awake()
    {
        allLessons = LessonData.GetAllLessons();

        if (nextButton)
            nextButton.onClick.AddListener(OnNextButtonClicked);

        HideNextButton();
        HideLessonUI();
        HideScoreUI();
        if (lessonCompletePanel) lessonCompletePanel.SetActive(false);
        if (sitDownPrompt) sitDownPrompt.SetActive(false);
    }

    void HideLessonUI()
    {
        if (progressText) progressText.gameObject.SetActive(false);
        if (feedbackText) feedbackText.gameObject.SetActive(false);
    }

    void ShowLessonUI()
    {
        if (progressText) progressText.gameObject.SetActive(true);
        if (feedbackText) feedbackText.gameObject.SetActive(true);
    }

    void HideScoreUI()
    {
        if (scoreText)
        {
            // Hide parent panel
            if (scoreText.transform.parent != null)
                scoreText.transform.parent.gameObject.SetActive(false);
        }
        if (comboText) comboText.gameObject.SetActive(false);
        if (multiplierText) multiplierText.gameObject.SetActive(false);
        if (timingFeedbackText) timingFeedbackText.gameObject.SetActive(false);
        if (timingZoneIndicator) timingZoneIndicator.gameObject.SetActive(false);

        // ALWAYS hide stars display during lessons - stars are only for end screen
        if (starObjects != null && starObjects.Length > 0 && starObjects[0] != null)
        {
            Transform starsParent = starObjects[0].transform.parent;
            if (starsParent != null)
                starsParent.gameObject.SetActive(false);
        }
    }

    void ShowScoreUI()
    {
        if (scoreText)
        {
            scoreText.gameObject.SetActive(true);
            // Also show parent panel
            if (scoreText.transform.parent != null)
                scoreText.transform.parent.gameObject.SetActive(true);
        }
        // Combo and multiplier shown dynamically
        // Timing feedback shown on hit

        // DO NOT show stars display - stars are only for end screen
        // StarsDisplay stays hidden during gameplay
    }

    [Header("=== DEBUG ===")]
    public bool skipStoryForTesting = false;

    [Header("=== AUTO UI CREATION ===")]
    public bool createScoreUIAutomatically = false;  // DISABLED - use ScoreUICreator instead

    void Start()
    {
        CachePianoKeys();

        if (!rhythmJudge)
            rhythmJudge = FindObjectOfType<RhythmJudge>();

        // Auto-find desktop mode components if not assigned in Inspector
        if (!keyboardPianoInput)
            keyboardPianoInput = FindObjectOfType<KeyboardPianoInput>();
        if (!movementLocker)
            movementLocker = FindObjectOfType<FirstPersonMovementLocker>();

        // Make sure keyboard piano input is OFF until player sits at piano
        if (keyboardPianoInput) keyboardPianoInput.SetInputEnabled(false);

        InitializeRhythmVisual();
        InitializeScoreUI();

        if (skipStoryForTesting)
        {
            currentPhase = GamePhase.Lessons;
            lessonsActive = true;
            LoadProgress();
            StartLesson(currentLessonIndex);
        }
        else
        {
            lessonsActive = false;
            StartCoroutine(WelcomeSequence());
        }
    }

    void InitializeScoreUI()
    {
        // Score UI is now created manually via ScoreUICreator
        // Just ensure references are assigned in Inspector
        if (scoreText == null)
            Debug.LogWarning("[GhostMaestro] scoreText not assigned! Use GameObject > UI > Create Score UI");
    }

    void InitializeRhythmVisual()
    {
        if (rhythmVisual == null)
            rhythmVisual = FindObjectOfType<RhythmVisualIndicator>();

        if (rhythmVisual == null && createRhythmVisualAutomatically)
        {
            Canvas lessonCanvas = null;
            if (mainText != null)
                lessonCanvas = mainText.GetComponentInParent<Canvas>();

            if (lessonCanvas != null)
            {
                GameObject rhythmObj = new GameObject("RhythmVisualIndicator");
                rhythmObj.transform.SetParent(lessonCanvas.transform, false);
                rhythmVisual = rhythmObj.AddComponent<RhythmVisualIndicator>();
                rhythmVisual.CreateUI(lessonCanvas);

                if (mainText != null)
                    rhythmVisual.ApplyFontFromReference(mainText);

                Debug.Log("[GhostMaestro] Created RhythmVisualIndicator automatically");
            }
        }

        if (rhythmVisual != null && rhythmJudge != null)
            rhythmVisual.SetRhythmJudge(rhythmJudge);
    }

    void CachePianoKeys()
    {
        PianoKeyEnhanced[] keys = FindObjectsByType<PianoKeyEnhanced>(FindObjectsSortMode.None);
        foreach (var key in keys)
        {
            if (!pianoKeys.ContainsKey(key.noteNumber))
                pianoKeys.Add(key.noteNumber, key);
        }
        Debug.Log($"[GhostMaestro] Found {pianoKeys.Count} piano keys");
    }

    void Update()
    {
        switch (currentPhase)
        {
            case GamePhase.FreeExploration:
                CheckPianoProximity();
                break;
            case GamePhase.PianoApproach:
                CheckPianoProximity();
                CheckSitDownInput();
                break;
            case GamePhase.Seated:
            case GamePhase.Lessons:
                CheckStandUpInput();
                break;
        }

        // Debug shortcuts - F keys to avoid conflicts with piano keys (Q-M)
        if (Input.GetKeyDown(KeyCode.F2) && lessonsActive)
            SkipToNextLesson();
        if (Input.GetKeyDown(KeyCode.F3) && lessonsActive)
            StartLesson(currentLessonIndex);

        // Enter/Return = activează butonul Next dacă e vizibil
        // (alternativă la click pentru cei care prefer\u0103 tastatura)
        if (waitingForNextButton && nextButton != null && nextButton.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnNextButtonClicked();
        }
    }

    void LateUpdate()
    {
        // Legacy XR position lock - kept for backwards compatibility
        // In desktop mode, FirstPersonMovementLocker handles movement blocking via script disable
        if (positionLocked && lockedCameraOffset != null)
            lockedCameraOffset.position = lockedPosition;
    }

    void SetMovementEnabled(bool enabled)
    {
        // Desktop mode: prefer\u0103m s\u0103 folosim SetPianoMode pe FPSController-ul direct,
        // pentru c\u0103 vrem ca mouse look-ul cu right-click s\u0103 r\u0103m\u00e2n\u0103 disponibil la pian.
        SimpleDesktopFPSController fps = null;
        if (movementLocker != null)
            fps = movementLocker.GetComponent<SimpleDesktopFPSController>();
        if (fps == null)
            fps = FindObjectOfType<SimpleDesktopFPSController>();

        if (fps != null)
        {
            // enabled = mers normal (afar\u0103 de pian)
            // !enabled = la pian (WASD blocat, mouse look pe right-click)
            fps.SetPianoMode(!enabled);
            string modeStr = enabled ? "NORMAL" : "PIANO (WASD blocked, right-click to look)";
            Debug.Log($"[GhostMaestro] FPS Mode: {modeStr}");
        }
        else
        {
            // Fallback la varianta veche dac\u0103 nu g\u0103sim FPSController
            if (movementLocker != null)
            {
                if (enabled)
                    movementLocker.UnlockAll();
                else
                    movementLocker.LockAll();
            }
            else
            {
                Debug.LogWarning("[GhostMaestro] No FirstPersonMovementLocker or SimpleDesktopFPSController found!");
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // SCORING SYSTEM
    // ═══════════════════════════════════════════════════════════════

    void ResetScoreForLesson()
    {
        currentScore = 0;
        combo = 0;
        maxCombo = 0;
        perfectCount = 0;
        greatCount = 0;
        goodCount = 0;
        okCount = 0;
        missCount = 0;
        wrongNoteCount = 0;
        totalNotesInLesson = 0;
        displayedScore = 0;

        // Calculate total notes in lesson AND realistic max score per phase
        // (rhythm phases pot atinge Perfect=100, non-rhythm phases doar Great=75)
        lessonMaxPossibleScore = 0;
        int simulatedCombo = 0;

        if (currentLesson != null && currentLesson.phases != null)
        {
            foreach (var phase in currentLesson.phases)
            {
                int notesInPhase;
                if (phase.targetNotes != null)
                    notesInPhase = phase.targetNotes.Length * phase.requiredCount;
                else
                    notesInPhase = phase.requiredCount;

                totalNotesInLesson += notesInPhase;

                // Punctele maxime per not\u0103 \u00een aceast\u0103 faz\u0103: Perfect dac\u0103 e ritm, altfel Great
                bool isRhythmPhase = phase.taskType == TaskType.PlayOnBeat ||
                                     phase.taskType == TaskType.PlaySequenceOnBeat;
                int pointsPerNote = isRhythmPhase ? perfectPoints : greatPoints;

                // Acumul\u0103m scorul maxim posibil cu combo continuu
                for (int i = 0; i < notesInPhase; i++)
                {
                    float multiplier = Mathf.Min(1f + (simulatedCombo * comboMultiplierStep), maxComboMultiplier);
                    lessonMaxPossibleScore += Mathf.RoundToInt(pointsPerNote * multiplier);
                    simulatedCombo++;
                    if (simulatedCombo % comboMilestones == 0)
                        lessonMaxPossibleScore += simulatedCombo * 10;
                }
            }
        }

        UpdateScoreDisplay();
        UpdateComboDisplay();

        Debug.Log($"[Scoring] Reset for lesson. Expected notes: {totalNotesInLesson}, Max realistic score: {lessonMaxPossibleScore}");
    }

    /// <summary>
    /// (Legacy) Calcul ideal cu Perfect pentru toate notele \u2014 nu mai e folosit.
    /// P\u0103strat pentru compatibilitate dac\u0103 alt cod \u00eel apeleaz\u0103.
    /// </summary>
    int CalculateMaxPossibleScore(int noteCount)
    {
        int score = 0;
        for (int i = 0; i < noteCount; i++)
        {
            float multiplier = Mathf.Min(1f + (i * comboMultiplierStep), maxComboMultiplier);
            score += Mathf.RoundToInt(perfectPoints * multiplier);

            if ((i + 1) % comboMilestones == 0)
                score += (i + 1) * 10;
        }
        return score;
    }

    /// <summary>
    /// Evaluate timing and return rating
    /// </summary>
    TimingRating EvaluateTimingRating(float timingOffsetMs)
    {
        float absOffset = Mathf.Abs(timingOffsetMs);
        if (absOffset <= perfectWindow) return TimingRating.Perfect;
        if (absOffset <= greatWindow) return TimingRating.Great;
        if (absOffset <= goodWindow) return TimingRating.Good;
        if (absOffset <= okWindow) return TimingRating.OK;
        return TimingRating.Miss;
    }

    /// <summary>
    /// Get points for a timing rating
    /// </summary>
    int GetPointsForRating(TimingRating rating)
    {
        switch (rating)
        {
            case TimingRating.Perfect: return perfectPoints;
            case TimingRating.Great: return greatPoints;
            case TimingRating.Good: return goodPoints;
            case TimingRating.OK: return okPoints;
            default: return 0;
        }
    }

    /// <summary>
    /// Register a correct note (non-rhythm lesson)
    /// </summary>
    void RegisterCorrectNote()
    {
        TimingRating rating = TimingRating.Great; // Non-rhythm = always Great
        int points = GetPointsForRating(rating);

        // Apply combo multiplier
        float multiplier = GetCurrentMultiplier();
        int finalPoints = Mathf.RoundToInt(points * multiplier);
        currentScore += finalPoints;

        greatCount++;
        IncrementCombo();

        ShowTimingFeedback(rating, finalPoints);
        UpdateScoreDisplay();

        Debug.Log($"[Scoring] Correct note: +{finalPoints} (x{multiplier:F1}), Combo: {combo}");
    }

    /// <summary>
    /// Register a note with timing (rhythm lesson)
    /// </summary>
    void RegisterNoteWithTiming(float timingOffsetMs)
    {
        TimingRating rating = EvaluateTimingRating(timingOffsetMs);
        int points = GetPointsForRating(rating);

        // Track counts
        switch (rating)
        {
            case TimingRating.Perfect: perfectCount++; break;
            case TimingRating.Great: greatCount++; break;
            case TimingRating.Good: goodCount++; break;
            case TimingRating.OK: okCount++; break;
            case TimingRating.Miss: missCount++; break;
        }

        if (rating != TimingRating.Miss)
        {
            // Apply combo multiplier
            float multiplier = GetCurrentMultiplier();
            int finalPoints = Mathf.RoundToInt(points * multiplier);
            currentScore += finalPoints;

            IncrementCombo();
            ShowTimingFeedback(rating, finalPoints);

            // Play special sound for perfect
            if (rating == TimingRating.Perfect && feedbackAudio && perfectSound)
                feedbackAudio.PlayOneShot(perfectSound, 0.7f);

            Debug.Log($"[Scoring] {rating}: {timingOffsetMs:F1}ms, +{finalPoints} (x{multiplier:F1}), Combo: {combo}");
        }
        else
        {
            BreakCombo();
            ShowTimingFeedback(rating, 0);
            Debug.Log($"[Scoring] MISS: {timingOffsetMs:F1}ms, Combo broken");
        }

        UpdateScoreDisplay();
    }

    /// <summary>
    /// Register a wrong note
    /// </summary>
    void RegisterWrongNote()
    {
        wrongNoteCount++;
        currentScore = Mathf.Max(0, currentScore + wrongNotePoints);
        BreakCombo();

        ShowTimingFeedback(TimingRating.WrongNote, wrongNotePoints);
        UpdateScoreDisplay();

        Debug.Log($"[Scoring] WRONG NOTE: {wrongNotePoints}, Combo broken");
    }

    void IncrementCombo()
    {
        combo++;
        if (combo > maxCombo)
            maxCombo = combo;

        UpdateComboDisplay();

        // Combo milestone bonus
        if (combo > 0 && combo % comboMilestones == 0)
        {
            int bonus = combo * 10;
            currentScore += bonus;
            UpdateScoreDisplay();

            if (feedbackAudio && milestoneSound)
                feedbackAudio.PlayOneShot(milestoneSound);

            ShowMilestoneEffect(combo, bonus);
            Debug.Log($"[Scoring] COMBO MILESTONE: {combo}! Bonus: +{bonus}");
        }
    }

    void BreakCombo()
    {
        if (combo > 5 && feedbackAudio && comboBreakSound)
            feedbackAudio.PlayOneShot(comboBreakSound, 0.5f);

        combo = 0;
        UpdateComboDisplay();
    }

    float GetCurrentMultiplier()
    {
        return Mathf.Min(1f + (combo * comboMultiplierStep), maxComboMultiplier);
    }

    /// <summary>
    /// Calculate star rating for current lesson based on SCORE achieved vs theoretical max.
    /// 
    /// Scorul ia automat \u00een calcul:
    ///   - Acuratete (note corecte = +puncte, gre\u0219eli = -puncte)
    ///   - Combo (multiplier x1.0 \u2192 x3.0 \u00een func\u021bie de combo)
    ///   - Bonusuri milestone (la fiecare 10 combo)
    /// 
    /// Praguri (raport currentScore / lessonMaxPossibleScore):
    ///   5\u2605 \u2265 70%  |  4\u2605 \u2265 55%  |  3\u2605 \u2265 40%  |  2\u2605 \u2265 25%  |  1\u2605 \u2265 10%
    /// </summary>
    int CalculateStarRating()
    {
        if (totalNotesInLesson == 0) return 0;
        if (lessonMaxPossibleScore <= 0)
        {
            Debug.LogWarning("[Stars] lessonMaxPossibleScore is 0 sau negativ \u2014 nu pot calcula stele.");
            return 0;
        }

        float scoreRatio = (float)currentScore / lessonMaxPossibleScore;
        scoreRatio = Mathf.Clamp01(scoreRatio); // \u00centre 0 \u0219i 1

        // Pentru log: \u0219i acuratete (informativ, nu folosit\u0103 \u00een calcul)
        int correctNotes = perfectCount + greatCount + goodCount + okCount;
        int totalAttempts = correctNotes + missCount + wrongNoteCount;
        float accuracy = totalAttempts > 0 ? (float)correctNotes / totalAttempts : 1f;

        Debug.Log($"[Stars] Score: {currentScore}/{lessonMaxPossibleScore} ({scoreRatio:P1}) | Accuracy: {accuracy:P1} | Combo max: {maxCombo}");

        if (scoreRatio >= 0.70f) return 5;
        if (scoreRatio >= 0.55f) return 4;
        if (scoreRatio >= 0.40f) return 3;
        if (scoreRatio >= 0.25f) return 2;
        if (scoreRatio >= 0.10f) return 1;
        return 0;
    }

    // ═══════════════════════════════════════════════════════════════
    // SCORE UI UPDATES
    // ═══════════════════════════════════════════════════════════════

    void UpdateScoreDisplay()
    {
        if (scoreAnimCoroutine != null)
            StopCoroutine(scoreAnimCoroutine);
        scoreAnimCoroutine = StartCoroutine(AnimateScoreChange(currentScore));
    }

    IEnumerator AnimateScoreChange(int targetScore)
    {
        if (scoreText == null) yield break;

        int startScore = displayedScore;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - (elapsed / duration), 3f);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, t));
            scoreText.text = $"Scor: {displayedScore:N0}";
            yield return null;
        }

        displayedScore = targetScore;
        scoreText.text = $"Scor: {displayedScore:N0}";
    }

    void UpdateComboDisplay()
    {
        if (comboText)
        {
            if (combo > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = $"{combo}x COMBO";

                // Color based on combo level
                if (combo >= 50)
                    comboText.color = new Color(1f, 0f, 1f); // Purple
                else if (combo >= 30)
                    comboText.color = new Color(1f, 0.5f, 0f); // Orange
                else if (combo >= 10)
                    comboText.color = Color.yellow;
                else
                    comboText.color = Color.white;

                StartCoroutine(PopAnimation(comboText.transform, 1.3f));
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }

        if (multiplierText)
        {
            float mult = GetCurrentMultiplier();
            if (mult > 1f)
            {
                multiplierText.gameObject.SetActive(true);
                multiplierText.text = $"x{mult:F1}";
            }
            else
            {
                multiplierText.gameObject.SetActive(false);
            }
        }
    }

    void ShowTimingFeedback(TimingRating rating, int points)
    {
        if (timingFeedbackCoroutine != null)
            StopCoroutine(timingFeedbackCoroutine);
        timingFeedbackCoroutine = StartCoroutine(ShowTimingFeedbackCoroutine(rating, points));
    }

    IEnumerator ShowTimingFeedbackCoroutine(TimingRating rating, int points)
    {
        if (timingFeedbackText == null) yield break;

        timingFeedbackText.gameObject.SetActive(true);

        string text = "";
        Color color = Color.white;

        switch (rating)
        {
            case TimingRating.Perfect:
                text = $"PERFECT! +{points}";
                color = perfectColor;
                break;
            case TimingRating.Great:
                text = $"GREAT! +{points}";
                color = greatColor;
                break;
            case TimingRating.Good:
                text = $"GOOD +{points}";
                color = goodColor;
                break;
            case TimingRating.OK:
                text = $"OK +{points}";
                color = okColor;
                break;
            case TimingRating.Miss:
                text = "MISS";
                color = missColor;
                break;
            case TimingRating.WrongNote:
                text = $"WRONG! {points}";
                color = missColor;
                break;
        }

        timingFeedbackText.text = text;
        timingFeedbackText.color = color;

        // Pop animation
        timingFeedbackText.transform.localScale = Vector3.one * 1.5f;
        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            elapsed += Time.deltaTime;
            timingFeedbackText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, elapsed / 0.15f);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Fade out
        Color startColor = timingFeedbackText.color;
        elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            timingFeedbackText.color = new Color(startColor.r, startColor.g, startColor.b, 1f - (elapsed / 0.2f));
            yield return null;
        }

        timingFeedbackText.gameObject.SetActive(false);
    }

    void ShowMilestoneEffect(int milestone, int bonus)
    {
        // Flash combo text
        if (comboText)
        {
            StartCoroutine(MilestoneFlash(milestone, bonus));
        }
    }

    IEnumerator MilestoneFlash(int milestone, int bonus)
    {
        if (comboText == null) yield break;

        string originalText = comboText.text;
        Color originalColor = comboText.color;

        comboText.text = $"🔥 {milestone}x! +{bonus} 🔥";
        comboText.color = Color.white;

        for (int i = 0; i < 3; i++)
        {
            comboText.transform.localScale = Vector3.one * 1.5f;
            yield return new WaitForSeconds(0.1f);
            comboText.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.1f);
        }

        comboText.text = originalText;
        comboText.color = originalColor;
    }

    IEnumerator PopAnimation(Transform target, float scale)
    {
        if (target == null) yield break;

        target.localScale = Vector3.one * scale;
        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(Vector3.one * scale, Vector3.one, elapsed / 0.15f);
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    void UpdateStarDisplay(int stars)
    {
        if (starObjects == null) return;

        for (int i = 0; i < starObjects.Length; i++)
        {
            if (starObjects[i] == null) continue;

            // Try Image first, then TextMeshProUGUI
            Image img = starObjects[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = i < stars ? starActiveColor : starInactiveColor;
            }
            else
            {
                TextMeshProUGUI tmp = starObjects[i].GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                    tmp.color = i < stars ? starActiveColor : starInactiveColor;
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // STORY SEQUENCES
    // ═══════════════════════════════════════════════════════════════

    IEnumerator WelcomeSequence()
    {
        currentPhase = GamePhase.Welcome;
        yield return new WaitForSeconds(welcomeDelay);

        yield return ShowGhostText(
            "Bine ai venit în casa mea, musafir.\n" +
            "Sunt Maestrul Ghost - cândva, pianul din colț\n" +
            "răsuna prin aceste camere..."
        );

        yield return ShowGhostText(
            "Aici am petrecut o viață întreagă,\n" +
            "învățând și cântând.\n" +
            "Acum, casa așteaptă o melodie nouă."
        );

        yield return ShowGhostText(
            "Simte-te liber să explorezi.\n" +
            "Când ești gata, apropie-te de pian.\n" +
            "Voi fi acolo să te ghidez."
        );

        currentPhase = GamePhase.FreeExploration;
        Debug.Log("[GhostMaestro] Phase: Free Exploration");
    }

    IEnumerator ShowGhostText(string text)
    {
        if (mainText == null) yield break;

        mainText.color = storyColor;
        mainText.text = text;

        yield return FadeText(mainText, 0f, 1f);
        yield return new WaitForSeconds(storyTextDuration);
        yield return FadeText(mainText, 1f, 0f);

        mainText.text = "";
    }

    IEnumerator FadeText(TextMeshProUGUI text, float from, float to)
    {
        float elapsed = 0f;
        Color color = text.color;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(from, to, elapsed);
            text.color = color;
            yield return null;
        }

        color.a = to;
        text.color = color;
    }

    void CheckPianoProximity()
    {
        if (playerCamera == null || pianoTransform == null) return;

        Vector3 cameraPos = playerCamera.position;
        Camera mainCam = playerCamera.GetComponentInChildren<Camera>();
        if (mainCam != null)
            cameraPos = mainCam.transform.position;

        float distance = Vector3.Distance(cameraPos, pianoTransform.position);

        if (distance <= pianoApproachDistance)
        {
            if (!canSitDown) OnPianoApproached();
        }
        else
        {
            if (canSitDown) OnPianoLeft();
        }
    }

    void OnPianoApproached()
    {
        currentPhase = GamePhase.PianoApproach;
        canSitDown = true;

        if (sitDownPrompt) sitDownPrompt.SetActive(true);

        if (currentStoryCoroutine != null) StopCoroutine(currentStoryCoroutine);
        currentStoryCoroutine = StartCoroutine(ShowGhostText(
            "Ah, simți chemarea pianului...\n" +
            "Apasă [X] pentru a te așeza.\n" +
            "Hai să începem."
        ));

        Debug.Log("[GhostMaestro] Phase: Piano Approach");
    }

    void OnPianoLeft()
    {
        currentPhase = GamePhase.FreeExploration;
        canSitDown = false;

        if (sitDownPrompt) sitDownPrompt.SetActive(false);

        if (currentStoryCoroutine != null) StopCoroutine(currentStoryCoroutine);
        if (mainText) mainText.text = "";

        Debug.Log("[GhostMaestro] Player left piano area");
    }

    void CheckSitDownInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
            StartCoroutine(SitDownSequence());
    }

    IEnumerator SitDownSequence()
    {
        currentPhase = GamePhase.SittingDown;
        canSitDown = false;

        if (sitDownPrompt) sitDownPrompt.SetActive(false);

        yield return FadeScreen(0f, 1f);

        // ───────────────────────────────────────────
        // DESKTOP TELEPORT: move the player rig (parent of Main Camera)
        // ───────────────────────────────────────────
        if (playerCamera != null && sitPosition != null)
        {
            // playerCamera should be the player root (with CharacterController etc.)
            // We move it to sitPosition. The camera (child) follows automatically.
            playerCamera.position = sitPosition.position;
            playerCamera.rotation = sitPosition.rotation;

            Debug.Log($"[GhostMaestro] Teleported player to sit position: {sitPosition.position}");
        }

        // Lock WASD + mouse look
        SetMovementEnabled(false);

        // Enable keyboard piano input (Q-M = piano keys)
        if (keyboardPianoInput) keyboardPianoInput.SetInputEnabled(true);

        yield return new WaitForSeconds(0.3f);
        yield return FadeScreen(1f, 0f);

        isSeated = true;
        currentPhase = GamePhase.Seated;

        yield return new WaitForSeconds(1f);
        yield return ShowGhostText(
            "Să începem cu fundamentele.\n" +
            "Folosește tastele Q-I, A-J, Z-M pentru a cânta.\n" +
            "Urmărește clapele care strălucesc AURIU.\n\n" +
            "(Apasă ESC pentru a te ridica)"
        );

        currentPhase = GamePhase.Lessons;
        lessonsActive = true;
        LoadProgress();
        StartLesson(currentLessonIndex);
    }

    void CheckStandUpInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(StandUpSequence());
    }

    IEnumerator StandUpSequence()
    {
        currentPhase = GamePhase.StandingUp;
        lessonsActive = false;

        // ───────────────────────────────────────────
        // OPRIRE TOTAL\u0102 a oric\u0103rei activit\u0103\u021bi de lec\u021bie
        // ───────────────────────────────────────────
        // Oprim TOATE coroutine-urile lec\u021biei (StartPhaseAfterDelay, hint pulse,
        // score animations, feedback timing). Folosim StopAllCoroutines pe acest
        // GameObject EXCEPT\u00ce coroutine-ul curent (StandUpSequence) \u2014 nu putem
        // s\u0103 \u00eel oprim pe noi \u00een\u0219ine. Solu\u021bie: oprim manual fiecare coroutine
        // urm\u0103rit\u0103 \u00een variabile.
        if (hintCoroutine != null) { StopCoroutine(hintCoroutine); hintCoroutine = null; }
        if (currentStoryCoroutine != null) { StopCoroutine(currentStoryCoroutine); currentStoryCoroutine = null; }
        if (scoreAnimCoroutine != null) { StopCoroutine(scoreAnimCoroutine); scoreAnimCoroutine = null; }
        if (timingFeedbackCoroutine != null) { StopCoroutine(timingFeedbackCoroutine); timingFeedbackCoroutine = null; }

        // Stop tastatur\u0103 pian
        if (keyboardPianoInput) keyboardPianoInput.SetInputEnabled(false);

        ClearAllHighlights();

        if (rhythmJudge) rhythmJudge.StopMetronome();
        if (rhythmVisual != null) rhythmVisual.Hide();
        if (timingZoneIndicator) timingZoneIndicator.gameObject.SetActive(false);

        // ───────────────────────────────────────────
        // Resetare progres lec\u021bie + scor
        // ───────────────────────────────────────────
        currentPhaseIndex = 0;
        currentLessonPhase = null;
        notesPlayedInPhase = 0;
        sequenceIndex = 0;
        correctNotesInPhase = 0;
        sequenceCompletions = 0;
        phaseComplete = false;
        waitingForNextButton = false;

        // Reset scor afi\u0219at (ca s\u0103 nu r\u0103m\u00e2n\u0103 cifre pe ecran)
        currentScore = 0;
        combo = 0;
        displayedScore = 0;

        // ───────────────────────────────────────────
        // Ascundere COMPLET\u0102 a UI-ului de lec\u021bie
        // ───────────────────────────────────────────
        if (mainText) { mainText.text = ""; mainText.gameObject.SetActive(false); }
        if (progressText) { progressText.text = ""; progressText.gameObject.SetActive(false); }
        if (feedbackText) { feedbackText.text = ""; feedbackText.gameObject.SetActive(false); }
        if (timingFeedbackText) { timingFeedbackText.text = ""; timingFeedbackText.gameObject.SetActive(false); }
        if (comboText) { comboText.text = ""; comboText.gameObject.SetActive(false); }
        if (multiplierText) { multiplierText.text = ""; multiplierText.gameObject.SetActive(false); }
        HideScoreUI();
        if (lessonCompletePanel) lessonCompletePanel.SetActive(false);
        HideNextButton();
        if (sitDownPrompt) sitDownPrompt.SetActive(false);

        // ───────────────────────────────────────────
        // Deblocare mi\u0219care + cursor
        // ───────────────────────────────────────────
        positionLocked = false;
        lockedCameraOffset = null;
        SetMovementEnabled(true);

        yield return FadeScreen(0f, 1f);

        // ───────────────────────────────────────────
        // Teleport \u00eenapoi la StandPosition
        // ───────────────────────────────────────────
        if (teleportToStandOnExit && playerCamera != null && standPosition != null)
        {
            playerCamera.position = standPosition.position;
            playerCamera.rotation = standPosition.rotation;
            Debug.Log($"[GhostMaestro] Teleported player to stand position: {standPosition.position}");
        }

        yield return new WaitForSeconds(0.3f);
        yield return FadeScreen(1f, 0f);

        isSeated = false;
        canSitDown = false;
        currentPhase = GamePhase.FreeExploration;

        // Reactiv\u0103m mainText pentru explorare (po\u0218te deveni iar text de poveste)
        if (mainText) mainText.gameObject.SetActive(true);

        Debug.Log("[GhostMaestro] Exited lesson - lesson will restart on re-entry");
    }

    IEnumerator FadeScreen(float from, float to)
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            fadePanel.alpha = Mathf.Lerp(from, to, elapsed);
            yield return null;
        }
        fadePanel.alpha = to;
    }

    // ═══════════════════════════════════════════════════════════════
    // LESSON SYSTEM
    // ═══════════════════════════════════════════════════════════════

    public void StartLesson(int lessonIndex)
    {
        if (mainText) mainText.gameObject.SetActive(true);
        if (progressText) progressText.gameObject.SetActive(false);

        if (allLessons == null || lessonIndex < 0 || lessonIndex >= allLessons.Count)
        {
            Debug.LogError($"[GhostMaestro] Invalid lesson index: {lessonIndex}");
            return;
        }

        currentLessonIndex = lessonIndex;
        currentLesson = allLessons[lessonIndex];
        currentPhaseIndex = 0;

        // Reset scoring for this lesson
        ResetScoreForLesson();
        ShowScoreUI();

        if (mainText)
        {
            mainText.color = storyColor;
            mainText.text = $"Lecția {lessonIndex + 1}: {currentLesson.title}\n\n{currentLesson.description}";
        }

        if (lessonCompletePanel) lessonCompletePanel.SetActive(false);
        HideNextButton();

        if (currentLesson.useMetronome && rhythmJudge)
            rhythmJudge.SetBPM(currentLesson.bpm);

        Debug.Log($"[GhostMaestro] Starting Lesson {lessonIndex + 1}: {currentLesson.title}");

        StartCoroutine(StartPhaseAfterDelay(0, 2f));
    }

    IEnumerator StartPhaseAfterDelay(int phaseIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartPhase(phaseIndex);
    }

    void StartPhase(int phaseIndex)
    {
        if (currentLesson == null || currentLesson.phases == null)
        {
            Debug.LogError("[GhostMaestro] currentLesson or phases is null!");
            return;
        }

        if (phaseIndex >= currentLesson.phases.Count)
        {
            CompleteLessonPhases();
            return;
        }

        currentPhaseIndex = phaseIndex;
        currentLessonPhase = currentLesson.phases[phaseIndex];

        notesPlayedInPhase = 0;
        sequenceIndex = 0;
        correctNotesInPhase = 0;
        sequenceCompletions = 0;
        phaseComplete = false;
        waitingForNextButton = false;

        UpdatePhaseUI();
        ClearAllHighlights();

        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        bool needsRhythm = currentLessonPhase.taskType == TaskType.PlayOnBeat ||
                          currentLessonPhase.taskType == TaskType.PlaySequenceOnBeat;

        if (needsRhythm)
        {
            if (rhythmJudge) rhythmJudge.StartMetronome();
            if (progressText) progressText.gameObject.SetActive(false);

            // Show timing zone indicator for rhythm lessons
            if (timingZoneIndicator) timingZoneIndicator.gameObject.SetActive(true);

            if (rhythmVisual != null)
            {
                if (currentLessonPhase.targetNotes != null && currentLessonPhase.targetNotes.Length > 0)
                {
                    string[] noteNames = new string[currentLessonPhase.targetNotes.Length];
                    for (int i = 0; i < currentLessonPhase.targetNotes.Length; i++)
                        noteNames[i] = GetNoteName(currentLessonPhase.targetNotes[i]);

                    if (currentLessonPhase.taskType == TaskType.PlaySequenceOnBeat)
                        rhythmVisual.ShowForSequence(noteNames, currentLessonPhase.targetNotes, currentLessonPhase.requiredCount);
                    else
                        rhythmVisual.ShowForNote(noteNames[0], currentLessonPhase.targetNotes[0], 0, currentLessonPhase.requiredCount);
                }
                else
                {
                    rhythmVisual.ShowForNote("♪", 0, 0, currentLessonPhase.requiredCount);
                }
            }
        }
        else
        {
            if (rhythmJudge) rhythmJudge.StopMetronome();
            if (progressText) progressText.gameObject.SetActive(true);
            if (timingZoneIndicator) timingZoneIndicator.gameObject.SetActive(false);
            if (rhythmVisual != null) rhythmVisual.Hide();
        }

        if (currentLessonPhase.showHints)
        {
            if (currentLessonPhase.hintDelay > 0)
                hintCoroutine = StartCoroutine(HintCoroutine());
            else
                ShowHintForCurrentNote();
        }

        Debug.Log($"[GhostMaestro] Phase {phaseIndex + 1}: {currentLessonPhase.instruction}");
    }

    string GetNoteName(int midiNote)
    {
        string[] noteNames = { "DO", "DO#", "RE", "RE#", "MI", "FA", "FA#", "SOL", "SOL#", "LA", "LA#", "SI" };
        int noteIndex = midiNote % 12;
        return noteNames[noteIndex];
    }

    void UpdatePhaseUI()
    {
        if (mainText && currentLessonPhase != null)
        {
            mainText.color = hintColor;
            mainText.text = currentLessonPhase.instruction;
        }

        if (progressText)
            progressText.text = GetProgressText();

        if (feedbackText)
            feedbackText.text = "";
    }

    string GetProgressText()
    {
        if (currentLessonPhase == null) return "";

        switch (currentLessonPhase.taskType)
        {
            case TaskType.PlayAnyKeys:
                return $"Note: {notesPlayedInPhase}/{currentLessonPhase.requiredCount}";
            case TaskType.PlaySpecificNote:
                return $"Corecte: {correctNotesInPhase}/{currentLessonPhase.requiredCount}";
            case TaskType.PlaySequence:
            case TaskType.PlaySequenceOnBeat:
                if (currentLessonPhase.requiredCount > 1)
                    return $"Secvență: {sequenceIndex}/{currentLessonPhase.targetNotes?.Length ?? 0}\nRep: {sequenceCompletions}/{currentLessonPhase.requiredCount}";
                else
                    return $"Secvență: {sequenceIndex}/{currentLessonPhase.targetNotes?.Length ?? 0}";
            case TaskType.PlayOnBeat:
                return $"Beats: {correctNotesInPhase}/{currentLessonPhase.requiredCount}";
            default:
                return "";
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // NOTE INPUT WITH SCORING
    // ═══════════════════════════════════════════════════════════════

    public void OnNotePlayed(int noteNumber, float timing, float velocity, PianoKeyEnhanced key = null)
    {
        if (!lessonsActive || currentLessonPhase == null) return;
        if (phaseComplete || waitingForNextButton) return;

        notesPlayedInPhase++;

        bool isCorrect = EvaluateNoteWithScoring(noteNumber, timing);

        if (key != null)
            StartCoroutine(FlashKey(key, isCorrect));

        PlayFeedbackSound(isCorrect);

        if (progressText)
            progressText.text = GetProgressText();

        CheckPhaseCompletion();
    }

    bool EvaluateNoteWithScoring(int noteNumber, float timing)
    {
        if (currentLessonPhase == null) return true;

        bool isCorrect = false;

        switch (currentLessonPhase.taskType)
        {
            case TaskType.PlayAnyKeys:
                isCorrect = true;
                RegisterCorrectNote();
                break;

            case TaskType.PlaySpecificNote:
                if (currentLessonPhase.targetNotes != null)
                {
                    isCorrect = System.Array.IndexOf(currentLessonPhase.targetNotes, noteNumber) >= 0;
                    if (isCorrect)
                    {
                        correctNotesInPhase++;
                        RegisterCorrectNote();
                    }
                    else
                    {
                        RegisterWrongNote();
                    }
                }
                break;

            case TaskType.PlaySequence:
                if (currentLessonPhase.targetNotes != null && sequenceIndex < currentLessonPhase.targetNotes.Length)
                {
                    int expected = currentLessonPhase.targetNotes[sequenceIndex];
                    isCorrect = (noteNumber == expected);

                    if (isCorrect)
                    {
                        sequenceIndex++;
                        correctNotesInPhase++;
                        RegisterCorrectNote();

                        if (sequenceIndex >= currentLessonPhase.targetNotes.Length)
                        {
                            sequenceCompletions++;
                            sequenceIndex = 0;
                            ShowFeedback("✓ Secvență completă!", correctColor);
                        }
                        else if (currentLessonPhase.showHints)
                        {
                            ShowHintForCurrentNote();
                        }
                    }
                    else
                    {
                        RegisterWrongNote();
                        ShowFeedback($"✗ Era {GetNoteName(expected)}", wrongColor);
                        sequenceIndex = 0;
                        if (currentLessonPhase.showHints)
                            ShowHintForCurrentNote();
                    }
                }
                break;

            case TaskType.PlayOnBeat:
                bool isTarget = currentLessonPhase.targetNotes != null &&
                               System.Array.IndexOf(currentLessonPhase.targetNotes, noteNumber) >= 0;

                if (!isTarget)
                {
                    RegisterWrongNote();
                    if (rhythmVisual != null) rhythmVisual.ShowMiss();
                    break;
                }

                float timingOffsetMs = 0f;
                if (currentLessonPhase.timingRequired && rhythmJudge)
                {
                    TimingResult result = rhythmJudge.EvaluateTiming(timing);
                    timingOffsetMs = result.errorMs;
                }

                RegisterNoteWithTiming(timingOffsetMs);

                TimingRating rating = EvaluateTimingRating(timingOffsetMs);
                isCorrect = rating != TimingRating.Miss;

                if (isCorrect)
                {
                    correctNotesInPhase++;
                    if (rhythmVisual != null)
                    {
                        rhythmVisual.ShowSuccess();
                        rhythmVisual.UpdateNoteProgressDisplay(correctNotesInPhase, currentLessonPhase.requiredCount);
                    }
                }
                else
                {
                    if (rhythmVisual != null) rhythmVisual.ShowMiss();
                }
                break;

            case TaskType.PlaySequenceOnBeat:
                if (currentLessonPhase.targetNotes != null && sequenceIndex < currentLessonPhase.targetNotes.Length)
                {
                    int expected = currentLessonPhase.targetNotes[sequenceIndex];
                    bool noteMatch = (noteNumber == expected);

                    if (!noteMatch)
                    {
                        RegisterWrongNote();
                        if (rhythmVisual != null)
                        {
                            rhythmVisual.ShowMiss();
                            if (currentLessonPhase.targetNotes.Length > 0)
                            {
                                int firstNote = currentLessonPhase.targetNotes[0];
                                rhythmVisual.UpdateCurrentNote(GetNoteName(firstNote), firstNote, 0,
                                    currentLessonPhase.targetNotes.Length, sequenceCompletions, currentLessonPhase.requiredCount);
                            }
                        }
                        sequenceIndex = 0;
                        if (currentLessonPhase.showHints) ShowHintForCurrentNote();
                        break;
                    }

                    // Note is correct, evaluate timing
                    float seqTimingOffsetMs = 0f;
                    if (currentLessonPhase.timingRequired && rhythmJudge)
                    {
                        TimingResult result = rhythmJudge.EvaluateTiming(timing);
                        seqTimingOffsetMs = result.errorMs;
                    }

                    RegisterNoteWithTiming(seqTimingOffsetMs);

                    TimingRating seqRating = EvaluateTimingRating(seqTimingOffsetMs);
                    isCorrect = seqRating != TimingRating.Miss;

                    if (isCorrect)
                    {
                        sequenceIndex++;
                        correctNotesInPhase++;

                        if (rhythmVisual != null) rhythmVisual.ShowSuccess();

                        if (sequenceIndex >= currentLessonPhase.targetNotes.Length)
                        {
                            sequenceCompletions++;
                            sequenceIndex = 0;

                            if (rhythmVisual != null && currentLessonPhase.targetNotes.Length > 0)
                            {
                                int firstNote = currentLessonPhase.targetNotes[0];
                                rhythmVisual.UpdateCurrentNote(GetNoteName(firstNote), firstNote, 0,
                                    currentLessonPhase.targetNotes.Length, sequenceCompletions, currentLessonPhase.requiredCount);
                            }
                        }
                        else
                        {
                            if (rhythmVisual != null)
                            {
                                int nextNote = currentLessonPhase.targetNotes[sequenceIndex];
                                rhythmVisual.UpdateCurrentNote(GetNoteName(nextNote), nextNote, sequenceIndex,
                                    currentLessonPhase.targetNotes.Length, sequenceCompletions, currentLessonPhase.requiredCount);
                            }
                            if (currentLessonPhase.showHints) ShowHintForCurrentNote();
                        }
                    }
                    else
                    {
                        // Timing miss
                        if (rhythmVisual != null)
                        {
                            rhythmVisual.ShowMiss();
                            if (currentLessonPhase.targetNotes.Length > 0)
                            {
                                int firstNote = currentLessonPhase.targetNotes[0];
                                rhythmVisual.UpdateCurrentNote(GetNoteName(firstNote), firstNote, 0,
                                    currentLessonPhase.targetNotes.Length, sequenceCompletions, currentLessonPhase.requiredCount);
                            }
                        }
                        sequenceIndex = 0;
                        if (currentLessonPhase.showHints) ShowHintForCurrentNote();
                    }
                }
                break;
        }

        return isCorrect;
    }

    void CheckPhaseCompletion()
    {
        if (currentLessonPhase == null) return;

        bool complete = false;

        switch (currentLessonPhase.taskType)
        {
            case TaskType.PlayAnyKeys:
                complete = notesPlayedInPhase >= currentLessonPhase.requiredCount;
                break;
            case TaskType.PlaySpecificNote:
            case TaskType.PlayOnBeat:
                complete = correctNotesInPhase >= currentLessonPhase.requiredCount;
                break;
            case TaskType.PlaySequence:
            case TaskType.PlaySequenceOnBeat:
                complete = sequenceCompletions >= currentLessonPhase.requiredCount;
                break;
        }

        if (complete && !phaseComplete)
            PhaseCompleted();
    }

    void PhaseCompleted()
    {
        phaseComplete = true;

        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        ClearAllHighlights();

        if (rhythmJudge) rhythmJudge.StopMetronome();
        if (rhythmVisual != null) rhythmVisual.Hide();
        if (timingZoneIndicator) timingZoneIndicator.gameObject.SetActive(false);

        if (currentPhaseIndex + 1 < currentLesson.phases.Count)
        {
            ShowFeedback("✓ Foarte bine!", correctColor);
            StartCoroutine(StartPhaseAfterDelay(currentPhaseIndex + 1, 1.5f));
        }
        else
        {
            ShowLessonComplete();
        }

        Debug.Log($"[GhostMaestro] Phase {currentPhaseIndex + 1} completed!");
    }

    void CompleteLessonPhases()
    {
        ShowLessonComplete();
    }

    void ShowLessonComplete()
    {
        waitingForNextButton = true;

        if (progressText) progressText.gameObject.SetActive(false);
        HideScoreUI();

        if (feedbackAudio && lessonCompleteSound)
            feedbackAudio.PlayOneShot(lessonCompleteSound);

        if (mainText) mainText.gameObject.SetActive(false);

        if (lessonCompletePanel) lessonCompletePanel.SetActive(true);

        // Calculate and display final stats
        int stars = CalculateStarRating();
        float accuracy = (float)(perfectCount + greatCount + goodCount + okCount) /
                        Mathf.Max(1, perfectCount + greatCount + goodCount + okCount + missCount + wrongNoteCount);

        if (completionMessageText)
        {
            completionMessageText.text = currentLesson.completionMessage;
        }

        // Show final score (just the number, label is separate)
        if (finalScoreText)
            finalScoreText.text = $"{currentScore:N0}";

        if (accuracyText)
            accuracyText.text = $"Acuratețe: {accuracy:P1}";

        if (maxComboText)
            maxComboText.text = $"Max Combo: {maxCombo}x";

        if (statsBreakdownText)
        {
            statsBreakdownText.text = $"Perfect: {perfectCount} | Great: {greatCount} | Good: {goodCount} | OK: {okCount} | Miss: {missCount} | Wrong: {wrongNoteCount}";
        }

        // Animate stars
        if (endScreenStars != null)
            StartCoroutine(AnimateEndScreenStars(stars));

        ShowNextButton();
        SaveProgress();
        SaveHighScore();

        Debug.Log($"[GhostMaestro] ✅ Lesson {currentLessonIndex + 1} COMPLETE! Score: {currentScore}, Stars: {stars}");
    }

    IEnumerator AnimateEndScreenStars(int starCount)
    {
        // Hide all stars first
        foreach (var star in endScreenStars)
        {
            if (star != null)
            {
                star.transform.localScale = Vector3.zero;

                Image img = star.GetComponent<Image>();
                if (img != null)
                    img.color = starInactiveColor;
                else
                {
                    TextMeshProUGUI tmp = star.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                        tmp.color = starInactiveColor;
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Animate each earned star
        for (int i = 0; i < starCount && i < endScreenStars.Length; i++)
        {
            if (endScreenStars[i] == null) continue;

            Image img = endScreenStars[i].GetComponent<Image>();
            if (img != null)
                img.color = starActiveColor;
            else
            {
                TextMeshProUGUI tmp = endScreenStars[i].GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                    tmp.color = starActiveColor;
            }

            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.3f;
                float scale = 1f + 0.3f * Mathf.Sin(t * Mathf.PI);
                endScreenStars[i].transform.localScale = Vector3.one * scale * t;
                yield return null;
            }

            endScreenStars[i].transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.2f);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // NEXT BUTTON
    // ═══════════════════════════════════════════════════════════════

    void ShowNextButton()
    {
        if (nextButtonPanel) nextButtonPanel.SetActive(true);
        if (nextButton) nextButton.gameObject.SetActive(true);

        if (nextButtonText)
        {
            if (currentLessonIndex + 1 >= allLessons.Count)
                nextButtonText.text = "🎉 FINALIZARE 🎉";
            else
                nextButtonText.text = $"▶ Lecția {currentLessonIndex + 2}";
        }

        // Eliberează cursorul ca jucătorul să poată face click pe buton
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HideNextButton()
    {
        if (nextButtonPanel) nextButtonPanel.SetActive(false);
        if (nextButton) nextButton.gameObject.SetActive(false);
    }

    void OnNextButtonClicked()
    {
        HideNextButton();
        if (lessonCompletePanel) lessonCompletePanel.SetActive(false);

        // Cursorul rămâne LIBER deoarece următoarea lecție tot UI-based e
        // (mouse-ul e necesar doar dacă vrei să dai click pe clape — dar tu folosești tastatura)
        // Dacă vrei să blochezi mouse-ul înapoi când începe noua lecție, decomentează:
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        if (currentLessonIndex + 1 < allLessons.Count)
            StartLesson(currentLessonIndex + 1);
        else
            AllLessonsComplete();
    }

    void AllLessonsComplete()
    {
        lessonsActive = false;

        if (mainText)
        {
            mainText.gameObject.SetActive(true);
            mainText.color = storyColor;
            mainText.text = "🎹 FELICITĂRI! 🎹\n\n" +
                           "Ai terminat toate lecțiile!\n" +
                           "Acum ești un pianist adevărat.\n\n" +
                           "Pianul e al tău. Cântă din inimă.";
        }

        Debug.Log("[GhostMaestro] 🎉 ALL LESSONS COMPLETE!");
    }

    public void SkipToNextLesson()
    {
        ShowLessonComplete();
    }

    // ═══════════════════════════════════════════════════════════════
    // HINT SYSTEM
    // ═══════════════════════════════════════════════════════════════

    IEnumerator HintCoroutine()
    {
        yield return new WaitForSeconds(currentLessonPhase.hintDelay);

        while (!phaseComplete)
        {
            ShowHintForCurrentNote();
            yield return new WaitForSeconds(3f);
        }
    }

    void ShowHintForCurrentNote()
    {
        ClearAllHighlights();

        if (currentLessonPhase?.targetNotes == null || currentLessonPhase.targetNotes.Length == 0)
            return;

        int noteToShow = 0;

        switch (currentLessonPhase.taskType)
        {
            case TaskType.PlaySpecificNote:
            case TaskType.PlayOnBeat:
                noteToShow = currentLessonPhase.targetNotes[0];
                break;
            case TaskType.PlaySequence:
            case TaskType.PlaySequenceOnBeat:
                if (sequenceIndex < currentLessonPhase.targetNotes.Length)
                    noteToShow = currentLessonPhase.targetNotes[sequenceIndex];
                break;
        }

        if (noteToShow > 0 && pianoKeys.ContainsKey(noteToShow))
        {
            currentHintedKey = pianoKeys[noteToShow];
            StartCoroutine(PulseKey(currentHintedKey));
        }
    }

    IEnumerator PulseKey(PianoKeyEnhanced key)
    {
        if (key?.keyRenderer == null) yield break;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration && currentHintedKey == key && !phaseComplete)
        {
            elapsed += Time.deltaTime;
            float pulse = Mathf.Sin(elapsed * 4f) * 0.5f + 0.5f;
            key.keyRenderer.material.color = Color.Lerp(normalColor, hintColor, pulse);
            yield return null;
        }

        if (key.keyRenderer && currentHintedKey == key)
            key.keyRenderer.material.color = normalColor;
    }

    void ClearAllHighlights()
    {
        currentHintedKey = null;
        foreach (var kvp in pianoKeys)
        {
            if (kvp.Value?.keyRenderer != null)
                kvp.Value.keyRenderer.material.color = normalColor;
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // FEEDBACK
    // ═══════════════════════════════════════════════════════════════

    IEnumerator FlashKey(PianoKeyEnhanced key, bool correct)
    {
        if (key?.keyRenderer == null) yield break;

        key.keyRenderer.material.color = correct ? correctColor : wrongColor;
        yield return new WaitForSeconds(0.2f);
        key.keyRenderer.material.color = normalColor;
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            StartCoroutine(ClearFeedbackAfterDelay(2f));
        }
    }

    IEnumerator ClearFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackText && !phaseComplete)
            feedbackText.text = "";
    }

    void PlayFeedbackSound(bool correct)
    {
        if (!feedbackAudio) return;
        AudioClip clip = correct ? correctSound : wrongSound;
        if (clip) feedbackAudio.PlayOneShot(clip, 0.5f);
    }

    // ═══════════════════════════════════════════════════════════════
    // SAVE / LOAD
    // ═══════════════════════════════════════════════════════════════

    void SaveProgress()
    {
        PlayerPrefs.SetInt("LESSON_PROGRESS", currentLessonIndex + 1);
        PlayerPrefs.Save();
    }

    void SaveHighScore()
    {
        string key = $"LESSON_{currentLessonIndex}_HIGHSCORE";
        int previousHigh = PlayerPrefs.GetInt(key, 0);
        if (currentScore > previousHigh)
        {
            PlayerPrefs.SetInt(key, currentScore);

            string starsKey = $"LESSON_{currentLessonIndex}_STARS";
            PlayerPrefs.SetInt(starsKey, CalculateStarRating());

            PlayerPrefs.Save();
            Debug.Log($"[Scoring] New high score for Lesson {currentLessonIndex + 1}: {currentScore}");
        }
    }

    void LoadProgress()
    {
        currentLessonIndex = PlayerPrefs.GetInt("LESSON_PROGRESS", 0);
        if (currentLessonIndex >= allLessons.Count)
            currentLessonIndex = allLessons.Count - 1;
    }

    public int GetLessonHighScore(int lessonIndex)
    {
        return PlayerPrefs.GetInt($"LESSON_{lessonIndex}_HIGHSCORE", 0);
    }

    public int GetLessonStars(int lessonIndex)
    {
        return PlayerPrefs.GetInt($"LESSON_{lessonIndex}_STARS", 0);
    }

    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("LESSON_PROGRESS");

        // Also reset all high scores
        for (int i = 0; i < 50; i++)
        {
            PlayerPrefs.DeleteKey($"LESSON_{i}_HIGHSCORE");
            PlayerPrefs.DeleteKey($"LESSON_{i}_STARS");
        }

        currentLessonIndex = 0;
        Debug.Log("[GhostMaestro] Progress and high scores reset!");
    }
}

public enum GamePhase
{
    Welcome,
    FreeExploration,
    PianoApproach,
    SittingDown,
    Seated,
    Lessons,
    StandingUp
}