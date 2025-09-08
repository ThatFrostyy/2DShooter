using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Weapons/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Info")]
    public string projectileName;
    public Vector2 scale = Vector2.one;

    [Header("Prefab")]
    [Tooltip("The fully configured projectile prefab.")]
    public GameObject projectilePrefab;

    [Header("Stats")]
    public float speed;
    public float damage;
    public float lifetime;

    [Header("Cosmetic")]
    public GameObject impactEffectPrefab;
    public AudioClip impactSound;
}
