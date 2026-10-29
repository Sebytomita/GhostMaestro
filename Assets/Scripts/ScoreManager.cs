using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Comprehensive Scoring System for Ghost Maestro VR Piano
/// Tracks accuracy, timing, combos, and calculates star ratings
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Current Session Stats")]
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int combo = 0;
    [SerializeField] private int maxCombo = 0;
    [SerializeField] private int perfectCount = 0;
    [SerializeField] private int greatCount = 0;
    [SerializeField] private int goodCount = 0;
    [SerializeField] private int okCount = 0;
    [SerializeField] private int missCount = 0;
    [SerializeField] private int totalNotes = 0;

    [Header("Timing Zones (milliseconds)")]
    [Tooltip("Perfect: ±50ms from beat")]
    public float perfectWindow = 50f;
    [Tooltip("Great: ±100ms from beat")]
    public float greatWindow = 100f;
    [Tooltip("Good: ±175ms from beat")]
    public float goodWindow = 175f;
    [Tooltip("OK: ±250ms from beat")]
    public float okWindow = 250f;
    // Beyond OK window = Miss

    [Header("Point Values")]
    public int perfectPoints = 100;
    public int greatPoints = 75;
    public int goodPoints = 50;
    public int okPoints = 25;
    public int missPoints = 0;
    public int wrongNotePoints = -10;

    [Header("Combo Multiplier")]
    public float comboMultiplierStep = 0.1f; // +10% per combo
    public float maxComboMultiplier = 3.0f;  // Cap at 3x
    public int comboMilestones = 10;         // Bonus every 10 combo

    [Header("Star Rating Thresholds (percentage of max score)")]
    public float oneStarThreshold = 0.3f;    // 30%
    public float twoStarThreshold = 0.5f;    // 50%
    public float threeStarThreshold = 0.7f;  // 70%
    public float fourStarThreshold = 0.85f;  // 85%
    public float fiveStarThreshold = 0.95f;  // 95%

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<int> OnComboChanged;
    public UnityEvent<TimingRating> OnNoteHit;
    public UnityEvent OnNoteMissed;
    public UnityEvent<int> OnComboMilestone;
    public UnityEvent<int> OnLessonComplete; // stars earned

    [Header("Audio Feedback")]
    public AudioSource feedbackAudio;
    public AudioClip perfectSound;
    public AudioClip greatSound;
    public AudioClip goodSound;
    public AudioClip missSound;
    public AudioClip comboBreakSound;
    public AudioClip milestoneSound;

    // Tracking for lesson
    private int lessonMaxPossibleScore = 0;
    private float lessonStartTime;
    private List<NoteResult> noteResults = new List<NoteResult>();

    public enum TimingRating
    {
        Perfect,
        Great,
        Good,
        OK,
        Miss,
        WrongNote
    }

    [System.Serializable]
    public class NoteResult
    {
        public int expectedNote;
        public int playedNote;
        public TimingRating rating;
        public float timingOffsetMs;
        public int pointsEarned;
        public float timestamp;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call at the start of each lesson/phase
    /// </summary>
    public void StartNewSession(int expectedTotalNotes = 0)
    {
        currentScore = 0;
        combo = 0;
        maxCombo = 0;
        perfectCount = 0;
        greatCount = 0;
        goodCount = 0;
        okCount = 0;
        missCount = 0;
        totalNotes = 0;
        noteResults.Clear();
        lessonStartTime = Time.time;

        // Calculate max possible score
        if (expectedTotalNotes > 0)
        {
            lessonMaxPossibleScore = CalculateMaxPossibleScore(expectedTotalNotes);
        }

        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(combo);

        Debug.Log("[ScoreManager] New session started");
    }

    /// <summary>
    /// Register a correct note hit (for non-rhythm lessons)
    /// </summary>
    public void RegisterCorrectNote(int expectedNote, int playedNote)
    {
        RegisterNoteWithTiming(expectedNote, playedNote, 0f, false);
    }

    /// <summary>
    /// Register a note hit with timing information (for rhythm lessons)
    /// </summary>
    public void RegisterNoteWithTiming(int expectedNote, int playedNote, float timingOffsetMs, bool isRhythmLesson = true)
    {
        totalNotes++;
        TimingRating rating;
        int points;

        // Check if correct note
        if (playedNote != expectedNote)
        {
            rating = TimingRating.WrongNote;
            points = wrongNotePoints;
            BreakCombo();
        }
        else if (!isRhythmLesson)
        {
            // Non-rhythm lesson - correct note always counts as "Great"
            rating = TimingRating.Great;
            points = greatPoints;
            IncrementCombo();
            greatCount++;
        }
        else
        {
            // Rhythm lesson - evaluate timing
            float absOffset = Mathf.Abs(timingOffsetMs);

            if (absOffset <= perfectWindow)
            {
                rating = TimingRating.Perfect;
                points = perfectPoints;
                perfectCount++;
            }
            else if (absOffset <= greatWindow)
            {
                rating = TimingRating.Great;
                points = greatPoints;
                greatCount++;
            }
            else if (absOffset <= goodWindow)
            {
                rating = TimingRating.Good;
                points = goodPoints;
                goodCount++;
            }
            else if (absOffset <= okWindow)
            {
                rating = TimingRating.OK;
                points = okPoints;
                okCount++;
            }
            else
            {
                rating = TimingRating.Miss;
                points = missPoints;
                missCount++;
                BreakCombo();
            }

            if (rating != TimingRating.Miss)
            {
                IncrementCombo();
            }
        }

        // Apply combo multiplier
        float multiplier = GetCurrentMultiplier();
        int finalPoints = Mathf.RoundToInt(points * multiplier);
        currentScore = Mathf.Max(0, currentScore + finalPoints);

        // Record result
        NoteResult result = new NoteResult
        {
            expectedNote = expectedNote,
            playedNote = playedNote,
            rating = rating,
            timingOffsetMs = timingOffsetMs,
            pointsEarned = finalPoints,
            timestamp = Time.time - lessonStartTime
        };
        noteResults.Add(result);

        // Play audio feedback
        PlayRatingSound(rating);

        // Fire events
        OnScoreChanged?.Invoke(currentScore);
        OnNoteHit?.Invoke(rating);

        Debug.Log($"[ScoreManager] Note: {rating}, Offset: {timingOffsetMs:F1}ms, Points: {finalPoints} (x{multiplier:F1}), Combo: {combo}");
    }

    /// <summary>
    /// Register a missed note (didn't play in time)
    /// </summary>
    public void RegisterMiss(int expectedNote)
    {
        totalNotes++;
        missCount++;
        BreakCombo();

        NoteResult result = new NoteResult
        {
            expectedNote = expectedNote,
            playedNote = -1,
            rating = TimingRating.Miss,
            timingOffsetMs = float.MaxValue,
            pointsEarned = 0,
            timestamp = Time.time - lessonStartTime
        };
        noteResults.Add(result);

        PlayRatingSound(TimingRating.Miss);
        OnNoteMissed?.Invoke();

        Debug.Log($"[ScoreManager] MISS! Combo broken.");
    }

    /// <summary>
    /// Register a wrong note (played incorrect key)
    /// </summary>
    public void RegisterWrongNote(int expectedNote, int playedNote)
    {
        totalNotes++;

        int points = wrongNotePoints;
        currentScore = Mathf.Max(0, currentScore + points);
        BreakCombo();

        NoteResult result = new NoteResult
        {
            expectedNote = expectedNote,
            playedNote = playedNote,
            rating = TimingRating.WrongNote,
            timingOffsetMs = 0,
            pointsEarned = points,
            timestamp = Time.time - lessonStartTime
        };
        noteResults.Add(result);

        PlayRatingSound(TimingRating.WrongNote);
        OnScoreChanged?.Invoke(currentScore);

        Debug.Log($"[ScoreManager] WRONG NOTE! Expected {expectedNote}, got {playedNote}. Combo broken.");
    }

    private void IncrementCombo()
    {
        combo++;
        if (combo > maxCombo)
        {
            maxCombo = combo;
        }

        OnComboChanged?.Invoke(combo);

        // Check for combo milestone
        if (combo > 0 && combo % comboMilestones == 0)
        {
            // Bonus points for milestone
            int bonus = combo * 10;
            currentScore += bonus;
            OnScoreChanged?.Invoke(currentScore);
            OnComboMilestone?.Invoke(combo);

            if (feedbackAudio && milestoneSound)
                feedbackAudio.PlayOneShot(milestoneSound);

            Debug.Log($"[ScoreManager] COMBO MILESTONE: {combo}! Bonus: {bonus} points!");
        }
    }

    private void BreakCombo()
    {
        if (combo > 5 && feedbackAudio && comboBreakSound)
        {
            feedbackAudio.PlayOneShot(comboBreakSound);
        }
        combo = 0;
        OnComboChanged?.Invoke(combo);
    }

    private void PlayRatingSound(TimingRating rating)
    {
        if (feedbackAudio == null) return;

        AudioClip clip = null;
        switch (rating)
        {
            case TimingRating.Perfect:
                clip = perfectSound;
                break;
            case TimingRating.Great:
                clip = greatSound;
                break;
            case TimingRating.Good:
                clip = goodSound;
                break;
            case TimingRating.Miss:
            case TimingRating.WrongNote:
                clip = missSound;
                break;
        }

        if (clip != null)
            feedbackAudio.PlayOneShot(clip);
    }

    private int CalculateMaxPossibleScore(int noteCount)
    {
        // Perfect score with max combo
        int score = 0;
        for (int i = 0; i < noteCount; i++)
        {
            float multiplier = Mathf.Min(1f + (i * comboMultiplierStep), maxComboMultiplier);
            score += Mathf.RoundToInt(perfectPoints * multiplier);

            // Add milestone bonuses
            if ((i + 1) % comboMilestones == 0)
            {
                score += (i + 1) * 10;
            }
        }
        return score;
    }

    /// <summary>
    /// Calculate star rating for current session
    /// </summary>
    public int CalculateStarRating()
    {
        if (lessonMaxPossibleScore <= 0 || totalNotes == 0)
        {
            // Fallback to accuracy-based rating
            float accuracy = GetAccuracy();
            if (accuracy >= 0.95f) return 5;
            if (accuracy >= 0.85f) return 4;
            if (accuracy >= 0.70f) return 3;
            if (accuracy >= 0.50f) return 2;
            if (accuracy >= 0.30f) return 1;
            return 0;
        }

        float scorePercentage = (float)currentScore / lessonMaxPossibleScore;

        if (scorePercentage >= fiveStarThreshold) return 5;
        if (scorePercentage >= fourStarThreshold) return 4;
        if (scorePercentage >= threeStarThreshold) return 3;
        if (scorePercentage >= twoStarThreshold) return 2;
        if (scorePercentage >= oneStarThreshold) return 1;
        return 0;
    }

    /// <summary>
    /// End session and get final stats
    /// </summary>
    public SessionStats EndSession()
    {
        int stars = CalculateStarRating();
        OnLessonComplete?.Invoke(stars);

        SessionStats stats = new SessionStats
        {
            finalScore = currentScore,
            maxPossibleScore = lessonMaxPossibleScore,
            stars = stars,
            totalNotes = totalNotes,
            perfectCount = perfectCount,
            greatCount = greatCount,
            goodCount = goodCount,
            okCount = okCount,
            missCount = missCount,
            maxCombo = maxCombo,
            accuracy = GetAccuracy(),
            averageTimingOffset = GetAverageTimingOffset()
        };

        Debug.Log($"[ScoreManager] Session ended: {stats.finalScore} points, {stats.stars} stars, {stats.accuracy:P1} accuracy");

        return stats;
    }

    public float GetAccuracy()
    {
        if (totalNotes == 0) return 0f;
        int correctNotes = perfectCount + greatCount + goodCount + okCount;
        return (float)correctNotes / totalNotes;
    }

    public float GetAverageTimingOffset()
    {
        if (noteResults.Count == 0) return 0f;

        float total = 0f;
        int count = 0;
        foreach (var result in noteResults)
        {
            if (result.rating != TimingRating.Miss && result.rating != TimingRating.WrongNote)
            {
                total += Mathf.Abs(result.timingOffsetMs);
                count++;
            }
        }
        return count > 0 ? total / count : 0f;
    }

    // Public getters
    public int GetCurrentScore() => currentScore;
    public int GetCombo() => combo;
    public int GetMaxCombo() => maxCombo;
    public float GetCurrentMultiplier() => Mathf.Min(1f + (combo * comboMultiplierStep), maxComboMultiplier);
    public int GetPerfectCount() => perfectCount;
    public int GetGreatCount() => greatCount;
    public int GetGoodCount() => goodCount;
    public int GetOKCount() => okCount;
    public int GetMissCount() => missCount;

    /// <summary>
    /// Get timing zone description for UI
    /// </summary>
    public string GetTimingZoneText(float timingOffsetMs)
    {
        float absOffset = Mathf.Abs(timingOffsetMs);
        if (absOffset <= perfectWindow) return "PERFECT!";
        if (absOffset <= greatWindow) return "GREAT!";
        if (absOffset <= goodWindow) return "GOOD";
        if (absOffset <= okWindow) return "OK";
        return "MISS";
    }

    /// <summary>
    /// Get color for timing rating
    /// </summary>
    public Color GetRatingColor(TimingRating rating)
    {
        switch (rating)
        {
            case TimingRating.Perfect: return new Color(1f, 0.84f, 0f); // Gold
            case TimingRating.Great: return Color.green;
            case TimingRating.Good: return Color.cyan;
            case TimingRating.OK: return Color.yellow;
            case TimingRating.Miss: return Color.red;
            case TimingRating.WrongNote: return Color.red;
            default: return Color.white;
        }
    }
}

[System.Serializable]
public class SessionStats
{
    public int finalScore;
    public int maxPossibleScore;
    public int stars;
    public int totalNotes;
    public int perfectCount;
    public int greatCount;
    public int goodCount;
    public int okCount;
    public int missCount;
    public int maxCombo;
    public float accuracy;
    public float averageTimingOffset;

    public override string ToString()
    {
        return $"Score: {finalScore} | Stars: {stars}/5 | Accuracy: {accuracy:P1} | Max Combo: {maxCombo}";
    }
}