using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [Header("Target Tracking")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private int maxActiveDebris = 20;
    [SerializeField] private float spawnInterval = 1f;

    [Header("Dynamic Player Ring")]
    [SerializeField] private float innerRadius = 15f;
    [SerializeField] private float outerRadius = 25f;

    private List<GameObject> activeDebrisList = new List<GameObject>();

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        StartCoroutine(ContinuousSpawnRoutine());
    }

    private void Update()
    {
        // Keep spawner position glued to player so the ring moves with the player
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }

    private IEnumerator ContinuousSpawnRoutine()
    {
        while (true)
        {
            activeDebrisList.RemoveAll(item => item == null);

            if (activeDebrisList.Count < maxActiveDebris && playerTransform != null)
            {
                SpawnInRing();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnInRing()
    {
        // Calculate random point inside the moving ring
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float distance = Random.Range(innerRadius, outerRadius);

        Vector3 spawnPos = playerTransform.position + new Vector3(randomDir.x * distance, 0f, randomDir.y * distance);
        spawnPos.y = 0.5f;

        GameObject newDebris = Instantiate(debrisPrefab, spawnPos, Quaternion.identity);

        // Drift slowly across player path
        Rigidbody rb = newDebris.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector2 driftDir = Random.insideUnitCircle.normalized;
            rb.linearVelocity = new Vector3(driftDir.x, 0f, driftDir.y) * Random.Range(2f, 5f);
        }

        activeDebrisList.Add(newDebris);
    }

    private void OnDrawGizmos()
    {
        Vector3 center = playerTransform != null ? playerTransform.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, innerRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, outerRadius);
    }
}