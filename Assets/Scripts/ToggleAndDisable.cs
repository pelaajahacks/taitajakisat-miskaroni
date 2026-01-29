using System.Threading;
using UnityEngine;

public class TriggerToggleObjects : MonoBehaviour
{
    [Header("Objects To Enable")]
    public GameObject[] enableObjects;

    [Header("Objects To Disable")]
    public GameObject[] disableObjects;

    [Header("Trigger Settings")]
    public bool onlyOnce = true;

    private bool triggered = false;



    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (onlyOnce && triggered) return;

        triggered = true;

        var state = FindAnyObjectByType<SpeedrunTimer>();
        state.isRunning = true;

        // Enable objects
        foreach (GameObject obj in enableObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Disable objects
        foreach (GameObject obj in disableObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}