using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArceusBoss : Enemy
{
    private bool isCurrentlyAttacking = false;
    private bool hasTriggeredPhase2 = false;
    private List<GameObject> activeEffects = new List<GameObject>();

    private Coroutine attackRoutineInstance;

    [Header("Arceus Animation")]
    [SerializeField] private Animator arceusAnimator;

    [Header("Groudon Power Prefabs")]
    [SerializeField] private GameObject fissureIndicatorPrefab;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject cagePrefab;
    [SerializeField] private GameObject magmaPrefab;
    [SerializeField] private GameObject smallIndicatorPrefab;

    [Header("Lugia Power Prefabs")]
    [SerializeField] private GameObject aoeIndicatorPrefab;

    [Header("Attack Settings")]
    [SerializeField] private float damageMultiplier = 2f;

    protected override void Start()
    {
        base.Start();
        if (arceusAnimator == null) arceusAnimator = GetComponent<Animator>();
        attackRoutineInstance = StartCoroutine(MasterAttackRoutine());
    }

    protected override void FixedUpdate()
    {
        // Check for 50% Health Trigger
        if (!hasTriggeredPhase2 && currentHealth <= data.maxHealth * 0.5f)
        {
            StartCoroutine(ArceusPhaseTwoRoutine());
        }

        if (!isCurrentlyAttacking && !isInvincible)
        {
            base.FixedUpdate();
            transform.localScale = Vector3.one;
            UpdateAnimation(rb.linearVelocity);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator ArceusPhaseTwoRoutine()
    {
        hasTriggeredPhase2 = true;

        // FIX: Stop ONLY the attack routine, not the whole script
        if (attackRoutineInstance != null) StopCoroutine(attackRoutineInstance);

        // CLEAN UP: Clear indicators and cages
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null) Destroy(effect);
        }
        activeEffects.Clear();

        isCurrentlyAttacking = true;
        isInvincible = true;

        // --- GHOSTLY VISUALS ---
        // Lower opacity and disable the collider so the player passes through him
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 1f, 0.4f);
        GetComponent<Collider2D>().enabled = false;

        arceusAnimator.SetBool("isMoving", false);
        arceusAnimator.SetBool("isAttack", false);
        arceusAnimator.SetBool("idle", true);

        // Trigger Resurrection
        DialgaBoss dialga = FindFirstObjectByType<DialgaBoss>();
        PalkiaBoss palkia = FindFirstObjectByType<PalkiaBoss>();
        if (dialga != null) ResurrectMiniBoss(dialga);
        if (palkia != null) ResurrectMiniBoss(palkia);

        // --- ROBUST WAIT LOOP ---
        bool guardiansStillKicking = true;
        while (guardiansStillKicking)
        {
            guardiansStillKicking = false;

            // Search the whole scene for any MiniBoss that isn't a ghost
            Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (Enemy e in allEnemies)
            {
                if (e.isMiniBoss && !e.isGhost)
                {
                    guardiansStillKicking = true;
                    break;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        // --- PHASE 2 COMPLETE ---
        isInvincible = false;
        isCurrentlyAttacking = false;

        // Restore Visuals and Physics
        sr.color = Color.white;
        GetComponent<Collider2D>().enabled = true;

        arceusAnimator.SetBool("idle", false);

        attackRoutineInstance = StartCoroutine(MasterAttackRoutine());

    }
    private void ResurrectMiniBoss(Enemy boss)
    {
        boss.isGhost = false;
        boss.currentHealth = boss.data.maxHealth;
        boss.GetComponent<Collider2D>().enabled = true;
        boss.GetComponent<SpriteRenderer>().color = Color.white;

        // Restore UI
        UIController.Instance.ShowBossHealth(boss.data.maxHealth, boss.data.enemyName);
    }

    IEnumerator MasterAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);

            if (PlayerController.Instance != null)
            {
                isCurrentlyAttacking = true;

                Vector2 dir = (PlayerController.Instance.transform.position - transform.position).normalized;
                arceusAnimator.SetFloat("moveX", dir.x);
                arceusAnimator.SetFloat("moveY", dir.y);
                arceusAnimator.SetBool("isAttack", true);

                int choice = Random.Range(0, 4);
                switch (choice)
                {
                    case 0: yield return StartCoroutine(FissureAttack()); break;
                    case 1: yield return StartCoroutine(MagmaCageAttack()); break;
                    case 2: yield return StartCoroutine(HomingAoEAttack()); break;
                    case 3: yield return StartCoroutine(SkyAttack()); break;
                }

                isCurrentlyAttacking = false;
                arceusAnimator.SetBool("isAttack", false);
            }
        }
    }

    // --- ATTACK FUNCTIONS WITH TRACKING ---

    IEnumerator FissureAttack()
    {
        Vector3 targetDir = (PlayerController.Instance.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        GameObject indicator = Instantiate(fissureIndicatorPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        activeEffects.Add(indicator);

        float length = 0;
        while (length < 10f)
        {
            length += Time.deltaTime * 20f;
            if (indicator != null) indicator.transform.localScale = new Vector3(length, 1, 1);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 1; i < 10; i++)
        {
            GameObject spike = Instantiate(spikePrefab, transform.position + (targetDir * i), Quaternion.identity);
            activeEffects.Add(spike);

            BossSpike sScript = spike.GetComponent<BossSpike>();
            if (sScript != null) sScript.damage *= damageMultiplier;

            yield return new WaitForSeconds(0.05f);
        }

        if (indicator != null)
        {
            activeEffects.Remove(indicator);
            Destroy(indicator);
        }
    }

    IEnumerator MagmaCageAttack()
    {
        Vector3 playerPos = PlayerController.Instance.transform.position;
        GameObject cage = Instantiate(cagePrefab, playerPos + new Vector3(3f, 3f, 0f), Quaternion.identity);
        activeEffects.Add(cage);

        int rainCount = Random.Range(6, 9);
        for (int i = 0; i < rainCount; i++)
        {
            Vector3 strikePos = playerPos + (Vector3)Random.insideUnitCircle * 1.2f;
            GameObject ind = Instantiate(smallIndicatorPrefab, strikePos, Quaternion.identity);
            activeEffects.Add(ind);

            yield return new WaitForSeconds(0.6f);

            GameObject magma = Instantiate(magmaPrefab, strikePos, Quaternion.identity);
            activeEffects.Add(magma);

            MagmaExplosion mScript = magma.GetComponent<MagmaExplosion>();
            if (mScript != null) mScript.damage *= damageMultiplier;

            if (ind != null)
            {
                activeEffects.Remove(ind);
                Destroy(ind);
            }
            yield return new WaitForSeconds(0.3f);
        }

        if (cage != null)
        {
            activeEffects.Remove(cage);
            Destroy(cage);
        }
    }

    IEnumerator HomingAoEAttack()
    {
        GameObject indicator = Instantiate(aoeIndicatorPrefab, PlayerController.Instance.transform.position, Quaternion.identity);
        activeEffects.Add(indicator);

        float followTime = 3f;
        while (followTime > 0)
        {
            followTime -= Time.deltaTime;
            if (indicator != null) indicator.transform.position = PlayerController.Instance.transform.position;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        if (indicator != null)
        {
            AoE_Explosion expScript = indicator.GetComponent<AoE_Explosion>();
            if (expScript != null) expScript.damage *= damageMultiplier;
            expScript.Explode();
            activeEffects.Remove(indicator);
        }
        CameraShake.Instance.ScreenShake(1f);
    }

    IEnumerator SkyAttack()
    {
        int count = 6;
        GameObject[] indicators = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = PlayerController.Instance.transform.position + (Vector3)Random.insideUnitCircle * 5f;
            indicators[i] = Instantiate(aoeIndicatorPrefab, pos, Quaternion.identity);
            activeEffects.Add(indicators[i]);

            AoE_Explosion expScript = indicators[i].GetComponent<AoE_Explosion>();
            if (expScript != null) expScript.damage *= damageMultiplier;
        }

        yield return new WaitForSeconds(2.0f);

        foreach (GameObject circle in indicators)
        {
            if (circle != null)
            {
                circle.GetComponent<AoE_Explosion>().Explode();
                activeEffects.Remove(circle);
            }
        }
        CameraShake.Instance.ScreenShake(1f);
    }

    private void UpdateAnimation(Vector2 velocity)
    {
        if (arceusAnimator == null) return;
        bool isMoving = velocity.sqrMagnitude > 0.1f;
        arceusAnimator.SetBool("isMoving", isMoving);
        if (isMoving)
        {
            arceusAnimator.SetFloat("moveX", velocity.x);
            arceusAnimator.SetFloat("moveY", velocity.y);
        }
    }
}