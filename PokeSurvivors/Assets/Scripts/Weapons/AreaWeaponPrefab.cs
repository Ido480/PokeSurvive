using System.Collections.Generic;
using UnityEngine;

public class AreaWeaponPrefab : MonoBehaviour
{
    public AreaWeapon weapon;
    private Vector3 targetSize;
    private float timer;
    public List<Enemy> enemiesInRange;
    private float counter;

    private WeaponLevelStats currentStats;

    void Start()
    {
        if (weapon == null)
            weapon = GameObject.Find("Area Weapon").GetComponent<AreaWeapon>();

        // We cache the stats once here
        currentStats = weapon.GetCurrentStats();

        targetSize = Vector3.one * currentStats.range;
        transform.localScale = Vector3.zero;
        timer = currentStats.duration;

        AudioController.Instance.PlaySound(AudioController.Instance.areaWeaponSpawn);
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime * 5);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            targetSize = Vector3.zero;
            if (transform.localScale.x == 0f)
            {
                AudioController.Instance.PlaySound(AudioController.Instance.areaWeaponDespawn);
                Destroy(gameObject);
            }
        }

        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            counter = currentStats.speed;

            enemiesInRange.RemoveAll(item => item == null);

            for (int i = 0; i < enemiesInRange.Count; i++)
            {
                enemiesInRange[i].TakeDamage(currentStats.damage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) enemiesInRange.Remove(enemy);
        }
    }
}