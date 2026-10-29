using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StoryController : MonoBehaviour
{
    [Header("Bindings")]
    public TextMeshProUGUI titleTMP;
    public TextMeshProUGUI bodyTMP;
    public TextMeshProUGUI hintTMP;
    public CanvasGroup cardGroup;        

    [Header("Next Scene")]
    public string nextSceneName = "Game";

    [Header("Timings")]
    public float fadeDuration = 0.25f;
    public float typingSpeed = 0.025f;   // sec/char
    public float commaPause = 0.10f;     // pauză la „,;:”
    public float dotPause = 0.18f;     // pauză la „.!?…”

    
    int page = 0;
    bool isTyping = false;
    bool isFading = false;
    Coroutine typingCo;

    readonly string cardTitle = "Camera în care timpul tace";

   
    readonly string[] pageTemplates = new[]
    {
        // 1
        "\nÎn mansarda casei vechi, praful plutea ca o ninsoare lentă peste pianul acoperit. " +
        "Pe capac, cineva scrijelise cândva trei note — un început de frază pe care nimeni nu l-a mai continuat.\n\n" +
        "Astăzi, ușa scârțâie iar tu pășești înauntru. Aerul miroase a lemn, cerneală și amintiri. " +
        "Când degetele îți ating clapele, un sunet mic, nesigur, trezește camera. Timpul se încruntă… și ascultă, {NAME}.",

        // 2
        "\nDe undeva, din spatele pereților, se aude o bătaie de metronom — constantă, răbdătoare. " +
        "Fiecare tic îți potrivește respirația. Între două respirații, auzi o temă. Între două bătăi, " +
        "îi auzi pașii: ai compozitorului care a tăcut la jumătatea frazei.\n\n" +
        "Nu știi dacă te cheamă sau te roagă să nu-l lași în uitare. Dar știi că povestea lui, " +
        "dacă va mai fi spusă, are nevoie de mâinile tale, {NAME}.",

        // 3
        "\nScoți pânza, lași lumina să cadă pe fildeș. Trei note îți rămân pe vârfurile degetelor. " +
        "Apoi, fără să te mai gândești, adaugi a patra. Camera tresare. Metronomul își schimbă mersul.\n\n" +
        "Poate că nu poți aduce înapoi trecutul. Dar poți scrie continuarea. Iar muzica — " +
        "dacă o asculți cu răbdare — îți va arăta drumul, {NAME}."
    };

    string[] pages;
    string playerName;

    void Start()
    {
        if (!titleTMP || !bodyTMP || !cardGroup)
        {
            Debug.LogError("StoryController: leagă Title/Body/CanvasGroup în Inspector.");
            enabled = false;
            return;
        }

        
        playerName = PlayerPrefs.GetString("PLAYER_NAME", "musafir");

        pages = new string[pageTemplates.Length];
        for (int i = 0; i < pageTemplates.Length; i++)
            pages[i] = pageTemplates[i].Replace("{NAME}", playerName);

        titleTMP.text = cardTitle;
        if (hintTMP) hintTMP.text = "ENTER — continuă / skip";
        cardGroup.alpha = 1f;

        page = 0;
        ShowPageInstant(page);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            OnEnter();
    }

    void OnEnter()
    {
        if (isFading) return;

        if (isTyping)
        {
            
            StopTypingAndRevealAll();
            if (hintTMP) hintTMP.text = "ENTER — continuă";
        }
        else
        {
            
            NextPage();
        }
    }

    void NextPage()
    {
        int next = page + 1;
        if (next >= pages.Length)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }
        StartCoroutine(FadeTo(next));
    }

    IEnumerator FadeTo(int nextPage)
    {
        isFading = true;

        // fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cardGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        
        ShowPageInstant(nextPage);

        // fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cardGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        cardGroup.alpha = 1f;
        isFading = false;
    }

    void ShowPageInstant(int p)
    {
        page = p;
        bodyTMP.maxVisibleCharacters = 0;
        bodyTMP.text = pages[page];

        if (hintTMP) hintTMP.text = "ENTER — skip";

        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = StartCoroutine(Typewriter());
    }

    IEnumerator Typewriter()
    {
        isTyping = true;

        bodyTMP.ForceMeshUpdate();
        int total = bodyTMP.textInfo.characterCount;
        int vis = 0;

        while (vis < total)
        {
            vis++;
            bodyTMP.maxVisibleCharacters = vis;

            
            char c = GetCharAt(bodyTMP.text, vis - 1);
            float delay = typingSpeed;
            if (c == ',' || c == ';' || c == ':') delay += commaPause;
            if (c == '.' || c == '!' || c == '?' || c == '…') delay += dotPause;

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        typingCo = null;
        if (hintTMP) hintTMP.text = "ENTER — continuă";
    }

    void StopTypingAndRevealAll()
    {
        if (typingCo != null) StopCoroutine(typingCo);
        bodyTMP.ForceMeshUpdate();
        bodyTMP.maxVisibleCharacters = bodyTMP.textInfo.characterCount;
        isTyping = false;
        typingCo = null;
    }

    static char GetCharAt(string s, int idx)
    {
        if (idx < 0 || idx >= s.Length) return '\0';
        return s[idx];
    }
}
