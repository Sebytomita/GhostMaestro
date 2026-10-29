using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NamePanelController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject namePanel;
    public TMP_InputField nameInput;
    public Button confirmButton;
    public TMP_Text errorLabel;  

    [Header("Settings")]
    public int minLen = 2;
    public int maxLen = 20;
    public string storySceneName = "Story";   

    void Awake()
    {
        if (namePanel) namePanel.SetActive(false);
        if (errorLabel) errorLabel.gameObject.SetActive(false);
        if (confirmButton) confirmButton.onClick.AddListener(Confirm);
        if (nameInput) nameInput.onValueChanged.AddListener(_ => ValidateName());
    }

    public void Open()
    {
        if (!namePanel) return;
        namePanel.SetActive(true);
        if (nameInput)
        {
            nameInput.text = "";
            nameInput.Select();
            nameInput.ActivateInputField();
        }
        ValidateName();
    }

    public void Close()
    {
        if (namePanel) namePanel.SetActive(false);
    }

    void Update()
    {
        if (namePanel && namePanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            if (confirmButton && confirmButton.interactable) Confirm();
        }
    }

    void ValidateName()
    {
        if (!nameInput || !confirmButton) return;

        string n = (nameInput.text ?? "").Trim();
        bool ok = n.Length >= minLen && n.Length <= maxLen;

        confirmButton.interactable = ok;
        if (errorLabel)
        {
            if (ok) errorLabel.gameObject.SetActive(false);
            else
            {
                errorLabel.gameObject.SetActive(true);
                errorLabel.text = $"Introduce un nume ({minLen}-{maxLen} caractere).";
            }
        }
    }

    public void Confirm()
    {
        if (!nameInput) return;
        string n = (nameInput.text ?? "").Trim();

        if (n.Length < minLen || n.Length > maxLen) { ValidateName(); return; }

        PlayerPrefs.SetString("PLAYER_NAME", n);
        PlayerPrefs.SetInt("HAS_SAVE", 1); 
        PlayerPrefs.Save();

        SceneManager.LoadScene(storySceneName);
    }
}
