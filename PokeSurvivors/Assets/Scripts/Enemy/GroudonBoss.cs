using System.Collections;
using UnityEngine;

public class GroudonBoss : Enemy
{
    [Header("Groudon Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fissureIndicatorPrefab;
    [SerializeField] private GameObject spikePrefab;
    private Vector2 lockedPosition;

    [Header("Cage Attack Settings")]
    [SerializeField] private GameObject cagePrefab;
    [SerializeField] private GameObject magmaPrefab;
    [SerializeField] private GameObject smallIndicatorPrefab;

    private float attackTimer;
    private bool isPerformingAttack;

    private void Update()
    {
        if (!GameManager.Instance.gameActive) return;

        // Only count up if we aren't already attacking
        if (!isPerformingAttack)
        {
            attackTimer += Time.deltaTime;

            // When the timer hits 5s, we trigger the "Selector" function
            if (attackTimer >= 5f)
            {
                attackTimer = 0;
                StartRandomAttack();
            }
        }
    }
    protected override void FixedUpdate()
    {
        if (isPerformingAttack)
        {
            if (lockedPosition == Vector2.zero) lockedPosition = rb.position;

            rb.linearVelocity = Vector2.zero;
            rb.position = lockedPosition;
            return;
        }

        lockedPosition = Vector2.zero;

        // 1. Run the base movement logic from Enemy.cs first
        base.FixedUpdate();

        // 2. DYNAMIC SCALE FIX:
        // Instead of forcing -2, we look at what base.FixedUpdate() did to the scale.
        // If it's positive (1), we make it 2. If it's negative (-1), we make it -2.
        float faceDirection = transform.localScale.x > 0 ? 2f : -2f;
        transform.localScale = new Vector3(faceDirection, 2f, 2f);
    }
    private void StartRandomAttack()
    {

        // This logic picks a random number (0 or 1)
        int choice = Random.Range(0, 2);

        if (choice == 0)
        {
            StartCoroutine(FissureAttack());
        }
        else
        {
            StartCoroutine(MagmaCageAttack());
        }

        // This resets the timer to a random negative number 
        // so there's a varied gap before the next attack choice
        attackTimer = -Random.Range(3f, 7f);
    }
    IEnumerator FissureAttack()
    {
        isPerformingAttack = true;
        rb.linearVelocity = Vector2.zero; // Immediate stop

        animator.SetBool("isAttackingBool", true);

        Vector3 targetDir = (PlayerController.Instance.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        GameObject indicator = Instantiate(fissureIndicatorPrefab, transform.position, Quaternion.Euler(0, 0, angle));

        float length = 0;
        while (length < 10f)
        {
            length += Time.deltaTime * 20f;
            indicator.transform.localScale = new Vector3(length, 1, 1);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 1; i < 10; i++)
        {
            Vector3 spikePos = transform.position + (targetDir * i);
            Instantiate(spikePrefab, spikePos, Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(indicator);

        // Brief pause so he doesn't instantly snap-move after the last spike
        yield return new WaitForSeconds(0.2f);

        animator.SetBool("isAttackingBool", false);
        isPerformingAttack = false;
    }
    IEnumerator MagmaCageAttack()
    {
        Vector3 playerPos = PlayerController.Instance.transform.position;

        
        isPerformingAttack = true;
        animator.SetBool("isAttackingBool", true);

        Vector3 cageOffset = new Vector3(3f, 3f, 0f);

        GameObject cage = Instantiate(cagePrefab, playerPos + cageOffset, Quaternion.identity);
        // ------------------

        int rainCount = Random.Range(6, 9);
        for (int i = 0; i < rainCount; i++)
        {
            // Use the playerPos (center) for the rain, not the cage's shifted pivot
            Vector3 spawnOffset = (Vector3)Random.insideUnitCircle * 1.2f; // Keep rain tight
            Vector3 strikePos = playerPos + spawnOffset;

            GameObject indicator = Instantiate(smallIndicatorPrefab, strikePos, Quaternion.identity);
            yield return new WaitForSeconds(0.6f);

            Instantiate(magmaPrefab, strikePos, Quaternion.identity);
            Destroy(indicator);

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(cage);
        isPerformingAttack = false;
        animator.SetBool("isAttackingBool", false);
    }
}