using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// UI Display for Score System
/// Shows score, combo, timing feedback, and star rating
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [Header("References")]
    public ScoreManager scoreManager;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI multiplierText;

    [Header("Timing Feedback")]
    public TextMeshProUGUI timingFeedbackText;
    public float feedbackDisplayDuration = 0.8f;
    public bool animateFeedback = true;

    [Header("Combo Effects")]
    public GameObject comboEffectPrefab;
    public Transform comboEffectParent;
    public Color comboColor = Color.yellow;
    public Color maxComboColor = new Color(1f, 0.5f, 0f); // Orange for high combo

    [Header("Star Display")]
    public GameObject[] starObjects; // 5 star GameObjects
    public Color starActiveColor = new Color(1f, 0.84f, 0f); // Gold
    public Color starInactiveColor = new Color(0.3f, 0.3f, 0.3f); // Gray

    [Header("End Screen")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI statsBreakdownText;
    public GameObject[] endScreenStars;

    [Header("Timing Bar (for Rhythm Lessons)")]
    public GameObject timingBarContainer;
    public RectTransform timingBarFill;
    public Image timingBarImage;
    public Color perfectZoneColor = new Color(1f, 0.84f, 0f, 0.8f);
    public Color greatZoneColor = new Color(0f, 1f, 0f, 0.6f);
    public Color goodZoneColor = new Color(0f, 1f, 1f, 0.4f);
    public Color okZoneColor = new Color(1f, 1f, 0f, 0.3f);

    [Header("Animation Settings")]
    public float scoreAnimDuration = 0.3f;
    public float comboPopScale = 1.3f;
    public float feedbackPopScale = 1.5f;

    private Coroutine feedbackCoroutine;
    private Coroutine scoreAnimCoroutine;
    private int displayedScore = 0;

    void Start()
    {
        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        if (scoreManager != null)
        {
            // Subscribe to events
            scoreManager.OnScoreChanged.AddListener(OnScoreChanged);
            scoreManager.OnComboChanged.AddListener(OnComboChanged);
            scoreManager.OnNoteHit.AddListener(OnNoteHit);
            scoreManager.OnNoteMissed.AddListener(OnNoteMissed);
            scoreManager.OnComboMilestone.AddListener(OnComboMilestone);
            scoreManager.OnLessonComplete.AddListener(OnLessonComplete);
        }

        // Initialize UI
        UpdateScoreDisplay(0);
        UpdateComboDisplay(0);
        HideTimingFeedback();

        if (endScreenPanel)
            endScreenPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged.RemoveListener(OnScoreChanged);
            scoreManager.OnComboChanged.RemoveListener(OnComboChanged);
            scoreManager.OnNoteHit.RemoveListener(OnNoteHit);
            scoreManager.OnNoteMissed.RemoveListener(OnNoteMissed);
            scoreManager.OnComboMilestone.RemoveListener(OnComboMilestone);
            scoreManager.OnLessonComplete.RemoveListener(OnLessonComplete);
        }
    }

    #region Event Handlers

    private void OnScoreChanged(int newScore)
    {
        if (scoreAnimCoroutine != null)
            StopCoroutine(scoreAnimCoroutine);
        scoreAnimCoroutine = StartCoroutine(AnimateScoreChange(newScore));
    }

    private void OnComboChanged(int newCombo)
    {
        UpdateComboDisplay(newCombo);
    }

    private void OnNoteHit(ScoreManager.TimingRating rating)
    {
        ShowTimingFeedback(rating);
    }

    private void OnNoteMissed()
    {
        ShowTimingFeedback(ScoreManager.TimingRating.Miss);
    }

    private void OnComboMilestone(int milestone)
    {
        ShowComboMilestone(milestone);
    }

    private void OnLessonComplete(int stars)
    {
        ShowEndScreen(stars);
    }

    #endregion

    #region Score Display

    private void UpdateScoreDisplay(int score)
    {
        if (scoreText)
        {
            scoreText.text = score.ToString("N0");
        }
    }

    private IEnumerator AnimateScoreChange(int targetScore)
    {
        int startScore = displayedScore;
        float elapsed = 0f;

        while (elapsed < scoreAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scoreAnimDuration;
            t = 1f - Mathf.Pow(1f - t, 3f); // Ease out cubic

            displayedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, t));
            UpdateScoreDisplay(displayedScore);
            yield return null;
        }

        displayedScore = targetScore;
        UpdateScoreDisplay(displayedScore);
    }

    #endregion

    #region Combo Display

    private void UpdateComboDisplay(int combo)
    {
        if (comboText)
        {
            if (combo > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = $"{combo}x COMBO";

                // Color based on combo level
                if (combo >= 50)
                    comboText.color = new Color(1f, 0f, 1f); // Purple for 50+
                else if (combo >= 30)
                    comboText.color = maxComboColor;
                else if (combo >= 10)
                    comboText.color = comboColor;
                else
                    comboText.color = Color.white;

                if (animateFeedback)
                    StartCoroutine(PopAnimation(comboText.transform, comboPopScale));
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }

        // Update multiplier display
        if (multiplierText && scoreManager != null)
        {
            float mult = scoreManager.GetCurrentMultiplier();
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

    private void ShowComboMilestone(int milestone)
    {
        // Big combo celebration
        if (comboText)
        {
            StartCoroutine(ComboMilestoneAnimation(milestone));
        }
    }

    private IEnumerator ComboMilestoneAnimation(int milestone)
    {
        if (comboText == null) yield break;

        // Flash effect
        Color originalColor = comboText.color;
        string originalText = comboText.text;

        comboText.text = $"🔥 {milestone}x COMBO! 🔥";
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

    #endregion

    #region Timing Feedback

    private void ShowTimingFeedback(ScoreManager.TimingRating rating)
    {
        if (timingFeedbackText == null) return;

        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackCoroutine = StartCoroutine(ShowFeedbackCoroutine(rating));
    }

    private IEnumerator ShowFeedbackCoroutine(ScoreManager.TimingRating rating)
    {
        timingFeedbackText.gameObject.SetActive(true);

        // Set text and color
        string text = "";
        Color color = Color.white;

        switch (rating)
        {
            case ScoreManager.TimingRating.Perfect:
                text = "PERFECT!";
                color = new Color(1f, 0.84f, 0f); // Gold
                break;
            case ScoreManager.TimingRating.Great:
                text = "GREAT!";
                color = Color.green;
                break;
            case ScoreManager.TimingRating.Good:
                text = "GOOD";
                color = Color.cyan;
                break;
            case ScoreManager.TimingRating.OK:
                text = "OK";
                color = Color.yellow;
                break;
            case ScoreManager.TimingRating.Miss:
                text = "MISS";
                color = Color.red;
                break;
            case ScoreManager.TimingRating.WrongNote:
                text = "WRONG!";
                color = Color.red;
                break;
        }

        timingFeedbackText.text = text;
        timingFeedbackText.color = color;

        // Pop animation
        if (animateFeedback)
        {
            timingFeedbackText.transform.localScale = Vector3.one * feedbackPopScale;
            float elapsed = 0f;
            while (elapsed < feedbackDisplayDuration * 0.3f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (feedbackDisplayDuration * 0.3f);
                timingFeedbackText.transform.localScale = Vector3.Lerp(
                    Vector3.one * feedbackPopScale,
                    Vector3.one,
                    t
                );
                yield return null;
            }
        }

        // Wait
        yield return new WaitForSeconds(feedbackDisplayDuration * 0.7f);

        // Fade out
        float fadeTime = 0.2f;
        Color startColor = timingFeedbackText.color;
        float fadeElapsed = 0f;
        while (fadeElapsed < fadeTime)
        {
            fadeElapsed += Time.deltaTime;
            float alpha = 1f - (fadeElapsed / fadeTime);
            timingFeedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        HideTimingFeedback();
    }

    private void HideTimingFeedback()
    {
        if (timingFeedbackText)
            timingFeedbackText.gameObject.SetActive(false);
    }

    #endregion

    #region Star Display

    public void UpdateStarDisplay(int stars)
    {
        if (starObjects == null) return;

        for (int i = 0; i < starObjects.Length; i++)
        {
            if (starObjects[i] == null) continue;

            Image starImage = starObjects[i].GetComponent<Image>();
            if (starImage)
            {
                starImage.color = i < stars ? starActiveColor : starInactiveColor;
            }
        }
    }

    #endregion

    #region End Screen

    public void ShowEndScreen(int stars)
    {
        if (endScreenPanel == null) return;

        endScreenPanel.SetActive(true);

        SessionStats stats = scoreManager.EndSession();

        // Final score
        if (finalScoreText)
            finalScoreText.text = $"Scor: {stats.finalScore:N0}";

        // Accuracy
        if (accuracyText)
            accuracyText.text = $"Acuratețe: {stats.accuracy:P1}";

        // Max combo
        if (maxComboText)
            maxComboText.text = $"Max Combo: {stats.maxCombo}x";

        // Stats breakdown
        if (statsBreakdownText)
        {
            statsBreakdownText.text = $"Perfect: {stats.perfectCount}\n" +
                                      $"Great: {stats.greatCount}\n" +
                                      $"Good: {stats.goodCount}\n" +
                                      $"OK: {stats.okCount}\n" +
                                      $"Miss: {stats.missCount}";
        }

        // Stars
        if (endScreenStars != null)
        {
            StartCoroutine(AnimateStars(stars));
        }
    }

    private IEnumerator AnimateStars(int starCount)
    {
        // First, hide all stars
        foreach (var star in endScreenStars)
        {
            if (star != null)
            {
                star.transform.localScale = Vector3.zero;
                Image img = star.GetComponent<Image>();
                if (img) img.color = starInactiveColor;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Animate each earned star
        for (int i = 0; i < starCount && i < endScreenStars.Length; i++)
        {
            if (endScreenStars[i] == null) continue;

            Image img = endScreenStars[i].GetComponent<Image>();
            if (img) img.color = starActiveColor;

            // Pop in animation
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Overshoot ease
                float scale = 1f + 0.3f * Mathf.Sin(t * Mathf.PI);
                endScreenStars[i].transform.localScale = Vector3.one * scale * t;
                yield return null;
            }

            endScreenStars[i].transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void HideEndScreen()
    {
        if (endScreenPanel)
            endScreenPanel.SetActive(false);
    }

    #endregion

    #region Utility

    private IEnumerator PopAnimation(Transform target, float scale)
    {
        if (target == null) yield break;

        target.localScale = Vector3.one * scale;
        float elapsed = 0f;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(Vector3.one * scale, Vector3.one, t);
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    /// <summary>
    /// Show/hide score UI (call when lesson starts/ends)
    /// </summary>
    public void SetScoreUIVisible(bool visible)
    {
        if (scoreText) scoreText.gameObject.SetActive(visible);
        if (comboText) comboText.gameObject.SetActive(visible && scoreManager.GetCombo() > 1);
        if (multiplierText) multiplierText.gameObject.SetActive(visible && scoreManager.GetCurrentMultiplier() > 1f);
    }

    /// <summary>
    /// Reset display for new lesson
    /// </summary>
    public void ResetDisplay()
    {
        displayedScore = 0;
        UpdateScoreDisplay(0);
        UpdateComboDisplay(0);
        HideTimingFeedback();
        HideEndScreen();
        UpdateStarDisplay(0);
    }

    #endregion
}