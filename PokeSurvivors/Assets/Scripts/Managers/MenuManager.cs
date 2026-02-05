using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject shopBackGround;

    [Header("Decoration Sprites")]
    [SerializeField] private GameObject[] menuEnemies;

    [SerializeField] private GlobalStats globalStats;
    [SerializeField] private PlayerStats runStats;

    public void NewGame()
    {
        runStats.ResetStats(globalStats);
        SceneManager.LoadScene("Stage 1");
    }

    public void OpenShop()
    {
        mainMenuCanvas.SetActive(false);
        background.SetActive(false);
        shopBackGround.SetActive(true);
        shopCanvas.SetActive(true);
        foreach (GameObject enemy in menuEnemies) enemy.SetActive(false);
    }

    public void CloseShop()
    {
        mainMenuCanvas.SetActive(true);
        background.SetActive(true);
        shopBackGround.SetActive(false);
        shopCanvas.SetActive(false);
        foreach (GameObject enemy in menuEnemies) enemy.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}