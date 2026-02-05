using UnityEngine;

public class MagmaExplosion : MonoBehaviour
{
    public float damage = 20f;
    [SerializeField] private float stayTime = 0.2f;

    void Start()
    {
        Animator anim = GetComponent<Animator>();
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animLength + stayTime);

        // Deal damage as soon as it spawns (impact)
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        // Using a small radius 
        float radius = 0.7f;
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius);
        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(damage);
        }
    }
}