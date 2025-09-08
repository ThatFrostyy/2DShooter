using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health Configuration")]
    [Tooltip("Color to flash when taking damage")]
    public Color flashColor = Color.red;

    [Header("Shake Settings")]
    [Tooltip("Check this if this Health component belongs to the player.")]
    public bool isPlayerHealth = false;
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.1f;

    private float maxHealth;
    private float currentHealth;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;
    private Enemy enemyComponent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        enemyComponent = GetComponent<Enemy>();
    }

    public void Initialize(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth;

        if (isPlayerHealth && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        // Forcefully stop any running coroutines from the object's previous life.
        StopAllCoroutines();

        // Reset the color and the flashing state flag.
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        isFlashing = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (isPlayerHealth && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        if (isPlayerHealth && CameraShake.Instance != null)
        {
            CameraShake.Instance.StartShake(shakeDuration, shakeMagnitude);
        }

        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        isFlashing = true;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    private void Die()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            if (enemyComponent != null && enemyComponent.enemyData != null && GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(enemyComponent.enemyData.scoreValue);
            }
        }

        gameObject.SetActive(false);
    }
}
