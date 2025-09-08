using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;

    [Header("Prefab")]
    [Tooltip("The fully configured enemy prefab to spawn.")]
    public GameObject enemyPrefab;
    public float lifetime;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float attackRange = 3f;
    [Tooltip("Attacks per second.")]
    public float attackRate = 1f;
    public float damage = 10f;
    public int scoreValue = 10;
}
