using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Globalization;
using System.Text.RegularExpressions;

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject namePanel;
    public GameObject playPanel;
    public GameObject settingsPanel;
    public GameObject controllersPanel;
    
    [Header("Play Panel")]
    public Button continueButton;
    public TMP_InputField nameInput;
    public Button btnConfirmName;

    public TextMeshProUGUI errorNameTMP;

    void Start()
    {
        ShowMain();
        if (continueButton) continueButton.interactable = SaveSystem.HasSave();
    }

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        playPanel.SetActive(false);
        settingsPanel.SetActive(false);
        if (controllersPanel) controllersPanel.SetActive(false); 
    }

    public void ShowPlay()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
        settingsPanel.SetActive(false);
        if (controllersPanel) controllersPanel.SetActive(false);
        if (continueButton) continueButton.interactable = SaveSystem.HasSave();
    }

    public void ShowNameInput()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(false);
        settingsPanel.SetActive(false);

        namePanel.SetActive(true);

        if (nameInput)
        {
            EventSystem.current.SetSelectedGameObject(nameInput.gameObject);
            nameInput.text = "";
            nameInput.ActivateInputField();
        }
    }

    public void OnNameChanged(string raw)
    {
        // oprește butonul până e valid
        if (btnConfirmName) btnConfirmName.interactable = IsValidName(raw);
        // ascunde eroarea la editare
        if (errorNameTMP) errorNameTMP.gameObject.SetActive(false);
    }

    bool IsValidName(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;
        s = s.Trim();
        if (s.Length < 2 || s.Length > 20) return false;
        // doar litere, spații și cratimă
        return Regex.IsMatch(s, @"^[\p{L} \-]+$");
    }

    string ToTitleCase(string s)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.Trim().ToLower());
    }

    
    public void ConfirmNameAndStart()
    {
        var s = nameInput ? nameInput.text : "";
        if (!IsValidName(s))
        {
            if (errorNameTMP)
            {
                errorNameTMP.text = "Introduce un nume (2–20 caractere).";
                errorNameTMP.gameObject.SetActive(true);
            }
            if (btnConfirmName) btnConfirmName.interactable = false;
            return;
        }
        s = ToTitleCase(s);
        PlayerPrefs.SetString("PLAYER_NAME", s);
        PlayerPrefs.Save();
        NewGame(); // sau Show story
    }
    


    public void BackFromName()
    {
        namePanel.SetActive(false);
        ShowPlay(); 
    }

    public void ShowSettings()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(false);
        settingsPanel.SetActive(true);
        if (controllersPanel) controllersPanel.SetActive(false);
    }

    
    public void ShowControllers()
    {
        if (!controllersPanel) return;
        settingsPanel.SetActive(false);
        controllersPanel.SetActive(true);
    }



    public void BackFromControllers()
    {
        if (!controllersPanel) return;
        controllersPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    

    public void NewGame()
    {
        SaveSystem.Clear();
        SceneManager.LoadScene("Story");
    }

    public void ContinueGame()
    {
        if (!SaveSystem.HasSave()) return;
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
