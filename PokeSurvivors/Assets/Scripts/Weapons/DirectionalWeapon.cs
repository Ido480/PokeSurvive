using UnityEngine;

public class DirectionalWeapon : Weapon
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

            // Try to find the nearest enemy
            Transform target = GetNearestEnemy(currentStats.range * 2f); // Range check

            if (target != null)
            {
                spawnCounter = currentStats.cooldown;

                for (int i = 0; i < (int)currentStats.amount; i++)
                {
                    GameObject proj = Instantiate(prefab, transform.position, transform.rotation);

                    // Add a random offset to the target position for each ball
                    Vector3 spread = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                    proj.GetComponent<DirectionalWeaponPrefab>().SetTarget(target.position + spread);
                }
            }
        }
    }

    private Transform GetNearestEnemy(float checkRange)
    {
        // Find all colliders in range
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, checkRange);
        Transform closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closest = enemy.transform;
                }
            }
        }
        return closest;
    }
}