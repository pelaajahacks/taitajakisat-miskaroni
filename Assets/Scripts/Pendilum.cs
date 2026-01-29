using UnityEngine;
using System.Collections;

public class RandomZMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxZOffset = 5f;      // max distance to move along Z
    public float moveDuration = 0.5f;  // time to reach target
    public float waitTime = 2f;        // how long it stays at target

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            // Pick a random Z offset
            float randomZ = Random.Range(-maxZOffset, maxZOffset);
            Vector3 targetPos = new Vector3(startPos.x, startPos.y, startPos.z + randomZ);

            // Move to target
            yield return StartCoroutine(MoveToPosition(targetPos, moveDuration));

            // Wait at target
            yield return new WaitForSeconds(waitTime);

            // Return to start
            yield return StartCoroutine(MoveToPosition(startPos, moveDuration));

            // Optional small delay before next move
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 initialPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(initialPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}