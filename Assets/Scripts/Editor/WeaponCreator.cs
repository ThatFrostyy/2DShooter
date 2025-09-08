using UnityEngine;
using UnityEditor;
using System.IO;

public class WeaponCreator : EditorWindow
{
    private ProjectileData existingProjectileData;

    private string projectileName = "NewProjectile";
    private GameObject projectilePrefab;
    private float projectileSpeed = 25f;
    private float projectileLifetime = 2f;
    private float projectileDamage = 10f;
    private Vector2 projectileScale = Vector2.one;
    private GameObject impactEffectPrefab;
    private AudioClip impactSound;

    private string weaponName = "NewWeapon";
    private WeaponData.FireMode fireMode = WeaponData.FireMode.FullAuto;
    private float fireRate = 10f;
    private GameObject shootEffectPrefab;
    private AudioClip shootSound;


    [MenuItem("Tools/Weapon and Projectile Creator")]
    public static void ShowWindow()
    {
        GetWindow<WeaponCreator>("Weapon and Projectile Creator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("1. Choose or Create a Projectile", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        existingProjectileData = (ProjectileData)EditorGUILayout.ObjectField("Existing Projectile", existingProjectileData, typeof(ProjectileData), false);

        // If no existing projectile is assigned, show the fields to create a new one.
        if (existingProjectileData == null)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("--- OR Create New Projectile ---", EditorStyles.centeredGreyMiniLabel);
            projectileName = EditorGUILayout.TextField("Projectile Name", projectileName);
            projectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", projectilePrefab, typeof(GameObject), false);
            projectileSpeed = EditorGUILayout.FloatField("Projectile Speed", projectileSpeed);
            projectileLifetime = EditorGUILayout.FloatField("Projectile Lifetime", projectileLifetime);
            projectileDamage = EditorGUILayout.FloatField("Projectile Damage", projectileDamage);
            projectileScale = EditorGUILayout.Vector2Field("Projectile Scale", projectileScale);
            impactEffectPrefab = (GameObject)EditorGUILayout.ObjectField("Impact Effect (Prefab)", impactEffectPrefab, typeof(GameObject), false);
            impactSound = (AudioClip)EditorGUILayout.ObjectField("Impact Sound", impactSound, typeof(AudioClip), false);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(15);


        EditorGUILayout.LabelField("2. Design the Weapon", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        weaponName = EditorGUILayout.TextField("Weapon Name", weaponName);
        fireMode = (WeaponData.FireMode)EditorGUILayout.EnumPopup("Fire Mode", fireMode);
        fireRate = EditorGUILayout.FloatField("Fire Rate (projectiles per sec)", fireRate);
        shootEffectPrefab = (GameObject)EditorGUILayout.ObjectField("Impact Effect (Prefab)", shootEffectPrefab, typeof(GameObject), false);
        shootSound = (AudioClip)EditorGUILayout.ObjectField("Impact Sound", shootSound, typeof(AudioClip), false);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(20);

        if (GUILayout.Button("Create Weapon Asset"))
        {
            CreateWeaponAsset();
        }
    }

    private void CreateWeaponAsset()
    {
        ProjectileData projectileToLink;

        if (existingProjectileData != null)
        {
            // Use the assigned existing projectile data
            projectileToLink = existingProjectileData;
        }
        else
        {
            ProjectileData newProjectileData = ScriptableObject.CreateInstance<ProjectileData>();
            newProjectileData.projectileName = projectileName;
            newProjectileData.projectilePrefab = projectilePrefab;
            newProjectileData.speed = projectileSpeed;
            newProjectileData.lifetime = projectileLifetime;
            newProjectileData.damage = projectileDamage;
            newProjectileData.scale = projectileScale;
            newProjectileData.impactEffectPrefab = impactEffectPrefab;
            newProjectileData.impactSound = impactSound;

            string projectilePath = "Assets/Projectiles";
            if (!Directory.Exists(projectilePath))
            {
                Directory.CreateDirectory(projectilePath);
            }
            AssetDatabase.CreateAsset(newProjectileData, $"{projectilePath}/{projectileName}.asset");
            projectileToLink = newProjectileData;
        }

        // Now create the weapon and link the chosen projectile
        WeaponData newWeapon = ScriptableObject.CreateInstance<WeaponData>();
        newWeapon.weaponName = weaponName;
        newWeapon.fireMode = fireMode;
        newWeapon.fireRate = fireRate;
        newWeapon.shootEffectPrefab = shootEffectPrefab;
        newWeapon.shootSound = shootSound;
        newWeapon.projectileData = projectileToLink;

        string weaponPath = "Assets/Weapons";
        if (!Directory.Exists(weaponPath))
        {
            Directory.CreateDirectory(weaponPath);
        }
        AssetDatabase.CreateAsset(newWeapon, $"{weaponPath}/{weaponName}.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newWeapon;
        Debug.Log($"Successfully created {weaponName}.asset linked to {projectileToLink.name}.asset!");
    }
}

