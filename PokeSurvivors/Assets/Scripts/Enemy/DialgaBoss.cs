using System.Collections;
using UnityEngine;

public class DialgaBoss : Enemy
{
    private bool isAttacking;
    private Vector2 lockedPosition;
    [SerializeField] private Animator animator;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(RoarOfTimeRoutine());
    }

    protected override void FixedUpdate()
    {
        if (isGhost) { rb.linearVelocity = Vector2.zero; return; }
        if (isAttacking)
        {
            if (lockedPosition == Vector2.zero) lockedPosition = rb.position;
            rb.linearVelocity = Vector2.zero;
            rb.position = lockedPosition;
            return;
        }

        // PHASE 2: Chase the player at high speed
        lockedPosition = Vector2.zero;
        base.FixedUpdate();

        // Ensure Dialga stays at a large boss scale
        float faceDirection = transform.localScale.x > 0 ? 1f : -1f;
        transform.localScale = new Vector3(faceDirection, 1f, 1f);
    }

IEnumerator RoarOfTimeRoutine()
{
    yield return new WaitForEndOfFrame();

    // Loop forever until Dialga is destroyed
    while (true)
    {
        if (isGhost) yield break; // Stop the loop if he becomes a ghost
            // --- START ATTACK ---
        isAttacking = true;
        
        if (animator != null) 
            animator.SetBool("isAttacking", true);

        // Phase 1 (2x Speed)
        Time.timeScale = 2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSeconds(5f); 

        // Phase 2 (3x Speed)
        isAttacking = false; 
        if (animator != null) 
            animator.SetBool("isAttacking", false);

        Time.timeScale = 3f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSeconds(5f);

        // --- RESET TIME ---
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // --- THE GAP ---
        // Dialga waits 30 seconds before warping time again
        yield return new WaitForSeconds(30f);
    }
}    private void OnDestroy()
    {
        // Safety check: Reset time if Dialga is killed mid-attack
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}