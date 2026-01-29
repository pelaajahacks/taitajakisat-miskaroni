using UnityEngine;
using TMPro; // Remove if not using TMP
using UnityEngine.UI;
using System.Collections.Generic;

public class SpeedrunTimer : MonoBehaviour
{
    [Header("Timer Mode")]
    public bool countdownMode = false;
    [SerializeField] private bool startBlocker = false;
    

    [Header("Starting Time (Example: 5 minutes)")]
    [SerializeField] private float startSeconds = 5f;

    [Header("Auto Start")]
    [SerializeField] private bool startOnPlay = true;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private Text uiText;

    [Header("Removing objects")]
    [SerializeField] private List<GameObject> objectsToRemove;

    public float currentTime;
    public bool isRunning;

    public bool timerEnded = false;

    void Start()
    {
        ResetTimer();

        if (startOnPlay)
            StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime += countdownMode ? -Time.deltaTime : Time.deltaTime;

        if (countdownMode && currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            timerEnded = true;

            if (startBlocker)
            {
                // Implement start blocker logic here
                Debug.Log("Timer ended. Start Blocker activated.");
                
                isRunning = true;
                currentTime = startSeconds;
                countdownMode = false;
                startSeconds = 0f;

                foreach (GameObject obj in objectsToRemove)
                {
                    if (obj != null)
                        Destroy(obj);
                }
                ResetTimer();

            }
        }


        UpdateUI();
    }

    // --------------------
    // Public Controls
    // --------------------

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = startSeconds;
        UpdateUI();
    }

    // --------------------
    // UI Formatting
    // --------------------

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime * 1000f) % 1000f);

        string timeString = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

        if (tmpText != null)
            tmpText.text = timeString;

        if (uiText != null)
            uiText.text = timeString;
    }
}
