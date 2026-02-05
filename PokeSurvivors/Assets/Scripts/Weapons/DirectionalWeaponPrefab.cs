using UnityEngine;

public class DirectionalWeaponPrefab : MonoBehaviour
{
    private DirectionalWeapon weapon;
    private Rigidbody2D rb;
    private float duration;
    private WeaponLevelStats currentStats;
    private Vector3 moveDirection;

    // This is called by the DirectionalWeapon script immediately after Instantiate
    public void SetTarget(Vector3 targetPos)
    {
        // Calculate the direction from the projectile to the enemy
        moveDirection = (targetPos - transform.position).normalized;

        // Rotate the sprite to face the target direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Start()
    {
        weapon = GameObject.Find("Directional Weapon").GetComponent<DirectionalWeapon>();

        // Cache the stats once to keep Update clean
        currentStats = weapon.GetCurrentStats();

        duration = currentStats.duration;
        rb = GetComponent<Rigidbody2D>();

        // Apply velocity toward the target with a tiny bit of random spread
        float randomAngle = Random.Range(-0.1f, 0.1f);
        rb.linearVelocity = new Vector2(moveDirection.x + randomAngle, moveDirection.y + randomAngle) * currentStats.speed;

        AudioController.Instance.PlaySound(AudioController.Instance.directionalWeaponSpawn);
    }

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 5);
            if (transform.localScale.x == 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(currentStats.damage);
                AudioController.Instance.PlaySound(AudioController.Instance.directionalWeaponHit);

                // Typically, targeted projectiles destroy themselves on impact
                Destroy(gameObject);
            }
        }
    }
}