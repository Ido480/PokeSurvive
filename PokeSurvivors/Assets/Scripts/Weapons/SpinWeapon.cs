using UnityEngine;

public class SpinWeapon : Weapon
{
    public GameObject prefab;
    private float spawnCounter;

    void Update()
    {
        if (weaponData == null) return;

        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            WeaponLevelStats currentStats = GetCurrentStats();
            spawnCounter = currentStats.cooldown;

            int amount = (int)currentStats.amount;
            for (int i = 0; i < amount; i++)
            {
                GameObject spawnedWeapon = Instantiate(prefab, transform.position, transform.rotation, transform);

                // Calculate even spacing based on the amount of projectiles
                float rotation = 360f / amount * i;
                spawnedWeapon.GetComponent<SpinWeaponPrefab>().SetRotationOffset(rotation);
            }
        }
    }
}