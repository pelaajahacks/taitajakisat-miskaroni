using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShowSpeedrunTimeTrigger : MonoBehaviour
{
    [Header("UI Display")]
    public TMP_Text displayText;        // TMP Text to show times
    public float displayDuration = 5f;  // How long to show the UI

    [Header("Scene Settings")]
    public int sceneIndexToLoad = 0;    // Scene index to load after display

    private string bestTimeKey = "BestTime"; // PlayerPrefs key

    private void Start()
    {
        if (displayText != null)
            displayText.gameObject.SetActive(false); // hide at start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SpeedrunTimer timer = FindObjectOfType<SpeedrunTimer>();
        if (timer != null && displayText != null)
        {
            float currentTime = timer.currentTime;

            // Load previously saved best time
            float bestTime = PlayerPrefs.GetFloat(bestTimeKey, Mathf.Infinity);

            // If current run is better, save it
            if (currentTime < bestTime)
            {
                bestTime = currentTime;
                PlayerPrefs.SetFloat(bestTimeKey, bestTime);
                PlayerPrefs.Save();
            }

            // Format times
            string currentTimeStr = FormatTime(currentTime);
            string bestTimeStr = bestTime == Mathf.Infinity ? "--:--.--" : FormatTime(bestTime);

            displayText.text = $"Best Time: {bestTimeStr}\nCurrent Time: {currentTimeStr}";
            displayText.gameObject.SetActive(true);

            StartCoroutine(HideAndLoadScene());
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    private IEnumerator HideAndLoadScene()
    {
        yield return new WaitForSeconds(displayDuration);

        if (displayText != null)
            displayText.gameObject.SetActive(false);

        SceneManager.LoadScene(sceneIndexToLoad);
    }
}