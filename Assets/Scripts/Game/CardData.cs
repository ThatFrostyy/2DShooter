using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    public enum UpgradeType
    {
        // Player Stats
        IncreaseMoveSpeed,
        IncreaseMaxHealth,

        // Weapon Stats
        IncreaseFireRate,
        ChangeWeapon
    }

    [Header("Card Info")]
    public string cardName;
    [TextArea(3, 5)]
    public string cardDescription;
    public Sprite cardIcon;

    [Header("Card Effect")]
    public UpgradeType upgradeType;

    [Tooltip("The value of the upgrade (e.g., +5 for speed, +10 for health). Not used for 'ChangeWeapon'.")]
    public float upgradeValue;

    [Tooltip("The new weapon to grant. Only used if the type is 'ChangeWeapon'.")]
    public WeaponData weaponData; 
}