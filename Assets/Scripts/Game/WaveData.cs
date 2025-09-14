using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Wave Data", menuName = "Enemies/Wave Data")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public struct SubWave
    {
        public EnemyData enemyData; 

        [Tooltip("The number of enemies to spawn in this group.")]
        public int count;

        [Tooltip("The time between each individual enemy spawn in this group.")]
        public float spawnInterval;

        [Tooltip("The delay before this specific group starts spawning.")]
        public float delayBefore;
    }

    [Header("Wave Configuration")]
    [Tooltip("The sequence of enemy groups that make up this entire wave.")]
    public List<SubWave> subWaves = new List<SubWave>();
}