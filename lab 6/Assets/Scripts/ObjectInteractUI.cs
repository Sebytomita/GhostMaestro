using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectInteractUI : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;           
    public CanvasGroup promptGroup;        
    public GameObject detailPanel;         
    public Image mainImage;          
    public TMP_Text mainText;             
    public Transform billboardRoot;        

    [Header("Settings")]
    public float showDistance = 2.0f;      
    public bool facePlayer = true;        

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

        float target = showPrompt ? 1f : 0f;
        fadeT = Mathf.MoveTowards(fadeT, target, Time.deltaTime * 5f);
        promptGroup.alpha = fadeT;
        promptGroup.interactable = showPrompt;
        promptGroup.blocksRaycasts = showPrompt;

        if (facePlayer && billboardRoot)
        {
            Vector3 look = playerHead.position;
            look.y = billboardRoot.position.y;
            billboardRoot.LookAt(look);
            billboardRoot.Rotate(0, 180, 0);
        }
    }

    public void OpenDetail()
    {
        if (!detailPanel) return;
        detailPanel.SetActive(true);

        promptGroup.alpha = 0f;
        promptGroup.interactable = false;
        promptGroup.blocksRaycasts = false;

        if (mainText && string.IsNullOrEmpty(mainText.text))
            mainText.text = "Istoria obiectului... scrie aici textul dorit.";
    }

    public void CloseDetail()
    {
        if (!detailPanel) return;
        detailPanel.SetActive(false);
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
