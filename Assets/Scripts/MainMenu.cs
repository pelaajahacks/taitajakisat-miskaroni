using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene switching

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the next scene (make sure it's in Build Settings)
        SceneManager.LoadScene("Miska");
    }

    public void QuitGame()
    {
        Debug.Log("Quit!"); // Just for testing in Editor
        Application.Quit(); // Will quit the game in build
    }
}
