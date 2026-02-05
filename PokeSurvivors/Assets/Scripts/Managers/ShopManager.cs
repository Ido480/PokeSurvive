using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GlobalStats globalStats;
    public TMP_Text totalCoinText;

    [System.Serializable]
    public class ShopItem
    {
        public string statKey; // Matches the PlayerPrefs key (e.g., "Damage")
        public string displayName;
        public TMP_Text levelText;
        public TMP_Text priceText;
        public Button buyButton;
        public int maxLevel;
        public int basePrice;
    }

    [Header("Upgrade Categories")]
    public ShopItem[] basicUpgrades;
    public ShopItem[] specialUpgrades;

    void OnEnable()
    {
        UpdateShopUI();
    }

    public void UpdateShopUI()
    {
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        totalCoinText.text = "Coins: " + currentCoins.ToString("N0");

        foreach (var item in basicUpgrades) SetupButton(item, currentCoins);
        foreach (var item in specialUpgrades) SetupButton(item, currentCoins);
    }

    private void SetupButton(ShopItem item, int currentCoins)
    {
        int level = PlayerPrefs.GetInt(item.statKey, 0);
        // Price formula: 100 * 2^level (100, 200, 400...)
        int price = item.basePrice * (int)Mathf.Pow(2, level);

        item.levelText.text = $"Lv. {level}/{item.maxLevel}";

        if (level >= item.maxLevel)
        {
            item.priceText.text = "MAXED";
            item.buyButton.interactable = false;
        }
        else
        {
            item.priceText.text = price.ToString() + " Coins";
            item.buyButton.interactable = currentCoins >= price;
        }
    }

    public void PurchaseUpgrade(string key)
    {
        int level = PlayerPrefs.GetInt(key, 0);

        ShopItem item = null;
        foreach (var i in basicUpgrades) if (i.statKey == key) item = i;
        foreach (var i in specialUpgrades) if (i.statKey == key) item = i;

        if (item == null) return;

        int price = item.basePrice * (int)Mathf.Pow(2, level);
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);

        if (currentCoins >= price && level < item.maxLevel)
        {
            // Spend money
            currentCoins -= price;
            PlayerPrefs.SetInt("TotalCoins", currentCoins);

            // Increase level
            level++;
            PlayerPrefs.SetInt(key, level);

            // Apply to GlobalStats
            ApplyStatToGlobal(key, level);

            UpdateShopUI();
        }
    }

    private void ApplyStatToGlobal(string key, int level)
    {
        switch (key)
        {
            // Basic Upgrades
            case "Damage": globalStats.bonusDamage = level * 0.10f; break; // +10% per lv
            case "Speed": globalStats.bonusMoveSpeed = level * 0.05f; break; // +5% per lv
            case "Health": globalStats.bonusMaxHealth = level * 20f; break; // +20 flat per lv
            case "Heal": globalStats.healOnLevelUp = level * 5f; break; // +5 heal per lv

            // Special Upgrades
            case "Coins": globalStats.bonusCoinChance = level * 5f; break; // +5% chance per lv
            case "Projectiles": globalStats.bonusProjectiles = level; break; // +1 per lv
            case "Area": globalStats.bonusArea = level * 0.10f; break; // +10% size per lv
            case "Cooldown": globalStats.bonusCooldown = level * 0.05f; break; // 5% faster per lv
        }
        // Mark SO as dirty so Unity saves the change to the file
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(globalStats);
#endif
    }


    [ContextMenu("DEBUG: RESET ALL PROGRESS")]
    public void ResetAllProgress()
    {
        // 1. Wipe the PlayerPrefs (Coins, Stat levels, everything)
        PlayerPrefs.DeleteAll();

        // 2. Reset the ScriptableObject values to 0
        if (globalStats != null)
        {
            globalStats.ResetStats();

        }

        // 3. Refresh the UI so you can see the reset happen instantly
        UpdateShopUI();

    }
}