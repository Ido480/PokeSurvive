using UnityEngine;

public class AoE_Explosion : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public float damage = 20f;
    [SerializeField] private float explosionRadius = 2f;

    public void Explode()
    {
        // 1. Randomly pick an explosion animation
        // Assuming you have "Explosion1" and "Explosion2" triggers in the animator
        string triggerName = Random.Range(0, 2) == 0 ? "Explosion1" : "Explosion2";
        anim.SetTrigger(triggerName);
       
        
        // 2. Check if player is inside
        float dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        if (dist <= explosionRadius)
        {
            PlayerController.Instance.TakeDamage(damage);
        }

        // 3. Destroy the indicator after the animation finishes
        Destroy(gameObject, 1f);
    }
}