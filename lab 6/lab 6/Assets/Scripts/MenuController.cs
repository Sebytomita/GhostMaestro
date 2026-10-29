using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject playPanel;
    public GameObject settingsPanel;
    public GameObject controllersPanel;   // <— NOU

    [Header("Play Panel")]
    public Button continueButton;

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
        if (controllersPanel) controllersPanel.SetActive(false);  // <— ascunde și Controllers
    }

    public void ShowPlay()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
        settingsPanel.SetActive(false);
        if (controllersPanel) controllersPanel.SetActive(false);
        if (continueButton) continueButton.interactable = SaveSystem.HasSave();
    }

    public void ShowSettings()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(false);
        settingsPanel.SetActive(true);
        if (controllersPanel) controllersPanel.SetActive(false);
    }

    // —— NOU: intrare/ieșire Controllers —— //
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
    // ———————————————————————— //

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
