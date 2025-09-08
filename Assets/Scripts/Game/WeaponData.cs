using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public enum FireMode { FullAuto, SemiAuto }

    [Header("Info")]
    public string weaponName;
    public FireMode fireMode;

    [Header("Stats")]
    [Tooltip("Projectiles per second.")]
    public float fireRate;

    [Header("Cosmetic")]
    public GameObject shootEffectPrefab;
    public AudioClip shootSound;

    [Header("Projectile")]
    public ProjectileData projectileData; // Direct reference to projectile stats
}

