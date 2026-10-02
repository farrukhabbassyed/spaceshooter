using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Damage Values")]
    [SerializeField] private int debrisDamage = 15;
    [SerializeField] private int chaserDamage = 25;

    [Header("VFX & Audio")]
    [SerializeField] private GameObject explosionVfxPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Debris"))
        {
            TakeDamage(debrisDamage);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            TakeDamage(chaserDamage);

            // Trigger enemy destruction effect if present
            EnemyChaser enemy = other.GetComponent<EnemyChaser>();
            if (enemy != null)
            {
                enemy.DestroyEnemy();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage! Remaining Health: {currentHealth}/{maxHealth}");

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
{
    // Stop the survival timer immediately
    if (GameTimer.Instance != null)
    {
        GameTimer.Instance.StopTimer();
    }

    // Play death VFX & Audio
    if (explosionVfxPrefab != null)
    {
        GameObject explosion = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);
    }

    if (deathSound != null && audioSource != null)
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
    }

    // Hide player ship
    gameObject.SetActive(false);

    // Trigger Game Over UI and activate Depth of Field background blur
    if (GameOverManager.Instance != null)
    {
        GameOverManager.Instance.TriggerGameOver();
    }
}

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}