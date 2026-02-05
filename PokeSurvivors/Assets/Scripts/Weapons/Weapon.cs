using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    public int weaponLevel = 1;
    public GlobalStats globalStats;

    public WeaponLevelStats GetCurrentStats()
    {
        int index = Mathf.Max(0, weaponLevel - 1);

        if (index >= weaponData.levels.Count)
            index = weaponData.levels.Count - 1;

        WeaponLevelStats baseData = weaponData.levels[index];

        WeaponLevelStats calculatedStats = new WeaponLevelStats();

        calculatedStats.damage = baseData.damage * (1 + globalStats.bonusDamage);
        calculatedStats.range = baseData.range * (1 + globalStats.bonusArea);
        calculatedStats.cooldown = baseData.cooldown * (1 - globalStats.bonusCooldown);
        calculatedStats.amount = baseData.amount + globalStats.bonusProjectiles;

        calculatedStats.speed = baseData.speed;
        calculatedStats.duration = baseData.duration;

        return calculatedStats;
    }
    public void LevelUp()
    {
        // If we have 10 levels, we can level up as long as we are below Level 10
        if (weaponLevel < weaponData.levels.Count)
        {
            weaponLevel++;

            // Max Level Logic: If the current level IS the count of the list
            if (weaponLevel >= weaponData.levels.Count)
            {
                if (!PlayerController.Instance.maxLevelWeapons.Contains(this))
                {
                    PlayerController.Instance.maxLevelWeapons.Add(this);
                    PlayerController.Instance.activeWeapons.Remove(this);
                }
            }
        }
    }
}