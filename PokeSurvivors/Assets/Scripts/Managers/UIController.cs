using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider playerExperienceSlider;
    [SerializeField] private TMP_Text experienceText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject levelUpPanel;
    public GameObject youWinPanel;
    [SerializeField] private TMP_Text timerText;

    [Header("Boss UI")]
    public GameObject bossHealthPanel;
    public Slider bossHealthSlider;
    public TMP_Text bossNameAndHealthText;

    [Header("Currency UI")]
    public TMP_Text coinText;

    public LevelUpButton[] levelUpButtons;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public void UpdateHealthSlider(){
        playerHealthSlider.maxValue = PlayerController.Instance.playerMaxHealth;
        playerHealthSlider.value = PlayerController.Instance.playerHealth;
        healthText.text = playerHealthSlider.value + " / " + playerHealthSlider.maxValue;
    }

    public void UpdateExperienceSlider(){
        playerExperienceSlider.maxValue = PlayerController.Instance.playerLevels[PlayerController.Instance.currentLevel - 1];
        playerExperienceSlider.value = PlayerController.Instance.experience;
        experienceText.text = playerExperienceSlider.value + " / " + playerExperienceSlider.maxValue;
    }

    public void UpdateTimer(float timer){
        float min = Mathf.FloorToInt(timer / 60f);
        float sec = Mathf.FloorToInt(timer % 60f);

        timerText.text = min + ":" + sec.ToString("00");
    }

    public void LevelUpPanelOpen(){
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LevelUpPanelClose(){
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateCoinUI(int currentCoins)
    {
        // "N0" adds commas to large numbers (e.g., 1,000)
        coinText.text = currentCoins.ToString("N0");
    }
    public void ShowBossHealth(float maxHealth, string bossName)
    {
        bossHealthPanel.SetActive(true);
        bossHealthSlider.maxValue = maxHealth;
        bossHealthSlider.value = 0; // Start at 0 for the fill effect
        StartCoroutine(FillBossBar(maxHealth, bossName));
    }

    private IEnumerator FillBossBar(float targetHealth, string bossName)
    {
        float duration = 1.0f; // How long the fill animation takes
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(0, targetHealth, elapsed / duration);
            bossHealthSlider.value = currentValue;
            bossNameAndHealthText.text = $"{bossName} {Mathf.FloorToInt(currentValue)} / {targetHealth}";
            yield return null;
        }

        bossHealthSlider.value = targetHealth;
    }

    public void UpdateBossHealth(float currentHealth, float maxHealth, string bossName)
    {
        // Hide panel if boss is dead
        if (currentHealth <= 0)
        {
            bossHealthPanel.SetActive(false);
            return;
        }

        bossHealthSlider.value = currentHealth;
        bossNameAndHealthText.text = $"{bossName} {Mathf.FloorToInt(currentHealth)} / {maxHealth}";
    }
    public void HideBossHealth()
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }
    }
}
