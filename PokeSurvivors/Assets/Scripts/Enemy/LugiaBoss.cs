using System.Collections;
using UnityEngine;

public class LugiaBoss : Enemy
{
    [Header("Lugia Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float attackCooldown = 5f;
    private float attackTimer;
    private bool isPerformingAttack = false;

    // References for the attacks
    [SerializeField] private GameObject aoeIndicatorPrefab;

    private void Update()
    {
        if (!GameManager.Instance.gameActive) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown && !isPerformingAttack)
        {
            attackTimer = 0;
            StartRandomAttack();
        }
    }
    protected override void FixedUpdate()
    {
        // If we are attacking, stop the Rigidbody and don't run movement logic
        if (isPerformingAttack)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            return;
        }

        // Otherwise, do the normal movement from the Enemy script
        base.FixedUpdate();
    }
    private void StartRandomAttack()
    {
        isPerformingAttack = true;

        animator.SetTrigger("isAttacking");


        int attackChoice = Random.Range(0, 2); // 0 or 1

        if (attackChoice == 0)
            StartCoroutine(HomingAoEAttack());
        else
            StartCoroutine(SkyAttack());

        attackTimer = -Random.Range(3f, 7f); // Random delay before next attack
    }

    IEnumerator HomingAoEAttack()
    {
        // Phase 1: Create Indicator and Follow Player
        GameObject indicator = Instantiate(aoeIndicatorPrefab, PlayerController.Instance.transform.position, Quaternion.identity);
        float followTime = 3f;

        while (followTime > 0)
        {
            followTime -= Time.deltaTime;
            // Smoothly follow player position
            indicator.transform.position = PlayerController.Instance.transform.position;
            yield return null;
        }

        // Phase 2: Lock Position (Wait 0.5s)
        yield return new WaitForSeconds(0.5f);

        // Phase 3: Explode!
        indicator.GetComponent<AoE_Explosion>().Explode();

        // Phase 4 : Screen Shake
        CameraShake.Instance.ScreenShake(1f);

        // Cleanup
        isPerformingAttack = false;
    }
    IEnumerator SkyAttack()
    {
        int circleCount = 6;
        GameObject[] indicators = new GameObject[circleCount];

        for (int i = 0; i < circleCount; i++)
        {
            // Get a random position near the player but not exactly on them
            Vector3 spawnPos = PlayerController.Instance.transform.position + (Vector3)Random.insideUnitCircle * 5f;
            indicators[i] = Instantiate(aoeIndicatorPrefab, spawnPos, Quaternion.identity);
        }

        // Wait for the "Charge Up"
        yield return new WaitForSeconds(2.0f);

        // Explode all of them
        foreach (GameObject circle in indicators)
        {
            if (circle != null) circle.GetComponent<AoE_Explosion>().Explode();
        }

        // Screen Shake
        CameraShake.Instance.ScreenShake(1f);

        yield return new WaitForSeconds(0.5f);
        isPerformingAttack = false;
    }
}