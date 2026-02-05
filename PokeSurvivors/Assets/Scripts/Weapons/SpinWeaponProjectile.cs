using UnityEngine;

public class SpinWeaponProjectile : MonoBehaviour
{
    private SpinWeapon weapon;
    private WeaponLevelStats currentStats;

    void Start()
    {
        weapon = GameObject.Find("Spin Weapon").GetComponent<SpinWeapon>();
        currentStats = weapon.GetCurrentStats();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();

            // Safety check for destroyed enemies
            if (enemy != null)
            {
                enemy.TakeDamage(currentStats.damage);
            }
        }
    }
}