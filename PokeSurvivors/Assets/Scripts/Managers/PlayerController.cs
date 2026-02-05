using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] public float moveSpeed;
    public Vector3 playerMoveDirection;
    public Vector3 lastMoveDirection;
    public float playerMaxHealth;
    public float playerHealth;

    public int totalCoins;
    public int experience;
    public int currentLevel;
    public int maxLevel;

    public GlobalStats globalStats;
    [SerializeField] private PlayerStats runStats;

    [SerializeField] private List<Weapon> inactiveWeapons;
    public List<Weapon> activeWeapons;
    [SerializeField] private List<Weapon> upgradeableWeapons;
    public List<Weapon> maxLevelWeapons;

    private bool isImmune;
    [SerializeField] private float immunityDuration;
    [SerializeField] private float immunityTimer;

    public List<int> playerLevels;
    
    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    void Start()
    {
        // 1. Basic Stat Setup
        playerMaxHealth = 30 + globalStats.bonusMaxHealth;
        moveSpeed = 2.5f + globalStats.bonusMoveSpeed;
        lastMoveDirection = new Vector3(0, -1);

        // 2. 30-Level EXP Curve
        playerLevels = new List<int> {
        50, 100, 175, 275, 400, 550, 725, 950, 1200,
        1600, 2100, 2700, 3400, 4200, 5100, 6100, 7200, 8400, 9700,
        12000, 15000, 18000, 22000, 27000, 33000, 40000, 50000, 65000, 85000
    };

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Stage 1")
        {
            runStats.ResetStats(globalStats);

            playerHealth = playerMaxHealth;
            currentLevel = 1;
            experience = 0;

            int rand = Random.Range(0, inactiveWeapons.Count);
            Weapon startWeapon = inactiveWeapons[rand];
            startWeapon.weaponLevel = 1;
            ActivateWeapon(startWeapon);
        }
        else
        {
            LoadRunData();
        }

        // 4. UI & Persistent Data
        UIController.Instance.UpdateHealthSlider();
        UIController.Instance.UpdateExperienceSlider();
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        UIController.Instance.UpdateCoinUI(totalCoins);
    }    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(inputX, inputY).normalized;

        if (playerMoveDirection == Vector3.zero){
            animator.SetBool("moving", false);
        } else if (Time.timeScale != 0) {
            animator.SetBool("moving", true);
            animator.SetFloat("moveX", inputX);
            animator.SetFloat("moveY", inputY);
            lastMoveDirection = playerMoveDirection;
        }

        if (immunityTimer > 0){
            immunityTimer -= Time.deltaTime;
        } else {
            isImmune = false;
        }
    }

    void FixedUpdate(){
        rb.linearVelocity = new Vector3(playerMoveDirection.x * moveSpeed, playerMoveDirection.y * moveSpeed);
    }
    private void LoadRunData()
    {
        currentLevel = runStats.currentLevel;
        experience = runStats.currentExperience;
        playerHealth = runStats.currentHealth;

        // Use a temporary list to avoid modifying the collection while iterating
        List<PlayerStats.SavedWeapon> weaponsToLoad = new List<PlayerStats.SavedWeapon>(runStats.savedWeapons);

        foreach (var saved in weaponsToLoad)
        {
            Weapon weaponToActivate = inactiveWeapons.FirstOrDefault(w => w.weaponData.weaponName == saved.weaponName);

            if (weaponToActivate != null)
            {
                // IMPORTANT: Set level BEFORE activating so the weapon doesn't default to 1
                weaponToActivate.weaponLevel = saved.level;

                // Manual activation logic so we don't trigger the "Level 1" reset in ActivateWeapon
                weaponToActivate.gameObject.SetActive(true);

                if (weaponToActivate.weaponLevel >= weaponToActivate.weaponData.levels.Count)
                {
                    maxLevelWeapons.Add(weaponToActivate);
                }
                else
                {
                    activeWeapons.Add(weaponToActivate);
                }

                inactiveWeapons.Remove(weaponToActivate);
            }
        }
    }

    public void TakeDamage(float damage){
        if (!isImmune){
            isImmune = true;
            immunityTimer = immunityDuration;
            playerHealth -= damage;
            UIController.Instance.UpdateHealthSlider();
            if (playerHealth <= 0){
                gameObject.SetActive(false);
                GameManager.Instance.GameOver();
            }
        }
    }

    public void GetExperience(int experienceToGet){
        experience += experienceToGet;
        UIController.Instance.UpdateExperienceSlider();
        if (experience >= playerLevels[currentLevel - 1]){
            LevelUp();
        }
    }

    public void LevelUp(){
        experience -= playerLevels[currentLevel - 1];
        currentLevel++;
        UIController.Instance.UpdateExperienceSlider();
        //UIController.Instance.levelUpButtons[0].ActivateButton(activeWeapon);

        playerHealth = Mathf.Min(playerHealth + globalStats.healOnLevelUp, playerMaxHealth);
        UIController.Instance.UpdateHealthSlider();
        upgradeableWeapons.Clear();

        if (activeWeapons.Count > 0){
            upgradeableWeapons.AddRange(activeWeapons);
        }
        if (inactiveWeapons.Count > 0){
            upgradeableWeapons.AddRange(inactiveWeapons);
        }
        for (int i = 0; i < UIController.Instance.levelUpButtons.Length; i++){
            if (upgradeableWeapons.ElementAtOrDefault(i) != null){
                UIController.Instance.levelUpButtons[i].ActivateButton(upgradeableWeapons[i]);
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(true);
            } else {
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(false);
            }
        }

        UIController.Instance.LevelUpPanelOpen();
    }

    private void AddWeapon(int index){
        activeWeapons.Add(inactiveWeapons[index]);
        inactiveWeapons[index].gameObject.SetActive(true);
        inactiveWeapons.RemoveAt(index);
    }

    public void ActivateWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(true);
        // Only set to 1 if it's a brand new weapon, otherwise keep existing level
        if (weapon.weaponLevel == 0) weapon.weaponLevel = 1;

        activeWeapons.Add(weapon);
        inactiveWeapons.Remove(weapon);
    }
    public void IncreaseMaxHealth(int value){
        playerMaxHealth += value;
        playerHealth = playerMaxHealth;
        UIController.Instance.UpdateHealthSlider();

        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }

    public void IncreaseMovementSpeed(float multiplier){
        moveSpeed *= multiplier;

        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }

    public void AddCoins(int amount)
    {
        // Load, Add, and Save
        int savedCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        savedCoins += amount;
        PlayerPrefs.SetInt("TotalCoins", savedCoins);

        // Update local variable
        totalCoins = savedCoins;

        // Update the UI!
        UIController.Instance.UpdateCoinUI(totalCoins);
    }
}
