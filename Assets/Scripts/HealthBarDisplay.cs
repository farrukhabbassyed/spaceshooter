using UnityEngine;
using UnityEngine.UI;

public class HealthBarDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject fillObject;

    private void Start()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (fillObject == null && healthSlider != null && healthSlider.fillRect != null)
        {
            fillObject = healthSlider.fillRect.gameObject;
        }

        UpdateHealthBarInstant();
    }

    private void Update()
    {
        UpdateHealthBarInstant();
    }

    private void UpdateHealthBarInstant()
    {
        if (playerHealth == null || healthSlider == null) return;

        float maxHP = playerHealth.GetMaxHealth();
        float currentHP = Mathf.Max(0, playerHealth.GetCurrentHealth());

        // Ensure bounds are synced exactly
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;

        // Instantly hide the fill graphics at 0 HP to avoid single-pixel leaks
        if (fillObject != null)
        {
            fillObject.SetActive(currentHP > 0);
        }
    }
}