using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Configuration")]
    public WeaponData currentWeapon;
    public int projectilePoolSize = 20;

    [Header("Object References")]
    public Transform firePoint;

    private InputSystem_Actions playerInputActions;
    private float nextFireTime = 0f;
    private bool isFiring = false;

    private void Awake()
    {
        playerInputActions = new InputSystem_Actions();

        // For Semi-Auto: fire once on the initial press
        playerInputActions.Player.Attack.started += ctx => Fire();

        // For Full-Auto: track when the button is held down
        playerInputActions.Player.Attack.performed += ctx => isFiring = true;
        playerInputActions.Player.Attack.canceled += ctx => isFiring = false;
    }

    private void OnEnable() => playerInputActions.Player.Enable();
    private void OnDisable() => playerInputActions.Player.Disable();

    private void Start()
    {
        // Pre-warm the object pool with our weapon's projectile type.
        if (currentWeapon != null && currentWeapon.projectileData != null && currentWeapon.projectileData.projectilePrefab != null)
        {
            ObjectPooler.Instance.PrewarmPool(currentWeapon.projectileData.projectilePrefab, projectilePoolSize);
        }
        else
        {
            Debug.LogError("PlayerShooting has no weapon or projectile configured to pool!", this);
            this.enabled = false;
        }
    }

    private void Update()
    {
        // Full-Auto logic: fires continuously while the button is held
        if (currentWeapon.fireMode == WeaponData.FireMode.FullAuto && isFiring)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            return;
        }

        if (currentWeapon == null || firePoint == null || Time.time < nextFireTime)
        {
            return;
        }

        // Check fire rate cooldown for both modes
        nextFireTime = Time.time + 1f / currentWeapon.fireRate;

        // Get projectile from pooler
        GameObject projectileObject = ObjectPooler.Instance.SpawnFromPool(currentWeapon.projectileData.projectilePrefab, firePoint.position, firePoint.rotation);

        if (projectileObject != null)
        {
            // Get the Projectile component and set its stats based on the weapon's projectile data
            if (projectileObject.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.SetStats(
                    currentWeapon.projectileData.speed,
                    currentWeapon.projectileData.lifetime,
                    currentWeapon.projectileData.damage,
                    currentWeapon.projectileData.impactEffectPrefab,
                    currentWeapon.projectileData.impactSound
                );
            }

            projectileObject.transform.localScale = currentWeapon.projectileData.scale;
        }

        // Use the pooler to efficiently spawn the effect
        ObjectPooler.Instance.SpawnFromPool(currentWeapon.shootEffectPrefab, firePoint.transform.position, Quaternion.identity);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(currentWeapon.shootSound);
        }
    }
}