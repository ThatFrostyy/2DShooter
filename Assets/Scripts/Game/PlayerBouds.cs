using UnityEngine;

// This script ensures that the GameObject it is attached to stays within the camera's view.
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerBounds : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    private float minX, maxX, minY, maxY;
    private float spriteWidthExtent, spriteHeightExtent;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found. Please ensure your camera is tagged 'MainCamera'.", this);
            enabled = false; 
            return;
        }

        CalculateBounds();
    }

    void LateUpdate()
    {
        // LateUpdate is used because it runs after all physics and movement calculations in Update/FixedUpdate.
        // This ensures our clamping is the very last thing to happen each frame.

        Vector3 currentPosition = transform.position;

        // Clamp the player's position to be within the calculated min/max values.
        // We add/subtract the sprite extents to ensure the *entire* sprite stays on screen.
        float clampedX = Mathf.Clamp(currentPosition.x, minX + spriteWidthExtent, maxX - spriteWidthExtent);
        float clampedY = Mathf.Clamp(currentPosition.y, minY + spriteHeightExtent, maxY - spriteHeightExtent);

        transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
    }

    private void CalculateBounds()
    {
        // Get the size of the sprite to offset the bounds
        // This prevents half the player from going off-screen
        var spriteBounds = spriteRenderer.bounds;
        spriteWidthExtent = spriteBounds.extents.x;
        spriteHeightExtent = spriteBounds.extents.y;

        // Convert the camera's viewport corners (0,0 and 1,1) to world coordinates
        Vector3 lowerLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = lowerLeft.x;
        maxX = upperRight.x;
        minY = lowerLeft.y;
        maxY = upperRight.y;
    }
}
