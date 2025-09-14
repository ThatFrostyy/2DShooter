using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    private float speed;
    private float lifetime;
    private float damage; 
    private float lifetimeTimer;

    private GameObject impactEffectPrefab;
    private AudioClip impactSound;

    private void OnEnable()
    {
        lifetimeTimer = 0f;
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetStats(float newSpeed, float newLifetime, float newDamage, GameObject newImpactEffectPrefab, AudioClip newImpactSound)
    {
        speed = newSpeed;
        lifetime = newLifetime;
        damage = newDamage;
        impactEffectPrefab = newImpactEffectPrefab;
        impactSound = newImpactSound;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(damage);
            }
        }

        HandleImpact();

        ObjectPooler.Instance.ReturnToPool(gameObject);
    }

    private void HandleImpact()
    {
        if (impactEffectPrefab != null)
        {
            // Use the pooler to efficiently spawn the effect
            ObjectPooler.Instance.SpawnFromPool(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.Instance != null && impactSound != null)
        {
            AudioManager.Instance.PlaySfx(impactSound);
        }
    }
}

