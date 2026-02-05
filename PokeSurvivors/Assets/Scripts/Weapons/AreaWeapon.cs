using UnityEngine;

public class AreaWeapon : Weapon
{
    [SerializeField] private GameObject prefab;
    private float spawnCounter;

    void Update()
    {
        if (weaponData == null) return;

        spawnCounter -= Time.deltaTime;

        if (spawnCounter <= 0)
        {
            WeaponLevelStats currentStats = GetCurrentStats();

            spawnCounter = currentStats.cooldown;
            Instantiate(prefab, transform.position, transform.rotation, transform);
        }
    }
}