using UnityEngine;

[CreateAssetMenu(fileName = "GlobalStats", menuName = "PokemonSurvivor/Global Stats")]
public class GlobalStats : ScriptableObject
{
    [Header("Basic Upgrades")]
    public float bonusDamage;      // %
    public float bonusMoveSpeed;   // %
    public float bonusMaxHealth;   // Flat
    public float healOnLevelUp;    // Flat (the +5 HP you mentioned)

    [Header("Special Upgrades")]
    public float bonusCoinChance;  // %
    public int bonusProjectiles;   // Max 3
    public float bonusArea;        // % Max 50%
    public float bonusCooldown;    // % Max 30% (Faster)


    void Start()
    {
        ShopManager shop = FindFirstObjectByType<ShopManager>();
        if (shop != null)
        {
            string[] keys = { "Damage", "Speed", "Health", "Heal", "Coins", "Projectiles", "Area", "Cooldown" };
            foreach (string k in keys)
            {
                int lv = PlayerPrefs.GetInt(k, 0);
            }
        }
    }
    public void ResetStats()
    {
        bonusDamage = 0;
        bonusMoveSpeed = 0;
        bonusMaxHealth = 0;
        healOnLevelUp = 0;
        bonusCoinChance = 0;
        bonusProjectiles = 0;
        bonusArea = 0;
        bonusCooldown = 0;
    }
}