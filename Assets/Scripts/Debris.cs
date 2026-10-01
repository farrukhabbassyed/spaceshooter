using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Debris : MonoBehaviour
{
    [Header("Drift Settings")]
    [SerializeField] private float tumbleTorque = 15f;

    [Header("Out-of-Bounds Removal")]
    [SerializeField] private float outerRingRadius = 26f; // Must match or slightly exceed Spawner outerRadius
    [SerializeField] private float timeAllowedOutside = 2f;

    private Rigidbody rb;
    private Transform playerTransform;
    private float outsideTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Random spin
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * tumbleTorque;

        rb.AddTorque(randomTorque, ForceMode.Impulse);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // If debris is outside the ring radius
        if (distanceToPlayer > outerRingRadius)
        {
            outsideTimer += Time.deltaTime;

            // Destroy if left outside the ring for 2 seconds continuous
            if (outsideTimer >= timeAllowedOutside)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Reset timer if it re-enters the ring boundary
            outsideTimer = 0f;
        }
    }
}