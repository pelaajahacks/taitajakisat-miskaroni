using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimedDestroy : MonoBehaviour
{
    [Header("Countdown Settings")]
    public float countdownTime = 3f;  // seconds
    public Text countdownText;        // UI Text to show timer

    void Start()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        StartCoroutine(CountdownAndDestroy());
    }

    IEnumerator CountdownAndDestroy()
    {
        float timer = countdownTime;

        while (timer > 0)
        {
            if (countdownText != null)
                countdownText.text = timer.ToString("F1"); // show 1 decimal

            timer -= Time.deltaTime;
            yield return null;
        }

        // Final update to show 0
        if (countdownText != null)
            countdownText.text = "0";

        Destroy(gameObject);
    }
}
