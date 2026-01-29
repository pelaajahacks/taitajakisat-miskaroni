using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShowSpeedrunTimeTrigger : MonoBehaviour
{
    [Header("UI Display")]
    public TMP_Text displayText; // Assign a TMP Text that will show the time
    public float displayDuration = 5f; // How long to show the time

    [Header("Scene Settings")]
    public int sceneIndexToLoad = 0; // Scene index to load after display

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
            float time = timer.currentTime; // grab current time
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

            displayText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            displayText.gameObject.SetActive(true);

            StartCoroutine(HideAndLoadScene());
        }
    }

    private IEnumerator HideAndLoadScene()
    {
        yield return new WaitForSeconds(displayDuration);

        if (displayText != null)
            displayText.gameObject.SetActive(false);

        SceneManager.LoadScene(sceneIndexToLoad);
    }
}