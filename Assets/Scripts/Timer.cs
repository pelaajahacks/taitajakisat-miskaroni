using UnityEngine;
using TMPro; // Remove if not using TMP
using UnityEngine.UI;

public class SpeedrunTimer : MonoBehaviour
{
    [Header("Timer Mode")]
    [SerializeField] private bool countdownMode = false;

    [Header("Starting Time (Example: 5 minutes)")]
    [SerializeField] private float startMinutes = 5f;

    [Header("Auto Start")]
    [SerializeField] private bool startOnPlay = true;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private Text uiText;

    private float currentTime;
    private bool isRunning;

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
        currentTime = startMinutes * 60f;
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
