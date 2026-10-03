using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChaser : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Despawn Settings")]
    [SerializeField] private float maxDistanceFromPlayer = 45f;

    [Header("VFX & Audio")]
    [SerializeField] private GameObject explosionVfxPrefab;
    [SerializeField] private AudioClip destructionSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    private Rigidbody rb;
    private Transform playerTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        // Smoothly rotate toward player on X/Z plane
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Drive forward
        rb.linearVelocity = transform.forward * moveSpeed; // Use rb.velocity on Unity 2022 or older
    }

    private void Update()
    {
        // Auto-cleanup if the player leaves the enemy far behind
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) > maxDistanceFromPlayer)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroyed when hit by player projectile bullets
        if (other.CompareTag("Bullet") || other.GetComponent<Bullet>() != null)
        {
            DestroyEnemy();
            Destroy(other.gameObject);
        }
        // Note: Player collision damage is handled by PlayerHealth on the player ship
    }

    public void DestroyEnemy()
    {
        // Award points when the player destroys this enemy
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddEnemyScore();
        }

        // Spawn explosion VFX if assigned
        if (explosionVfxPrefab != null)
        {
            GameObject explosion = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1.5f);
        }

        // Play sound independently on a dedicated temporary AudioSource
        PlayIndependentSound();

        Destroy(gameObject);
    }

    private void PlayIndependentSound()
    {
        if (destructionSound == null) return;

        // Create a standalone GameObject for this specific enemy sound instance
        GameObject soundObj = new GameObject("EnemyExplosionAudio");

        // Attach and configure AudioSource
        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = destructionSound;
        source.volume = soundVolume;
        source.spatialBlend = 0f; // 2D sound for crisp arcade feedback
        source.pitch = Random.Range(0.88f, 1.12f); // Slightly wider pitch variance for heavier enemy impacts
        source.Play();

        // Destroy the audio GameObject automatically after clip completes
        Destroy(soundObj, destructionSound.length);
    }
}