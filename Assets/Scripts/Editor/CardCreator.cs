using UnityEngine;
using UnityEditor;
using System.IO;

public class CardCreator : EditorWindow
{
    private string cardName = "NewCard";
    private string cardDescription = "Describe the card's effect here.";
    private Sprite cardIcon;
    private CardData.UpgradeType upgradeType;
    private float upgradeValue;
    private WeaponData weaponData; 

    private Vector2 scrollPosition;

    [MenuItem("Tools/Card Creator")]
    public static void ShowWindow()
    {
        GetWindow<CardCreator>("Card Creator");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("1. Card Display Info", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        cardName = EditorGUILayout.TextField("Card Name", cardName);
        cardIcon = (Sprite)EditorGUILayout.ObjectField("Card Icon", cardIcon, typeof(Sprite), false);

        EditorGUILayout.LabelField("Card Description");
        cardDescription = EditorGUILayout.TextArea(cardDescription, GUILayout.Height(60));

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(15);

        EditorGUILayout.LabelField("2. Card Gameplay Effect", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        upgradeType = (CardData.UpgradeType)EditorGUILayout.EnumPopup("Upgrade Type", upgradeType);

        // --- Conditional Fields: Show only the relevant field based on the selected upgrade type ---
        switch (upgradeType)
        {
            case CardData.UpgradeType.ChangeWeapon:
                // If we're changing the weapon, we need to know which weapon to give.
                weaponData = (WeaponData)EditorGUILayout.ObjectField("Weapon To Grant", weaponData, typeof(WeaponData), false);
                break;
            default:
                // For all other types (speed, health, fire rate), we just need a number.
                upgradeValue = EditorGUILayout.FloatField("Upgrade Value", upgradeValue);
                break;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(20);

        if (GUILayout.Button("Create Card Asset", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(cardName))
            {
                EditorUtility.DisplayDialog("Error", "Please provide a name for the card.", "OK");
            }
            else
            {
                CreateCardAsset();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateCardAsset()
    {
        CardData newCard = ScriptableObject.CreateInstance<CardData>();

        newCard.cardName = this.cardName;
        newCard.cardDescription = this.cardDescription;
        newCard.cardIcon = this.cardIcon;
        newCard.upgradeType = this.upgradeType;
        newCard.upgradeValue = this.upgradeValue;
        newCard.weaponData = this.weaponData;

        string cardPath = "Assets/Cards";
        if (!Directory.Exists(cardPath))
        {
            Directory.CreateDirectory(cardPath);
        }

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{cardPath}/{cardName}.asset");
        AssetDatabase.CreateAsset(newCard, uniquePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newCard;

        Debug.Log($"Successfully created {cardName}.asset at {uniquePath}");
    }
}