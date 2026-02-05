using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerRunStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Current Run Data")]
    public float currentHealth;
    public int currentLevel;
    public int currentExperience;

    // We store the weapon name and its level
    [System.Serializable]
    public struct SavedWeapon
    {
        public string weaponName;
        public int level;
    }

    public List<SavedWeapon> savedWeapons = new List<SavedWeapon>();

    public void ResetStats(GlobalStats global)
    {
        float baseHealth = 30f;
        currentHealth = baseHealth + global.bonusMaxHealth;

        currentLevel = 1;
        currentExperience = 0;

        // CRITICAL: Clear the list so the next run starts with 0 weapons
        savedWeapons.Clear();
    }
}