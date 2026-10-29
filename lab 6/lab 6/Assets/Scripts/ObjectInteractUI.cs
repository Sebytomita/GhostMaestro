using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectInteractUI : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;           // XROrigin/Camera Offset/Main Camera
    public CanvasGroup promptGroup;        // Canvas Group de pe Canvas (sau PromptPanel)
    public GameObject detailPanel;         // Panelul mare
    public Image mainImage;                // Imaginea din panou (optional)
    public TMP_Text mainText;              // Textul din panou (optional)
    public Transform billboardRoot;        // De obicei chiar Canvas-ul

    [Header("Settings")]
    public float showDistance = 2.0f;      // distanța de apariție a promptului
    public bool facePlayer = true;         // orientează canvasul spre player

    float fadeT = 0f;

    void Reset()
    {
        if (Camera.main != null) playerHead = Camera.main.transform;
    }

    void Update()
    {
        if (!playerHead || !promptGroup) return;

        float d = Vector3.Distance(playerHead.position, transform.position);
        bool showPrompt = d <= showDistance && detailPanel != null && !detailPanel.activeSelf;

        // mic fade pentru prompt
        float target = showPrompt ? 1f : 0f;
        fadeT = Mathf.MoveTowards(fadeT, target, Time.deltaTime * 5f);
        promptGroup.alpha = fadeT;
        promptGroup.interactable = showPrompt;
        promptGroup.blocksRaycasts = showPrompt;

        // billboarding (canvasul se uită spre player)
        if (facePlayer && billboardRoot)
        {
            Vector3 look = playerHead.position;
            look.y = billboardRoot.position.y;
            billboardRoot.LookAt(look);
            billboardRoot.Rotate(0, 180, 0);
        }
    }

    // apelată de BtnInteract
    public void OpenDetail()
    {
        if (!detailPanel) return;
        detailPanel.SetActive(true);

        // ascunde promptul când panoul e deschis
        promptGroup.alpha = 0f;
        promptGroup.interactable = false;
        promptGroup.blocksRaycasts = false;

        // exemplu conținut; poți seta din Inspector
        if (mainText && string.IsNullOrEmpty(mainText.text))
            mainText.text = "Istoria obiectului... scrie aici textul dorit.";
    }

    // apelată de BtnBack
    public void CloseDetail()
    {
        if (!detailPanel) return;
        detailPanel.SetActive(false);
        // Promptul va reapărea în Update dacă ești încă în raza de proximitate
    }

    public bool IsOpen => detailPanel != null && detailPanel.activeSelf;

    public void ToggleDetail()
    {
        if (!detailPanel) return;

        if (detailPanel.activeSelf)
            CloseDetail();
        else
            OpenDetail();
    }

}
