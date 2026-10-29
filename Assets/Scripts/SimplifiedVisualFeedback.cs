using UnityEngine;
using System.Collections;

/// <summary>
/// SIMPLIFIED Visual Feedback - Clear color system
/// Only 3 states: Normal, Hint, Result
/// </summary>
public class SimplifiedVisualFeedback : MonoBehaviour
{
    [Header("Color System")]
    public Color normalColor = new Color(0.95f, 0.95f, 0.9f);
    public Color hintColor = new Color(1f, 0.85f, 0.3f);
    public Color correctColor = new Color(0.3f, 1f, 0.3f);
    public Color wrongColor = new Color(1f, 0.3f, 0.3f);

    [Header("Timing")]
    public float feedbackDuration = 0.5f;
    public float hintPulseDuration = 3f;

    void Start()
    {
        ResetAllKeys();
    }

    public void ResetAllKeys()
    {
        PianoKeyEnhanced[] allKeys = FindObjectsByType<PianoKeyEnhanced>(FindObjectsSortMode.None);

        foreach (var key in allKeys)
        {
            if (key.keyRenderer && key.defaultMaterial)
            {
                key.keyRenderer.material.color = normalColor;
            }
        }
    }

    public void ShowKeyFeedback(PianoKeyEnhanced key, bool isCorrect)
    {
        if (key == null) return;
        StartCoroutine(FlashKey(key, isCorrect ? correctColor : wrongColor));
    }

    IEnumerator FlashKey(PianoKeyEnhanced key, Color flashColor)
    {
        if (key == null || key.keyRenderer == null) yield break;

        key.keyRenderer.material.color = flashColor;
        yield return new WaitForSeconds(feedbackDuration);
        key.keyRenderer.material.color = normalColor;
    }

    public void PulseHintKey(PianoKeyEnhanced key)
    {
        ResetAllKeys();
        if (key != null)
        {
            StartCoroutine(PulseKey(key, hintColor, hintPulseDuration));
        }
    }

    IEnumerator PulseKey(PianoKeyEnhanced key, Color pulseColor, float duration)
    {
        if (key == null || key.keyRenderer == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float pulse = Mathf.Sin(elapsed * 2f) * 0.25f + 0.75f;
            key.keyRenderer.material.color = Color.Lerp(normalColor, pulseColor, pulse);
            yield return null;
        }

        key.keyRenderer.material.color = normalColor;
    }
}