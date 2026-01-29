using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSystem : MonoBehaviour
{
    [Header("Player")]
    public Transform player;  // assign your player
    public Rigidbody playerRb;

    [Header("Checkpoint Data")]
    private Vector3 latestCheckpointPos;
    private float timeSinceLevelStart;

    [Header("Hazards")]
    public List<GameObject> hazards; // optional, assign hazard objects that need resetting
    private Dictionary<GameObject, Vector3> hazardStartPositions = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        // Initialize first checkpoint at player start
        latestCheckpointPos = player.position;
        timeSinceLevelStart = 0f;

        // Save initial hazard positions
        foreach (var h in hazards)
        {
            hazardStartPositions[h] = h.transform.position;
        }
    }

    private void Update()
    {
        // Keep track of level timer
        timeSinceLevelStart += Time.deltaTime;
    }

    // Call this when player hits a checkpoint trigger
    public void SetCheckpoint(Vector3 position)
    {
        latestCheckpointPos = position;
        Debug.Log("Checkpoint saved at: " + position);
    }

    // Call this when player hits a death wall
    public void RespawnAtCheckpoint()
    {
        // Reset player
        player.position = latestCheckpointPos;
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        // Reset hazards
        foreach (var h in hazards)
        {
            h.transform.position = hazardStartPositions[h];
        }

        // Optional: reset level timer
        // timeSinceLevelStart = 0f;

        Debug.Log("Player respawned at checkpoint!");
    }
}