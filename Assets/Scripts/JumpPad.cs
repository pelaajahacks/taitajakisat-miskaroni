using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Launch Settings")]
    public Vector3 launchDirection = Vector3.up; // default straight up
    public float launchForce = 15f;              // adjust how high / far

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Reset vertical velocity before launch
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                // Apply launch force
                rb.AddForce(launchDirection.normalized * launchForce, ForceMode.VelocityChange);
            }
        }
    }
}