/*using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Physics")]
    [SerializeField] private float speed = 35f;
    [SerializeField] private float lifeTime = 3f;

    [Header("Impact Audio & VFX")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debris debrisComponent = other.GetComponent<Debris>();

        if (debrisComponent != null)
        {
            // Spawn Hit Visual Effect at point of impact
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 1.5f); // Auto-cleanup hit effect particle
            }

            // Play Hit Audio Effect at impact position
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }

            Destroy(other.gameObject); // Destroy hit debris
            Destroy(gameObject);        // Destroy bullet
        }
    }
} */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Physics")]
    [SerializeField] private float speed = 35f;
    [SerializeField] private float lifeTime = 3f;

    [Header("Impact Audio & VFX")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
{
    // 1. Ignore collision if bullet hits the Player ship (or any of its child meshes)
    if (other.CompareTag("Player") || other.GetComponentInParent<PlayerHealth>() != null)
    {
        return;
    }

    // 2. Check for Debris Hit
    Debris debrisComponent = other.GetComponentInParent<Debris>();
    if (debrisComponent != null)
    {
        PlayImpactEffects();
        Destroy(debrisComponent.gameObject);
        Destroy(gameObject);
        return;
    }

    // 3. Check for Enemy Hit
    EnemyChaser enemyComponent = other.GetComponentInParent<EnemyChaser>();
    if (enemyComponent != null || other.CompareTag("Enemy"))
    {
        PlayImpactEffects();

        if (enemyComponent != null)
        {
            enemyComponent.DestroyEnemy();
        }
        else
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }
}
    private void PlayImpactEffects()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1.5f);
        }

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
    }
}