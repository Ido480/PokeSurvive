using UnityEngine;

public class PalkiaPortal : MonoBehaviour
{
    private float speed;
    private PalkiaBoss owner; // Reference back to Palkia to know where to teleport

    public void Setup(float playerSpeed, PalkiaBoss palkia)
    {
        speed = playerSpeed * 0.85f;
        owner = palkia;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (PlayerController.Instance != null)
        {
            Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    // PROBLEM 2 FIX: Teleport only on contact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && owner != null)
        {
            // Teleport player to Palkia's position
            PlayerController.Instance.transform.position = owner.transform.position;

            // Optional: Destroy portal after it hits you
            Destroy(gameObject);
        }
    }
}