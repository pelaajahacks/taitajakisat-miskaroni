using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger on player (optional but recommended)
        if (!other.CompareTag("Player"))
            return;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Safety: Loop back if no next scene
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            nextIndex = 0;

        SceneManager.LoadScene(nextIndex);
    }
}
