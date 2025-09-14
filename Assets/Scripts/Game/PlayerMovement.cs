using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Configuration")]
    [Tooltip("The force applied when moving forward.")]
    public float accelerationForce = 10f;
    [Tooltip("The maximum speed the ship can reach.")]
    public float maxSpeed = 12f;
    [Tooltip("The drag/friction that slows the ship down. Higher values mean faster stops.")]
    public float linearDrag = 0.8f;

    [Header("Rotation Configuration")]
    [Tooltip("How quickly the ship turns to face the mouse cursor.")]
    public float rotationSpeed = 5f;
    [Tooltip("The visual offset to correct the sprite's forward direction. For the default triangle, this should be -90.")]
    public float rotationOffset = -90f;

    private Rigidbody2D rb;
    private InputSystem_Actions playerInputActions;
    private Camera mainCamera;

    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        rb.linearDamping = linearDrag;

        if (mainCamera == null)
        {
            Debug.LogError("ERROR: Main Camera not found. Make sure your camera is tagged 'MainCamera'.");
        }

        playerInputActions = new InputSystem_Actions();
        playerInputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            rb.linearVelocity = Vector2.zero; 
            return;
        }

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        rb.AddForce(accelerationForce * moveInput.y * transform.up);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        }
    }

    private void HandleRotation()
    {
        if (mainCamera == null) return;

        Vector2 mouseScreenPosition = playerInputActions.Player.Look.ReadValue<Vector2>();
        Vector3 mouseScreenPosWithZ = new(mouseScreenPosition.x, mouseScreenPosition.y, -mainCamera.transform.position.z);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosWithZ);

        Vector2 directionToMouse = (Vector2)mouseWorldPosition - rb.position;
        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        float newAngle = Mathf.LerpAngle(rb.rotation, targetAngle + rotationOffset, rotationSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(newAngle);
    }

    #region Upgrades
    public void IncreaseSpeed(float amount)
    {
        maxSpeed += amount;
    }
    #endregion Upgrades

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && rb != null)
        {
            // Display the current speed as a label above the player
#if UNITY_EDITOR
            float lineHeight = 0.3f; // spacing between lines
            Vector3 basePos = transform.position + Vector3.up * 2f;

            // Speed
            UnityEditor.Handles.Label(basePos, $"Speed: {rb.linearVelocity.magnitude:F2}");

            // Position
            UnityEditor.Handles.Label(basePos + lineHeight * Vector3.down, $"Position: {rb.position}");
#endif
        }
    }
}