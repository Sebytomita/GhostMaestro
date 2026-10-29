using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Rhythm Judge - evaluates timing accuracy for rhythm-based lessons
/// Handles metronome, beat detection, and timing tolerance (±60/±30/±15ms)
/// </summary>
public class RhythmJudge : MonoBehaviour
{
    [Header("Metronome Settings")]
    [Range(40, 200)]
    public int bpm = 60;
    public bool autoStart = true;
    public AudioClip metronomeTick;
    public AudioSource metronomeSource;

    [Header("Timing Tolerance")]
    [Tooltip("Perfect: ±15ms")]
    public float perfectTolerance = 0.015f;

    [Tooltip("Good: ±30ms")]
    public float goodTolerance = 0.030f;

    [Tooltip("OK: ±60ms")]
    public float okTolerance = 0.060f;

    [Header("Visual Feedback")]
    public GameObject beatIndicator; // Pulsing visual
    public Color perfectColor = Color.green;
    public Color goodColor = Color.yellow;
    public Color okColor = Color.white;
    public Color missColor = Color.red;

    // Internal state
    private float beatInterval; // seconds per beat
    private float nextBeatTime;
    private int currentBeat = 0;
    private bool isRunning = false;

    // Timing history
    private List<float> timingErrors = new List<float>();
    private int perfectHits = 0;
    private int goodHits = 0;
    private int okHits = 0;
    private int misses = 0;

    // Events
    public System.Action<int> OnBeat; // Beat number
    public System.Action<TimingResult> OnTimingEvaluated;

    void Start()
    {
        CalculateBeatInterval();

        if (!metronomeSource)
        {
            metronomeSource = gameObject.AddComponent<AudioSource>();
            metronomeSource.playOnAwake = false;
        }

        if (autoStart)
        {
            StartMetronome();
        }
    }

    void Update()
    {
        if (!isRunning) return;

        // Check if it's time for next beat
        if (Time.time >= nextBeatTime)
        {
            PlayBeat();
            nextBeatTime += beatInterval;
            currentBeat++;
        }

        // Visual pulse (optional)
        UpdateBeatIndicator();
    }

    /// <summary>
    /// Start the metronome
    /// </summary>
    public void StartMetronome()
    {
        isRunning = true;
        nextBeatTime = Time.time + beatInterval;
        currentBeat = 0;
        ResetStats();
        Debug.Log($"[RhythmJudge] Metronome started at {bpm} BPM");
    }

    /// <summary>
    /// Stop the metronome
    /// </summary>
    public void StopMetronome()
    {
        isRunning = false;
        Debug.Log("[RhythmJudge] Metronome stopped");
    }

    /// <summary>
    /// Set new BPM and recalculate
    /// </summary>
    public void SetBPM(int newBpm)
    {
        bpm = Mathf.Clamp(newBpm, 40, 200);
        CalculateBeatInterval();
        Debug.Log($"[RhythmJudge] BPM changed to {bpm}");
    }

    /// <summary>
    /// Calculate beat interval from BPM
    /// </summary>
    void CalculateBeatInterval()
    {
        beatInterval = 60f / bpm;
    }

    /// <summary>
    /// Play metronome tick sound
    /// </summary>
    void PlayBeat()
    {
        if (metronomeSource && metronomeTick)
        {
            // Adjust pitch based on BPM - slower BPM = lower pitch = longer sound
            // At 60 BPM pitch is 1.0 (normal), at 30 BPM pitch is 0.5 (slower/longer)
            float baseBPM = 60f;
            metronomeSource.pitch = bpm / baseBPM;

            metronomeSource.PlayOneShot(metronomeTick);
        }

        OnBeat?.Invoke(currentBeat);

        // Visual pulse
        if (beatIndicator)
        {
            // Simple scale pulse - you can expand this
            beatIndicator.transform.localScale = Vector3.one * 1.2f;
        }
    }

