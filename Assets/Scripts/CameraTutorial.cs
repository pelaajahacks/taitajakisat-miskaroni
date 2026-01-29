using UnityEngine;

public class CameraTutorial : MonoBehaviour
{
    public float speed = 10f;
    public float edgeSize = 40f; // Pixels from screen edge

    void Update()
    {
        Vector3 dir = Vector3.zero;
        Vector3 mouse = Input.mousePosition;

        // Horizontal
        if (mouse.x < edgeSize)
            dir.x = -1;
        else if (mouse.x > Screen.width - edgeSize)
            dir.x = 1;

        // Vertical
        if (mouse.y < edgeSize)
            dir.z = -1;
        else if (mouse.y > Screen.height - edgeSize)
            dir.z = 1;

        // Normalize = perfect 45° diagonals
        dir = dir.normalized;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }
}
