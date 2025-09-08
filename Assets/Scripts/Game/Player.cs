using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth = 100;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (health != null)
        {
            health.Initialize(maxHealth);
        }
    }
}
