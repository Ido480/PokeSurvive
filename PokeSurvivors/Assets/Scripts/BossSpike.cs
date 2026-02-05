using UnityEngine;

public class BossSpike : MonoBehaviour
{
    public float damage = 25f;
    private bool hasDealtDamage = false;

    // This runs automatically if the Player hits the Spike's Trigger Collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasDealtDamage && collision.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(damage);
            hasDealtDamage = true; // Prevents hitting the player 100 times in 1 second
        }
    }

    private void Start()
    {
        // Auto-destroy after animation
        Destroy(gameObject, 1.2f);
    }
}