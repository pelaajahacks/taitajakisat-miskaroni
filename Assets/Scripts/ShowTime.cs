using UnityEngine;
using TMPro;

public class ShowBestTime : MonoBehaviour
{
    public TMP_Text text;
    private const string BestTimeKey = "BestTime";

    void Start()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        float bestTime = PlayerPrefs.GetFloat(BestTimeKey, float.MaxValue);

        if (bestTime == float.MaxValue)
        {
            text.text = "Best Time: --:--.---";
        }
        else
        {
            text.text = "Best Time: " + FormatTime(bestTime);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
