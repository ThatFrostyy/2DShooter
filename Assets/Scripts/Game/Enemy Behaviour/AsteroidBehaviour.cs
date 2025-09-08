using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AsteroidBehavior : MonoBehaviour, IEnemyBehavior
{
    private Rigidbody2D rb;
    private Transform playerTransform;
    private float lifetime;
    private float moveSpeed;
    private float rotationSpeed;
    private Vector2 moveDirection;

    private float lifetimeTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(EnemyData data)
    {
        this.moveSpeed = data.moveSpeed;
        this.lifetime = data.lifetime;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform != null)
        {
            // Aim at a random point in a 5-unit radius circle around the player
            Vector3 targetPoint = playerTransform.position + (Vector3)Random.insideUnitCircle * 5f;
            moveDirection = (targetPoint - transform.position).normalized;
        }
        else 
        {
            // Aim towards the center of the screen (0,0,0)
            moveDirection = (Vector3.zero - transform.position).normalized;
        }

        rotationSpeed = Random.Range(-50f, 50f);
    }

    private void Update()
    {
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
            rb.rotation += rotationSpeed * Time.fixedDeltaTime;
        }
    }
}