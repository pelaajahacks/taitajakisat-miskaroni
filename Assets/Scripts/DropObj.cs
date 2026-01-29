using UnityEngine;
using System.Collections;

public class RandomDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    public float dropDistance = 3f;
    public float dropSpeed = 4f;
    public float riseSpeed = 2f;

    [Header("Timing")]
    public float minWait = 2f;
    public float maxWait = 6f;
    public float stayDownTime = 2f;

    Vector3 startPos;
    bool moving = false;

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        while (true)
        {
            // Random wait before dropping
            float wait = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(wait);

            if (!moving)
                yield return StartCoroutine(DropAndRise());
        }
    }

    IEnumerator DropAndRise()
    {
        moving = true;

        Vector3 downPos = startPos + Vector3.down * dropDistance;

        // Drop
        while (Vector3.Distance(transform.position, downPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                downPos,
                dropSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Stay down
        yield return new WaitForSeconds(stayDownTime);

        // Rise
        while (Vector3.Distance(transform.position, startPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPos,
                riseSpeed * Time.deltaTime
            );

            yield return null;
        }

        moving = false;
    }
}