    /// <summary>
    /// Evaluate player's timing when they play a note
    /// </summary>
    /// <param name="playTime">When the player pressed the key</param>
    /// <returns>Timing evaluation result</returns>
    public TimingResult EvaluateTiming(float playTime)
    {
        if (!isRunning)
        {
            return new TimingResult { judgment = TimingJudgment.Miss, error = 0f };
        }

        // Calculate time difference from nearest beat
        float timeSinceLastBeat = (playTime - (nextBeatTime - beatInterval));
        float timeToNextBeat = nextBeatTime - playTime;

        // Find closest beat
        float error = Mathf.Min(Mathf.Abs(timeSinceLastBeat), Mathf.Abs(timeToNextBeat));

        // Determine judgment
        TimingJudgment judgment;
        Color feedbackColor;

        if (error <= perfectTolerance)
        {
            judgment = TimingJudgment.Perfect;
            feedbackColor = perfectColor;
            perfectHits++;
        }
        else if (error <= goodTolerance)
        {
            judgment = TimingJudgment.Good;
            feedbackColor = goodColor;
            goodHits++;
        }
        else if (error <= okTolerance)
        {
            judgment = TimingJudgment.OK;
            feedbackColor = okColor;
            okHits++;
        }
        else
        {
            judgment = TimingJudgment.Miss;
            feedbackColor = missColor;
            misses++;
        }

        // Store timing error
        timingErrors.Add(error);

        var result = new TimingResult
        {
            judgment = judgment,
            error = error,
            errorMs = error * 1000f,
            color = feedbackColor,
            beatNumber = currentBeat
        };

        OnTimingEvaluated?.Invoke(result);

        Debug.Log($"[RhythmJudge] {judgment} | Error: {result.errorMs:F1}ms");

        return result;
    }

    /// <summary>
    /// Get current statistics
    /// </summary>
    public RhythmStats GetStats()
    {
        int total = perfectHits + goodHits + okHits + misses;
        float avgError = 0f;

        if (timingErrors.Count > 0)
        {
            foreach (float e in timingErrors)
                avgError += e;
            avgError /= timingErrors.Count;
        }

        return new RhythmStats
        {
            perfectHits = perfectHits,
            goodHits = goodHits,
            okHits = okHits,
            misses = misses,
            totalHits = total,
            accuracy = total > 0 ? (float)(perfectHits + goodHits) / total : 0f,
            averageErrorMs = avgError * 1000f
        };
    }

    /// <summary>
    /// Reset statistics
    /// </summary>
    public void ResetStats()
    {
        perfectHits = 0;
        goodHits = 0;
        okHits = 0;
        misses = 0;
        timingErrors.Clear();
    }

    /// <summary>
    /// Visual beat indicator update
    /// </summary>
    void UpdateBeatIndicator()
    {
        if (!beatIndicator) return;

        // Pulse animation
        float t = (Time.time - (nextBeatTime - beatInterval)) / beatInterval;
        float scale = Mathf.Lerp(1.2f, 1.0f, t);
        beatIndicator.transform.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// Get time until next beat (for predictive visuals)
    /// </summary>
    public float GetTimeToNextBeat()
    {
        return nextBeatTime - Time.time;
    }

    /// <summary>
    /// Get time since last beat
    /// </summary>
    public float GetTimeSinceLastBeat()
    {
        return Time.time - (nextBeatTime - beatInterval);
    }

    void OnDrawGizmos()
    {
        if (!isRunning) return;

        // Draw timing windows
        Gizmos.color = perfectColor;
        Gizmos.DrawWireSphere(transform.position, perfectTolerance * 10f);

        Gizmos.color = goodColor;
        Gizmos.DrawWireSphere(transform.position, goodTolerance * 10f);

        Gizmos.color = okColor;
        Gizmos.DrawWireSphere(transform.position, okTolerance * 10f);
    }
}

/// <summary>
/// Timing evaluation result
/// </summary>
[System.Serializable]
public struct TimingResult
{
    public TimingJudgment judgment;
    public float error; // in seconds
    public float errorMs; // in milliseconds
    public Color color;
    public int beatNumber;
}

/// <summary>
/// Timing judgment levels
/// </summary>
public enum TimingJudgment
{
    Perfect,  // ±15ms
    Good,     // ±30ms
    OK,       // ±60ms
    Miss      // >60ms
}

/// <summary>
/// Rhythm statistics
/// </summary>
[System.Serializable]
public struct RhythmStats
{
    public int perfectHits;
    public int goodHits;
    public int okHits;
    public int misses;
    public int totalHits;
    public float accuracy; // 0-1
    public float averageErrorMs;

    public override string ToString()
    {
        return $"Perfect: {perfectHits} | Good: {goodHits} | OK: {okHits} | Miss: {misses} | Accuracy: {accuracy:P0}";
    }
}