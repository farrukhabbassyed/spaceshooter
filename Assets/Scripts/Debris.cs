using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Debris : MonoBehaviour
{
    [Header("Drift Settings")]
    [SerializeField] private float tumbleTorque = 15f;

    [Header("Out-of-Bounds Removal")]
    [SerializeField] private float outerRingRadius = 26f;
    [SerializeField] private float timeAllowedOutside = 2f;

    [Header("VFX & Audio")]
    [SerializeField] private GameObject explosionVfxPrefab;

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

        // Apply random spin impulse on spawn
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

        // Despawn out-of-bounds debris without awarding points
        if (distanceToPlayer > outerRingRadius)
        {
            outsideTimer += Time.deltaTime;

            if (outsideTimer >= timeAllowedOutside)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            outsideTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if hit by player projectiles or weapons
        if (other.CompareTag("Bullet") || other.GetComponent<Bullet>() != null)
        {
            DestroyDebris();
            Destroy(other.gameObject); // Destroy the hitting bullet
        }
    }

    // Call this method whenever player attacks destroy this debris
    public void DestroyDebris()
    {
        // Award score points
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddDebrisScore();
        }

        // Spawn explosion VFX if assigned
        if (explosionVfxPrefab != null)
        {
            GameObject explosion = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1.5f);
        }

        Destroy(gameObject);
    }
}