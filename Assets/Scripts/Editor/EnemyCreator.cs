using UnityEngine;
using UnityEditor;
using System.IO;

public class EnemyCreator : EditorWindow
{
    private string enemyName = "NewEnemy";
    private GameObject enemyPrefab;
    private float lifetime;
    private float maxHealth = 100f;
    private float moveSpeed = 3f;
    private float attackRange = 3f;
    private float attackRate = 1f;
    private float damage = 10f;
    private int scoreValue = 10;

    [MenuItem("Tools/Enemy Creator")]
    public static void ShowWindow()
    {
        GetWindow<EnemyCreator>("Enemy Creator");
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("This tool creates an EnemyData asset. You must create the enemy's prefab first.", MessageType.Info);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");
        enemyName = EditorGUILayout.TextField("Data Asset Name", enemyName);
        enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab", enemyPrefab, typeof(GameObject), false);
        lifetime = EditorGUILayout.FloatField("Life Time", lifetime);
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        maxHealth = EditorGUILayout.FloatField("Max Health", maxHealth);
        moveSpeed = EditorGUILayout.FloatField("Move Speed", moveSpeed);
        attackRange = EditorGUILayout.FloatField("Attack Range", attackRange);
        attackRate = EditorGUILayout.FloatField("Attack Rate (attacks per sec)", attackRate);
        damage = EditorGUILayout.FloatField("Damage", damage);
        scoreValue = EditorGUILayout.IntField("Score Value", scoreValue);
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(20);

        if (GUILayout.Button("Create Enemy Data"))
        {
            CreateEnemyDataAsset();
        }
    }

    private void CreateEnemyDataAsset()
    {
        if (enemyPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "You must assign an Enemy Prefab.", "OK");
            return;
        }

        EnemyData newEnemyData = ScriptableObject.CreateInstance<EnemyData>();
        newEnemyData.enemyName = enemyName;
        newEnemyData.enemyPrefab = enemyPrefab;
        newEnemyData.lifetime = lifetime;
        newEnemyData.maxHealth = maxHealth;
        newEnemyData.moveSpeed = moveSpeed;
        newEnemyData.attackRange = attackRange;
        newEnemyData.attackRate = attackRate;
        newEnemyData.damage = damage;
        newEnemyData.scoreValue = scoreValue;

        string enemyPath = "Assets/Enemies";
        if (!Directory.Exists(enemyPath))
        {
            Directory.CreateDirectory(enemyPath);
        }
        AssetDatabase.CreateAsset(newEnemyData, $"{enemyPath}/{enemyName}.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newEnemyData;
        Debug.Log($"Successfully created {enemyName}.asset!");
    }
}