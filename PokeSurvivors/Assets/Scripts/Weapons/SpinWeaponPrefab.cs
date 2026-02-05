using UnityEngine;

public class SpinWeaponPrefab : MonoBehaviour
{
    private SpinWeapon weapon;
    private float duration;
    private Vector3 targetSize;
    [SerializeField] private GameObject projectile;
    private WeaponLevelStats currentStats;

    void Start()
    {
        weapon = GameObject.Find("Spin Weapon").GetComponent<SpinWeapon>();
        currentStats = weapon.GetCurrentStats();

        duration = currentStats.duration;
        targetSize = Vector3.one;
        transform.localScale = Vector3.zero;

        // Position the projectile based on the 'range' stat
        projectile.transform.localPosition = new Vector3(0f, currentStats.range, 0f);

        AudioController.Instance.PlaySound(AudioController.Instance.spinWeaponSpawn);
    }

    void Update()
    {
        // Rotate based on the 'speed' stat
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + (90 * Time.deltaTime * currentStats.speed));

        // Grow animation
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime * 3);

        // Shrink and cleanup logic
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            targetSize = Vector3.zero;
            if (transform.localScale.x == 0f)
            {
                AudioController.Instance.PlaySound(AudioController.Instance.spinWeaponDespawn);
                Destroy(gameObject);
            }
        }
    }

    public void SetRotationOffset(float rotationOffset)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, rotationOffset);
    }
}