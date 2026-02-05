using UnityEngine;

public class CoinDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public int coinsToAdd = 100;

    [ContextMenu("Add Coins")] // This adds a button to the 3-dot menu in Inspector
    public void DebugAddCoins()
    {
        int current = PlayerPrefs.GetInt("TotalCoins", 0);
        PlayerPrefs.SetInt("TotalCoins", current + coinsToAdd);
        Debug.Log($"<color=yellow>DEBUG:</color> Added {coinsToAdd} coins. Total: {PlayerPrefs.GetInt("TotalCoins")}");
    }

    [ContextMenu("RESET ALL COINS")]
    public void DebugResetCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", 0);
        Debug.Log("<color=red>DEBUG:</color> Coins have been wiped to 0.");
    }
}