using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Creează obiectele UI pentru Score în Editor - rămân permanent în Hierarchy
/// Click dreapta pe script -> "Create Score UI In Scene"
/// </summary>
public class ScoreUICreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create Score UI (Ghost Maestro)", false, 10)]
    static void CreateScoreUI()
    {
        // Găsește LessonUI Canvas
        Canvas canvas = null;
        GameObject lessonUI = GameObject.Find("LessonUI");
        if (lessonUI != null)
            canvas = lessonUI.GetComponent<Canvas>();
        
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Nu am găsit niciun Canvas! Creează unul mai întâi.");
            return;
        }

        // ============================================
        // 1. SCORE PANEL (dreapta-sus)
        // ============================================
        GameObject scorePanel = new GameObject("ScorePanel");
        scorePanel.transform.SetParent(canvas.transform, false);
        
        RectTransform scorePanelRect = scorePanel.AddComponent<RectTransform>();
        scorePanelRect.anchorMin = new Vector2(1, 1);
        scorePanelRect.anchorMax = new Vector2(1, 1);
        scorePanelRect.pivot = new Vector2(1, 1);
        scorePanelRect.anchoredPosition = new Vector2(-20, -20);
        scorePanelRect.sizeDelta = new Vector2(280, 130);

        Image scorePanelBg = scorePanel.AddComponent<Image>();
        scorePanelBg.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);
        // MANUAL: Setează Material = UI_Overlay

        // Score Text
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(scorePanel.transform, false);
        
        RectTransform scoreTextRect = scoreTextObj.AddComponent<RectTransform>();
        scoreTextRect.anchorMin = new Vector2(0, 1);
        scoreTextRect.anchorMax = new Vector2(1, 1);
        scoreTextRect.pivot = new Vector2(0.5f, 1);
        scoreTextRect.anchoredPosition = new Vector2(0, -15);
        scoreTextRect.sizeDelta = new Vector2(-30, 40);

        TextMeshProUGUI scoreText = scoreTextObj.AddComponent<TextMeshProUGUI>();
        scoreText.text = "🎵 0";
        scoreText.fontSize = 32;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.alignment = TextAlignmentOptions.Right;
        scoreText.color = Color.white;
        // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay

        // Combo Text
        GameObject comboTextObj = new GameObject("ComboText");
        comboTextObj.transform.SetParent(scorePanel.transform, false);
        
        RectTransform comboTextRect = comboTextObj.AddComponent<RectTransform>();
        comboTextRect.anchorMin = new Vector2(0, 1);
        comboTextRect.anchorMax = new Vector2(1, 1);
        comboTextRect.pivot = new Vector2(0.5f, 1);
        comboTextRect.anchoredPosition = new Vector2(0, -55);
        comboTextRect.sizeDelta = new Vector2(-30, 35);

        TextMeshProUGUI comboText = comboTextObj.AddComponent<TextMeshProUGUI>();
        comboText.text = "🔥 10x COMBO";
        comboText.fontSize = 24;
        comboText.fontStyle = FontStyles.Bold;
        comboText.alignment = TextAlignmentOptions.Right;
        comboText.color = new Color(1f, 0.9f, 0.3f);
        // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay
        comboTextObj.SetActive(false); // Ascuns inițial

        // Multiplier Text
        GameObject multTextObj = new GameObject("MultiplierText");
        multTextObj.transform.SetParent(scorePanel.transform, false);
        
        RectTransform multTextRect = multTextObj.AddComponent<RectTransform>();
        multTextRect.anchorMin = new Vector2(0, 1);
        multTextRect.anchorMax = new Vector2(1, 1);
        multTextRect.pivot = new Vector2(0.5f, 1);
        multTextRect.anchoredPosition = new Vector2(0, -90);
        multTextRect.sizeDelta = new Vector2(-30, 30);

        TextMeshProUGUI multText = multTextObj.AddComponent<TextMeshProUGUI>();
        multText.text = "×1.5";
        multText.fontSize = 20;
        multText.alignment = TextAlignmentOptions.Right;
        multText.color = new Color(0.6f, 1f, 0.6f);
        // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay
        multTextObj.SetActive(false); // Ascuns inițial

        // ============================================
        // 2. STARS DISPLAY (stânga-sus)
        // ============================================
        GameObject starsDisplay = new GameObject("StarsDisplay");
        starsDisplay.transform.SetParent(canvas.transform, false);
        
        RectTransform starsRect = starsDisplay.AddComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(0, 1);
        starsRect.anchorMax = new Vector2(0, 1);
        starsRect.pivot = new Vector2(0, 1);
        starsRect.anchoredPosition = new Vector2(20, -20);
        starsRect.sizeDelta = new Vector2(260, 50);

        Image starsBg = starsDisplay.AddComponent<Image>();
        starsBg.color = new Color(0.1f, 0.1f, 0.15f, 0.7f);
        // MANUAL: Setează Material = UI_Overlay

        HorizontalLayoutGroup starsLayout = starsDisplay.AddComponent<HorizontalLayoutGroup>();
        starsLayout.padding = new RectOffset(15, 15, 8, 8);
        starsLayout.spacing = 8;
        starsLayout.childAlignment = TextAnchor.MiddleCenter;
        starsLayout.childControlWidth = false;
        starsLayout.childControlHeight = false;

        // Creează 5 stele - folosim "O" care merge sigur cu orice font
        for (int i = 0; i < 5; i++)
        {
            GameObject star = new GameObject($"Star{i + 1}");
            star.transform.SetParent(starsDisplay.transform, false);
            
            RectTransform starRect = star.AddComponent<RectTransform>();
            starRect.sizeDelta = new Vector2(34, 34);

            // Folosim Image în loc de text - mai sigur
            Image starImg = star.AddComponent<Image>();
            starImg.color = new Color(0.4f, 0.4f, 0.4f, 0.5f); // Gri inițial
            // MANUAL: Setează Material = UI_Overlay
        }

        // ============================================
        // 3. TIMING FEEDBACK (centru)
        // ============================================
        GameObject timingFeedback = new GameObject("TimingFeedbackText");
        timingFeedback.transform.SetParent(canvas.transform, false);
        
        RectTransform feedbackRect = timingFeedback.AddComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.5f, 0.7f);
        feedbackRect.anchorMax = new Vector2(0.5f, 0.7f);
        feedbackRect.pivot = new Vector2(0.5f, 0.5f);
        feedbackRect.anchoredPosition = Vector2.zero;
        feedbackRect.sizeDelta = new Vector2(600, 120);

        TextMeshProUGUI feedbackText = timingFeedback.AddComponent<TextMeshProUGUI>();
        feedbackText.text = "PERFECT!";
        feedbackText.fontSize = 72;
        feedbackText.fontStyle = FontStyles.Bold;
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.color = new Color(1f, 0.84f, 0f); // Gold
        // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay
        timingFeedback.SetActive(false); // Ascuns inițial

        // ============================================
        // 4. END SCREEN STATS (pentru CompletionPanel)
        // ============================================
        GameObject completePanel = GameObject.Find("CompletePanel");
        if (completePanel == null)
        {
            // Caută și în LessonUI
            Transform found = canvas.transform.Find("CompletePanel");
            if (found != null) completePanel = found.gameObject;
        }
        
        // Caută recursiv
        if (completePanel == null)
        {
            foreach (Transform child in canvas.transform)
            {
                Transform found = child.Find("CompletePanel");
                if (found != null)
                {
                    completePanel = found.gameObject;
                    break;
                }
            }
        }

        if (completePanel != null)
        {
            Debug.Log("✅ Am găsit CompletePanel: " + completePanel.name);
            
            // Stats Container - CENTRAT în panel
            GameObject statsContainer = new GameObject("ScoreStatsContainer");
            statsContainer.transform.SetParent(completePanel.transform, false);
            
            RectTransform statsRect = statsContainer.AddComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(0.1f, 0.15f);
            statsRect.anchorMax = new Vector2(0.9f, 0.85f);
            statsRect.offsetMin = Vector2.zero;
            statsRect.offsetMax = Vector2.zero;

            // Background semi-transparent
            Image statsBg = statsContainer.AddComponent<Image>();
            statsBg.color = new Color(0, 0, 0, 0.5f);
            // MANUAL: Material = UI_Overlay

            VerticalLayoutGroup statsLayout = statsContainer.AddComponent<VerticalLayoutGroup>();
            statsLayout.padding = new RectOffset(30, 30, 30, 30);
            statsLayout.spacing = 20;
            statsLayout.childAlignment = TextAnchor.MiddleCenter;
            statsLayout.childControlWidth = true;
            statsLayout.childControlHeight = false;
            statsLayout.childForceExpandWidth = true;

            // End Stars Container
            GameObject endStars = new GameObject("EndScreenStars");
            endStars.transform.SetParent(statsContainer.transform, false);
            
            RectTransform endStarsRect = endStars.AddComponent<RectTransform>();
            endStarsRect.sizeDelta = new Vector2(350, 80);

            HorizontalLayoutGroup endStarsLayout = endStars.AddComponent<HorizontalLayoutGroup>();
            endStarsLayout.spacing = 15;
            endStarsLayout.childAlignment = TextAnchor.MiddleCenter;
            endStarsLayout.childControlWidth = false;
            endStarsLayout.childControlHeight = false;

            LayoutElement endStarsLE = endStars.AddComponent<LayoutElement>();
            endStarsLE.preferredHeight = 80;

            // 5 stele pentru end screen - folosim Image
            for (int i = 0; i < 5; i++)
            {
                GameObject star = new GameObject($"EndStar{i + 1}");
                star.transform.SetParent(endStars.transform, false);
                
                RectTransform starRect = star.AddComponent<RectTransform>();
                starRect.sizeDelta = new Vector2(60, 60);

                Image starImg = star.AddComponent<Image>();
                starImg.color = new Color(0.4f, 0.4f, 0.4f, 0.5f);
                // MANUAL: Setează Material = UI_Overlay
            }

            // Final Score Label
            GameObject scoreLabelObj = new GameObject("ScoreLabel");
            scoreLabelObj.transform.SetParent(statsContainer.transform, false);
            
            TextMeshProUGUI scoreLabelText = scoreLabelObj.AddComponent<TextMeshProUGUI>();
            scoreLabelText.text = "SCOR FINAL";
            scoreLabelText.fontSize = 24;
            scoreLabelText.alignment = TextAlignmentOptions.Center;
            scoreLabelText.color = new Color(0.8f, 0.8f, 0.8f);
            // MANUAL: Material Preset = LiberationSans SDF - Overlay

            LayoutElement scoreLabelLE = scoreLabelObj.AddComponent<LayoutElement>();
            scoreLabelLE.preferredHeight = 30;

            // Final Score Text - MARE
            GameObject finalScoreObj = new GameObject("FinalScoreText");
            finalScoreObj.transform.SetParent(statsContainer.transform, false);
            
            TextMeshProUGUI finalScoreText = finalScoreObj.AddComponent<TextMeshProUGUI>();
            finalScoreText.text = "12,500";
            finalScoreText.fontSize = 64;
            finalScoreText.fontStyle = FontStyles.Bold;
            finalScoreText.alignment = TextAlignmentOptions.Center;
            finalScoreText.color = new Color(1f, 0.84f, 0f);
            // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay

            LayoutElement finalScoreLE = finalScoreObj.AddComponent<LayoutElement>();
            finalScoreLE.preferredHeight = 75;

            // Accuracy Text
            GameObject accuracyObj = new GameObject("AccuracyText");
            accuracyObj.transform.SetParent(statsContainer.transform, false);
            
            TextMeshProUGUI accuracyText = accuracyObj.AddComponent<TextMeshProUGUI>();
            accuracyText.text = "Acuratețe: 94.5%";
            accuracyText.fontSize = 36;
            accuracyText.alignment = TextAlignmentOptions.Center;
            accuracyText.color = new Color(0.3f, 1f, 0.3f);
            // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay

            LayoutElement accLE = accuracyObj.AddComponent<LayoutElement>();
            accLE.preferredHeight = 45;

            // Max Combo Text
            GameObject maxComboObj = new GameObject("MaxComboText");
            maxComboObj.transform.SetParent(statsContainer.transform, false);
            
            TextMeshProUGUI maxComboText = maxComboObj.AddComponent<TextMeshProUGUI>();
            maxComboText.text = "Max Combo: 25x";
            maxComboText.fontSize = 32;
            maxComboText.alignment = TextAlignmentOptions.Center;
            maxComboText.color = new Color(1f, 0.6f, 0.2f);
            // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay

            LayoutElement comboLE = maxComboObj.AddComponent<LayoutElement>();
            comboLE.preferredHeight = 40;

            // Stats Breakdown Text
            GameObject breakdownObj = new GameObject("StatsBreakdownText");
            breakdownObj.transform.SetParent(statsContainer.transform, false);
            
            TextMeshProUGUI breakdownText = breakdownObj.AddComponent<TextMeshProUGUI>();
            breakdownText.text = "Perfect: 10 | Great: 5 | Good: 2 | OK: 1 | Miss: 0";
            breakdownText.fontSize = 20;
            breakdownText.alignment = TextAlignmentOptions.Center;
            breakdownText.color = new Color(0.7f, 0.7f, 0.7f);
            // MANUAL: Setează Material Preset = LiberationSans SDF - Overlay

            LayoutElement breakdownLE = breakdownObj.AddComponent<LayoutElement>();
            breakdownLE.preferredHeight = 30;
            
            Debug.Log("✅ ScoreStatsContainer creat în CompletePanel!");
        }
        else
        {
            Debug.LogWarning("⚠️ CompletePanel nu a fost găsit! Creează manual EndScreenStars.");
        }

        // Dezactivează ScorePanel și StarsDisplay inițial
        scorePanel.SetActive(false);
        starsDisplay.SetActive(false);

        // Selectează ScorePanel în Editor
        Selection.activeGameObject = scorePanel;

        Debug.Log("✅ Score UI creat! Acum setează manual:");
        Debug.Log("   - Pe toate Image: Material = UI_Overlay");
        Debug.Log("   - Pe toate TextMeshPro: Material Preset = LiberationSans SDF - Overlay");
        Debug.Log("   - Apoi trage referințele în GhostMaestroManager");
    }
#endif
}