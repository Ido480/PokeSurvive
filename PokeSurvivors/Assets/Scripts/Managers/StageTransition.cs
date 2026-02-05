using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageTransition : MonoBehaviour
{
    public static StageTransition Instance;

    [Header("References")]
    [SerializeField] private GameObject containerObject; // The black background + text parent
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private PlayerStats playerStats;

    void Awake() => Instance = this;

    void Start()
    {
        // Make sure it's visible so we can see the fade-in at start of scene
        containerObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 1);
        statusText.text = "";
        StartCoroutine(FadeIn());
    }

    private void SavePlayerData()
    {
        PlayerController pc = PlayerController.Instance;
        playerStats.currentLevel = pc.currentLevel;
        playerStats.currentExperience = pc.experience;

        // Save full health for stage clear
        playerStats.currentHealth = pc.playerMaxHealth;

        playerStats.savedWeapons.Clear();

        // Check every possible list to find weapons the player has touched
        List<Weapon> weaponsToSave = new List<Weapon>();
        weaponsToSave.AddRange(pc.activeWeapons);
        weaponsToSave.AddRange(pc.maxLevelWeapons);

        foreach (Weapon w in weaponsToSave)
        {
            playerStats.savedWeapons.Add(new PlayerStats.SavedWeapon
            {
                weaponName = w.weaponData.weaponName,
                level = w.weaponLevel
            });
        }
    }
    IEnumerator FadeIn()
    {
        float alpha = 1;
        statusText.text = "";

        // Using > 0.01f is safer for float math than > 0
        while (alpha > 0.01f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0); // Force alpha to zero
        containerObject.SetActive(false);        // Turn off the parent object
    }
    public void StartTransition(string nextSceneName)
    {
        containerObject.SetActive(true);
        // We pass the name to the routine which now handles the initial wait
        StartCoroutine(TransitionRoutine(nextSceneName));
    }
    private IEnumerator TransitionRoutine(string nextSceneName)
    {
        statusText.text = "";
        fadeImage.color = new Color(0, 0, 0, 0); // Ensure it's transparent during the wait
        yield return new WaitForSeconds(1f);
        // -----------------

        // 1. Fade to Black
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        // 2. The Countdown
        statusText.text = "Stage Clear!";
        yield return new WaitForSeconds(1f);

        for (int i = 3; i > 0; i--)
        {
            statusText.text = "Moving to next stage in " + i + "...";
            yield return new WaitForSeconds(1f);
        }

        statusText.text = "Loading...";
        // 3. Save Player Data
        SavePlayerData();
        SceneManager.LoadScene(nextSceneName);
    }
}