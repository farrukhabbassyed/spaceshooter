using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Debris : MonoBehaviour
{
[Header("Drift Settings")]
[SerializeField] private float tumbleTorque = 15f;

[Header("Out-of-Bounds Removal")]
[SerializeField] private float outerRingRadius = 26f;
[SerializeField] private float timeAllowedOutside = 2f;

[Header("Shatter Mechanics")]
[SerializeField] private GameObject shatteredDebrisPrefab;
[SerializeField] private float explosionForce = 350f;
[SerializeField] private float explosionRadius = 2.5f;
[SerializeField] private float piecesLifetime = 3f;

[Header("VFX & Audio")]
[SerializeField] private GameObject explosionVfxPrefab;
[SerializeField] private AudioClip destructionSound;
[Range(0f, 1f)]
[SerializeField] private float soundVolume = 1f;

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

// Despawn out-of-bounds debris without awarding points or playing sound
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

// 1. Spawn and shatter broken pieces
if (shatteredDebrisPrefab != null)
{
GameObject shatteredInstance = Instantiate(shatteredDebrisPrefab, transform.position, transform.rotation);

// Apply outward physics impulse to all child fragment Rigidbodies
Rigidbody[] pieces = shatteredInstance.GetComponentsInChildren<Rigidbody>();
foreach (Rigidbody pieceRb in pieces)
{
pieceRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
}

// Cleanup fragments after a set time
Destroy(shatteredInstance, piecesLifetime);
}

// 2. Spawn explosion VFX if assigned
if (explosionVfxPrefab != null)
{
GameObject explosion = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
Destroy(explosion, 1.5f);
}

// 3. Play sound independently on a dedicated temporary AudioSource
PlayIndependentSound();

// 4. Destroy main debris object
Destroy(gameObject);
}

private void PlayIndependentSound()
{
if (destructionSound == null) return;

// Create a standalone GameObject for this specific sound instance
GameObject soundObj = new GameObject("DebrisExplosionAudio");

// Attach and configure AudioSource
AudioSource source = soundObj.AddComponent<AudioSource>();
source.clip = destructionSound;
source.volume = soundVolume;
source.spatialBlend = 0f; // 2D sound for crisp UI/arcade feedback
source.pitch = Random.Range(0.9f, 1.1f); // Prevents repetitive harshness when destroyed in groups
source.Play();

// Destroy the audio GameObject automatically after clip completes
Destroy(soundObj, destructionSound.length);
}
}