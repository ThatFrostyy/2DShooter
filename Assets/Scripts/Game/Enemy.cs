using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(IEnemyBehavior))]
public class Enemy : MonoBehaviour
{
    public EnemyData enemyData { get; private set; } // Can be read publicly, but only set internally

    private Health health;
    private IEnemyBehavior behavior;

    private void Awake()
    {
        health = GetComponent<Health>();
        behavior = GetComponent<IEnemyBehavior>();
    }
    public void Initialize(EnemyData data)
    {
        this.enemyData = data;

        // Now, use the data to initialize all other necessary components
        // on this GameObject. This script acts as the central manager.
        if (health != null)
        {
            health.Initialize(enemyData.maxHealth);
        }
        else
        {
            Debug.LogError($"Enemy '{gameObject.name}' is missing a Health component!", this);
        }


        if (behavior != null)
        {
            behavior.Initialize(enemyData);
        }
        else
        {
            Debug.LogError($"Enemy '{gameObject.name}' is missing a script that implements IEnemyBehavior!", this);
        }
    }
}

