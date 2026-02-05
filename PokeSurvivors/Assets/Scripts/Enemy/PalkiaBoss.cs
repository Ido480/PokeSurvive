using UnityEngine;
using System.Collections;

public class PalkiaBoss : Enemy
{
    [Header("Palkia Specific Boundaries")]
    [SerializeField] private Transform innerMin; // Place at bottom-left of orange outline
    [SerializeField] private Transform innerMax; // Place at top-right of orange outline

    [Header("Palkia Settings")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Animator animator;

    private Vector2 safeMin;
    private Vector2 safeMax;

    protected override void Start()
    {
        base.Start();

        // Cache the positions of your custom orange boundaries
        if (innerMin != null && innerMax != null)
        {
            safeMin = innerMin.position;
            safeMax = innerMax.position;
        }
        StartCoroutine(SpacialRendRoutine());
    }

    protected override void FixedUpdate()
    {
        if (isGhost)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        base.FixedUpdate();
    }

    IEnumerator SpacialRendRoutine()
    {
        while (true)
        {
            // If Palkia is a ghost, wait here and don't proceed with attacks
            if (isGhost)
            {
                yield return new WaitUntil(() => !isGhost);
            }

            yield return new WaitForSeconds(Random.Range(6f, 10f));

            if (IsInsideOrangeOutline() && !isGhost)
            {
                if (PlayerController.Instance != null)
                {
                    if (animator != null) animator.SetTrigger("isAttacking");

                    GameObject portal = Instantiate(portalPrefab, transform.position, Quaternion.identity);
                    portal.GetComponent<PalkiaPortal>().Setup(PlayerController.Instance.moveSpeed, this);
                }
            }
        }
    }
    private bool IsInsideOrangeOutline()
    {
        return transform.position.x > safeMin.x && transform.position.x < safeMax.x &&
               transform.position.y > safeMin.y && transform.position.y < safeMax.y;
    }
}