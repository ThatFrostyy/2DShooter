using UnityEngine;
using UnityEditor;
using UnityEditorInternal; 
using System.IO;
using System.Collections.Generic;

public class WaveCreator : EditorWindow
{
    private string waveName = "NewWave";
    private List<WaveData.SubWave> subWaves = new();
    private ReorderableList reorderableList;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Wave Creator")]
    public static void ShowWindow()
    {
        GetWindow<WaveCreator>("Wave Creator");
    }

    private void OnEnable()
    {
        reorderableList = new ReorderableList(subWaves, typeof(WaveData.SubWave),
            true, true, true, true);

        // Define how the header of the list is drawn
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Sub-Waves (Enemy Groups)");
        };

        // Define how each element in the list is drawn
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = subWaves[index];
            rect.y += 2; // Add a little padding

            // Calculate rects for each property field
            Rect enemyDataRect = new(rect.x, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);
            Rect countRect = new(rect.x + rect.width * 0.42f, rect.y, rect.width * 0.15f, EditorGUIUtility.singleLineHeight);
            Rect intervalRect = new(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.18f, EditorGUIUtility.singleLineHeight);
            Rect delayRect = new(rect.x + rect.width * 0.81f, rect.y, rect.width * 0.18f, EditorGUIUtility.singleLineHeight);

            // Draw the fields
            element.enemyData = (EnemyData)EditorGUI.ObjectField(enemyDataRect, element.enemyData, typeof(EnemyData), false);
            element.count = EditorGUI.IntField(countRect, element.count);
            element.spawnInterval = EditorGUI.FloatField(intervalRect, element.spawnInterval);
            element.delayBefore = EditorGUI.FloatField(delayRect, element.delayBefore);

            subWaves[index] = element; 
        };
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("1. Name Your Wave", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        waveName = EditorGUILayout.TextField("Wave Name", waveName);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(15);

        EditorGUILayout.LabelField("2. Design the Sub-Waves", EditorStyles.boldLabel);

        // Use a tooltip to explain the columns
        EditorGUILayout.LabelField("Enemy Data | Count | Spawn Interval (s) | Wave Delay (s)", EditorStyles.miniLabel);

        reorderableList.DoLayoutList();

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Create Wave Asset", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(waveName) || subWaves.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "Please provide a wave name and at least one sub-wave.", "OK");
            }
            else
            {
                CreateWaveAsset();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateWaveAsset()
    {
        WaveData newWave = ScriptableObject.CreateInstance<WaveData>();

        newWave.subWaves = new List<WaveData.SubWave>(this.subWaves);

        string wavePath = "Assets/Waves"; 
        if (!Directory.Exists(wavePath))
        {
            Directory.CreateDirectory(wavePath);
        }

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{wavePath}/{waveName}.asset");
        AssetDatabase.CreateAsset(newWave, uniquePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newWave;

        Debug.Log($"Successfully created {waveName}.asset at {uniquePath}");
    }
}