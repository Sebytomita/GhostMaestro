using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    public float autoNextAfterSeconds = 6f;
    void Start()
    {
        if (autoNextAfterSeconds > 0f)
            Invoke(nameof(GoNext), autoNextAfterSeconds);
    }

    void Update()
    {
        if (Input.anyKeyDown) GoNext(); // click/tasta → treci mai departe
    }

    public void GoNext()
    {
        SceneManager.LoadScene("Game");
    }
}
