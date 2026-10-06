/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Target Tracking")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject chaserEnemyPrefab;
    [SerializeField] private int maxActiveEnemies = 8;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Perimeter Spawn Ring")]
    [SerializeField] private float innerRadius = 18f;
    [SerializeField] private float outerRadius = 26f;

    private List<GameObject> activeEnemyList = new List<GameObject>();

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        StartCoroutine(ContinuousSpawnRoutine());
    }

    private IEnumerator ContinuousSpawnRoutine()
    {
        while (true)
        {
            activeEnemyList.RemoveAll(item => item == null);

            if (activeEnemyList.Count < maxActiveEnemies && playerTransform != null)
            {
                SpawnChaserOffscreen();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnChaserOffscreen()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float distance = Random.Range(innerRadius, outerRadius);

        Vector3 spawnPos = playerTransform.position + new Vector3(randomDir.x * distance, 0f, randomDir.y * distance);
        spawnPos.y = 0.5f;

        GameObject newEnemy = Instantiate(chaserEnemyPrefab, spawnPos, Quaternion.identity);
        activeEnemyList.Add(newEnemy);
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Target Tracking")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject chaserEnemyPrefab;
    [SerializeField] private int maxActiveEnemies = 8;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Dynamic Player Ring")]
    [SerializeField] private float innerRadius = 18f;
    [SerializeField] private float outerRadius = 26f;

    private List<GameObject> activeEnemyList = new List<GameObject>();

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
            activeEnemyList.RemoveAll(item => item == null);

            if (activeEnemyList.Count < maxActiveEnemies && playerTransform != null)
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

        GameObject newEnemy = Instantiate(chaserEnemyPrefab, spawnPos, Quaternion.identity);

        activeEnemyList.Add(newEnemy);
    }

    private void OnDrawGizmos()
    {
        Vector3 center = playerTransform != null ? playerTransform.position : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, innerRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(center, outerRadius);
    }
}