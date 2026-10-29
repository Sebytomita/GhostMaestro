using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Visual indicator showing timing zones for rhythm lessons
/// Displays colored zones: Perfect (center), Great, Good, OK
/// Arrow shows current position relative to beat
/// </summary>
public class TimingZoneIndicator : MonoBehaviour
{
    [Header("Zone Visuals")]
    public RectTransform zoneContainer;
    public Image perfectZone;
    public Image greatZone;
    public Image goodZone;
    public Image okZone;
    public Image missZone; // Background/outer area

    [Header("Timing Arrow")]
    public RectTransform timingArrow;
    public Image arrowImage;

    [Header("Hit Marker")]
    public GameObject hitMarkerPrefab;
    public Transform hitMarkerParent;
    public float hitMarkerLifetime = 0.5f;

    [Header("Zone Colors")]
    public Color perfectColor = new Color(1f, 0.84f, 0f, 0.9f);      // Gold
    public Color greatColor = new Color(0f, 1f, 0f, 0.7f);           // Green
    public Color goodColor = new Color(0f, 1f, 1f, 0.5f);            // Cyan
    public Color okColor = new Color(1f, 1f, 0f, 0.4f);              // Yellow
    public Color missColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);      // Gray

    [Header("Arrow Colors (changes based on position)")]
    public Color arrowPerfectColor = new Color(1f, 0.84f, 0f);
    public Color arrowGreatColor = Color.green;
    public Color arrowGoodColor = Color.cyan;
    public Color arrowOKColor = Color.yellow;
    public Color arrowMissColor = Color.red;

    [Header("Settings")]
    public float totalWidth = 400f;  // Total width of the timing bar
    public float arrowOscillationSpeed = 1f; // How fast arrow moves (synced with BPM)

    [Header("References")]
    public ScoreManager scoreManager;
    public RhythmJudge rhythmJudge;

    // Zone widths as percentage of total (must sum to 1.0)
    private float perfectWidthPercent;
    private float greatWidthPercent;
    private float goodWidthPercent;
    private float okWidthPercent;

    void Start()
    {
        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        CalculateZoneWidths();
        SetupZoneVisuals();
        SetupColors();
    }

    void Update()
    {
        UpdateArrowPosition();
        UpdateArrowColor();
    }

    /// <summary>
    /// Calculate zone widths based on ScoreManager timing windows
    /// </summary>
    private void CalculateZoneWidths()
    {
        if (scoreManager == null) return;

        // Get timing windows from ScoreManager
        float perfectMs = scoreManager.perfectWindow;
        float greatMs = scoreManager.greatWindow;
        float goodMs = scoreManager.goodWindow;
        float okMs = scoreManager.okWindow;

        // Total range is 2x OK window (both sides of center)
        float totalRange = okMs * 2f;

        // Calculate each zone's width as percentage
        // Perfect is in the center, others expand outward
        perfectWidthPercent = (perfectMs * 2f) / totalRange;
        greatWidthPercent = ((greatMs - perfectMs) * 2f) / totalRange;
        goodWidthPercent = ((goodMs - greatMs) * 2f) / totalRange;
        okWidthPercent = ((okMs - goodMs) * 2f) / totalRange;

        Debug.Log($"[TimingZone] Widths - Perfect: {perfectWidthPercent:P0}, Great: {greatWidthPercent:P0}, Good: {goodWidthPercent:P0}, OK: {okWidthPercent:P0}");
    }

    /// <summary>
    /// Setup the visual representation of zones
    /// </summary>
    private void SetupZoneVisuals()
    {
        if (zoneContainer == null) return;

        float containerWidth = totalWidth;
        float centerX = 0f; // Center of container

        // Perfect zone (center)
        if (perfectZone)
        {
            float width = containerWidth * perfectWidthPercent;
            perfectZone.rectTransform.sizeDelta = new Vector2(width, perfectZone.rectTransform.sizeDelta.y);
            perfectZone.rectTransform.anchoredPosition = new Vector2(centerX, 0);
        }

        // Great zone (around perfect)
        if (greatZone)
        {
            float perfectWidth = containerWidth * perfectWidthPercent;
            float greatTotalWidth = perfectWidth + (containerWidth * greatWidthPercent);
            greatZone.rectTransform.sizeDelta = new Vector2(greatTotalWidth, greatZone.rectTransform.sizeDelta.y);
            greatZone.rectTransform.anchoredPosition = new Vector2(centerX, 0);
        }

        // Good zone (around great)
        if (goodZone)
        {
            float upToGoodWidth = containerWidth * (perfectWidthPercent + greatWidthPercent + goodWidthPercent);
            goodZone.rectTransform.sizeDelta = new Vector2(upToGoodWidth, goodZone.rectTransform.sizeDelta.y);
            goodZone.rectTransform.anchoredPosition = new Vector2(centerX, 0);
        }

        // OK zone (around good)
        if (okZone)
        {
            float upToOKWidth = containerWidth * (perfectWidthPercent + greatWidthPercent + goodWidthPercent + okWidthPercent);
            okZone.rectTransform.sizeDelta = new Vector2(upToOKWidth, okZone.rectTransform.sizeDelta.y);
            okZone.rectTransform.anchoredPosition = new Vector2(centerX, 0);
        }

        // Miss zone (full width background)
        if (missZone)
        {
            missZone.rectTransform.sizeDelta = new Vector2(containerWidth, missZone.rectTransform.sizeDelta.y);
            missZone.rectTransform.anchoredPosition = new Vector2(centerX, 0);
        }
    }

    private void SetupColors()
    {
        if (perfectZone) perfectZone.color = perfectColor;
        if (greatZone) greatZone.color = greatColor;
        if (goodZone) goodZone.color = goodColor;
        if (okZone) okZone.color = okColor;
        if (missZone) missZone.color = missColor;
    }

    /// <summary>
    /// Update arrow position based on time since last beat
    /// </summary>
    private void UpdateArrowPosition()
    {
        if (timingArrow == null || rhythmJudge == null) return;

        float beatInterval = 60f / rhythmJudge.bpm;
        float timeSinceLastBeat = rhythmJudge.GetTimeSinceLastBeat();

        // Normalize to -0.5 to 0.5 (center at beat)
        float normalizedTime = (timeSinceLastBeat / beatInterval) - 0.5f;

        // Convert to position
        float xPos = normalizedTime * totalWidth;

        // Clamp to visible area
        xPos = Mathf.Clamp(xPos, -totalWidth / 2f, totalWidth / 2f);

        timingArrow.anchoredPosition = new Vector2(xPos, timingArrow.anchoredPosition.y);
    }

    /// <summary>
    /// Update arrow color based on current position (timing zone)
    /// </summary>
    private void UpdateArrowColor()
    {
        if (arrowImage == null || timingArrow == null) return;

        float xPos = timingArrow.anchoredPosition.x;
        float absPos = Mathf.Abs(xPos);
        float halfWidth = totalWidth / 2f;

        // Calculate zone boundaries
        float perfectBoundary = halfWidth * perfectWidthPercent;
        float greatBoundary = halfWidth * (perfectWidthPercent + greatWidthPercent);
        float goodBoundary = halfWidth * (perfectWidthPercent + greatWidthPercent + goodWidthPercent);
        float okBoundary = halfWidth * (perfectWidthPercent + greatWidthPercent + goodWidthPercent + okWidthPercent);

        // Set color based on position
        if (absPos <= perfectBoundary)
            arrowImage.color = arrowPerfectColor;
        else if (absPos <= greatBoundary)
            arrowImage.color = arrowGreatColor;
        else if (absPos <= goodBoundary)
            arrowImage.color = arrowGoodColor;
        else if (absPos <= okBoundary)
            arrowImage.color = arrowOKColor;
        else
            arrowImage.color = arrowMissColor;
    }

    /// <summary>
    /// Show a hit marker at current arrow position
    /// </summary>
    public void ShowHitMarker(ScoreManager.TimingRating rating)
    {
        if (hitMarkerPrefab == null || hitMarkerParent == null || timingArrow == null) return;

        GameObject marker = Instantiate(hitMarkerPrefab, hitMarkerParent);
        RectTransform markerRect = marker.GetComponent<RectTransform>();

        if (markerRect)
        {
            markerRect.anchoredPosition = timingArrow.anchoredPosition;
        }

        // Set marker color based on rating
        Image markerImage = marker.GetComponent<Image>();
        if (markerImage && scoreManager != null)
        {
            markerImage.color = scoreManager.GetRatingColor(rating);
        }

        // Destroy after lifetime
        Destroy(marker, hitMarkerLifetime);
    }

    /// <summary>
    /// Get timing offset in milliseconds based on arrow position
    /// </summary>
    public float GetCurrentTimingOffsetMs()
    {
        if (timingArrow == null || scoreManager == null) return float.MaxValue;

        float xPos = timingArrow.anchoredPosition.x;
        float halfWidth = totalWidth / 2f;

        // Convert position to timing offset
        // At center (0) = 0ms offset
        // At edges = max offset (okWindow)
        float normalizedPos = xPos / halfWidth;
        return normalizedPos * scoreManager.okWindow;
    }

    /// <summary>
    /// Flash the zone that was hit
    /// </summary>
    public void FlashZone(ScoreManager.TimingRating rating)
    {
        Image zoneToFlash = null;

        switch (rating)
        {
            case ScoreManager.TimingRating.Perfect:
                zoneToFlash = perfectZone;
                break;
            case ScoreManager.TimingRating.Great:
                zoneToFlash = greatZone;
                break;
            case ScoreManager.TimingRating.Good:
                zoneToFlash = goodZone;
                break;
            case ScoreManager.TimingRating.OK:
                zoneToFlash = okZone;
                break;
        }

        if (zoneToFlash != null)
        {
            StartCoroutine(FlashZoneCoroutine(zoneToFlash));
        }
    }

    private System.Collections.IEnumerator FlashZoneCoroutine(Image zone)
    {
        Color originalColor = zone.color;
        Color flashColor = new Color(1f, 1f, 1f, 0.9f);

        zone.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        zone.color = originalColor;
    }

    /// <summary>
    /// Show/hide the timing zone indicator
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    /// <summary>
    /// Update settings when BPM changes
    /// </summary>
    public void OnBPMChanged(float newBPM)
    {
        // Arrow speed is tied to BPM through RhythmJudge
        // No additional changes needed here
    }
}