using UnityEngine;

/// <summary>
/// Simple helper to reset lesson progress
/// </summary>
public class ResetLessonProgress : MonoBehaviour
{
    [ContextMenu("Reset All Progress")]
    public void ResetProgress()
    {
        // Clear lesson progress
        PlayerPrefs.DeleteKey("LESSON_PROGRESS");

        // Clear player name
        PlayerPrefs.DeleteKey("PLAYER_NAME");

        // Clear old save data
        SaveSystem.Clear();

        PlayerPrefs.Save();

        Debug.Log("✅ All progress reset! Restart the game.");
    }

    [ContextMenu("Reset Only Lesson Progress")]
    public void ResetLessonsOnly()
    {
        PlayerPrefs.DeleteKey("LESSON_PROGRESS");
        PlayerPrefs.Save();

        Debug.Log("✅ Lesson progress reset!");

        // Restart current lesson
        GhostMaestroManager manager = FindObjectOfType<GhostMaestroManager>();
        if (manager)
        {
            manager.StartLesson(0);
        }
    }
}