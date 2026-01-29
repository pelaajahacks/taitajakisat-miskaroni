using UnityEngine;

public class RisingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public float riseHeight = 3f;
    public float riseSpeed = 2f;

    Vector3 startPos;
    Vector3 targetPos;

    bool activated = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * riseHeight;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only react to player
        if (!collision.gameObject.CompareTag("Player"))
            return;

        activated = true;
    }

    void Update()
    {
        if (!activated)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            riseSpeed * Time.deltaTime
        );
    }
}
