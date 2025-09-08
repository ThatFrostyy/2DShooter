using UnityEngine;

[RequireComponent(typeof(Health))] 
public class PlayerCollisionHandler : MonoBehaviour
{
    private Health playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                float damageToTake;
                if (enemy.enemyData != null)
                {
                    damageToTake = enemy.enemyData.damage;
                }
                else
                {
                    Debug.LogWarning("Collided with an enemy that is missing its EnemyData!", enemy.gameObject);
                    damageToTake = 10; // Default damage as a fallback
                }

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToTake);
                }

                if (collision.gameObject.TryGetComponent<Health>(out Health enemyHealth))
                {
                    enemyHealth.TakeDamage(999);
                }
            }
        }  
    }
}
