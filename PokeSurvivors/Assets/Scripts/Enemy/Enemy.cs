using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] protected Rigidbody2D rb;

    [Header("Health & Movement")]
    public float currentHealth; // Public so Arceus can resurrect
    private float currentMoveSpeed;
    private Vector3 direction;
    private float pushCounter;

    [Header("Type Settings")]
    public bool isBigBoss;
    public bool isMiniBoss;
    public bool canEnterGhostMode;
    public string nextSceneName;

    [Header("Stage 3 Trio Logic")]
    public bool isGhost = false;
    public bool isInvincible = false;

    [SerializeField] private GameObject destroyEffect;

    protected virtual void Start()
    {
        currentHealth = data.maxHealth;
        currentMoveSpeed = data.moveSpeed;

        if (isBigBoss || isMiniBoss)
        {
            UIController.Instance.ShowBossHealth(data.maxHealth, data.enemyName);
        }
    }

    protected virtual void FixedUpdate()
    {
        // Stop movement if this is a ghost or if the player is dead
        if (isGhost || !PlayerController.Instance.gameObject.activeSelf)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        direction = (PlayerController.Instance.transform.position - transform.position).normalized;

        // Flip Logic
        if (isBigBoss)
        {
            if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);
            else transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);
            else transform.localScale = new Vector3(1, 1, 1);
        }

        // Push back logic
        if (pushCounter > 0)
        {
            pushCounter -= Time.deltaTime;
            currentMoveSpeed = -data.moveSpeed;
            if (pushCounter <= 0) currentMoveSpeed = data.moveSpeed;
        }

        rb.linearVelocity = new Vector2(direction.x * currentMoveSpeed, direction.y * currentMoveSpeed);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!isGhost && collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(data.damage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        DamageNumberController.Instance.CreateNumber(damage, transform.position);
        pushCounter = data.pushTime;

        if (isBigBoss || isMiniBoss)
        {
            UIController.Instance.UpdateBossHealth(currentHealth, data.maxHealth, data.enemyName);
        }

        if (currentHealth <= 0)
        {
            float currentTime = GameManager.Instance.gameTime;

            if (isMiniBoss && currentTime >= 120f && currentTime < 240f && canEnterGhostMode)
            {
                EnterGhostMode();
            }
            else
            {
                DieNormally();
            }
        }
    }

    private void EnterGhostMode()
    {
        isGhost = true;
        currentHealth = 0;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 0.3f);

        //Pass 'this' so the UI knows to look for someone else
        CheckForRemainingBossUI(this);
    }

    private void DieNormally()
    {
        if (isBigBoss)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
                StageTransition.Instance.StartTransition(nextSceneName);
            else
                GameManager.Instance.WinGame();
        }

        float roll = Random.Range(0f, 100f);
        if (roll <= data.dropChance) PlayerController.Instance.AddCoins(data.coinValue);

        Instantiate(destroyEffect, transform.position, transform.rotation);
        PlayerController.Instance.GetExperience(data.expValue);
        AudioController.Instance.PlayModifiedSound(AudioController.Instance.enemyDie);

        // FIX: Pass 'this' so the UI knows to look for someone else
        if (isBigBoss || isMiniBoss) CheckForRemainingBossUI(this);

        Destroy(gameObject);
    }

    private void CheckForRemainingBossUI(Enemy caller)
    {
        DialgaBoss d = FindFirstObjectByType<DialgaBoss>();
        PalkiaBoss p = FindFirstObjectByType<PalkiaBoss>();
        ArceusBoss a = FindFirstObjectByType<ArceusBoss>();

        // If the boss we found is the one currently dying, ignore it!
        if (d == caller) d = null;
        if (p == caller) p = null;
        if (a == caller) a = null;

        if (d != null && !d.isGhost)
        {
            UIController.Instance.ShowBossHealth(d.data.maxHealth, d.data.enemyName);
            UIController.Instance.UpdateBossHealth(d.currentHealth, d.data.maxHealth, d.data.enemyName);
        }
        else if (p != null && !p.isGhost)
        {
            UIController.Instance.ShowBossHealth(p.data.maxHealth, p.data.enemyName);
            UIController.Instance.UpdateBossHealth(p.currentHealth, p.data.maxHealth, p.data.enemyName);
        }
        else if (a != null)
        {
            UIController.Instance.ShowBossHealth(a.data.maxHealth, a.data.enemyName);
            UIController.Instance.UpdateBossHealth(a.currentHealth, a.data.maxHealth, a.data.enemyName);
        }
        else
        {
            UIController.Instance.HideBossHealth();
        }
    }
}