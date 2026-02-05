using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float gameTime;
    public bool gameActive;
    [SerializeField] private PlayerStats runStats;
    [SerializeField] private GlobalStats globalStats;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    void Start(){
        gameActive = true;
    }

    void Update()
    {
        if (gameActive)
        {
            gameTime += Time.deltaTime;
            UIController.Instance.UpdateTimer(gameTime);

            // Pause logic
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                Pause();
            }
        }
    }

    public void GameOver(){
        gameActive = false;
        StartCoroutine(ShowGameOverScreen());
    }

    IEnumerator ShowGameOverScreen(){
        yield return new WaitForSeconds(1.5f);
        UIController.Instance.gameOverPanel.SetActive(true);
        AudioController.Instance.PlaySound(AudioController.Instance.gameOver);
    }

    public void Restart()
    {
        runStats.ResetStats(globalStats);
        SceneManager.LoadScene("Stage 1");
    }

    public void Pause(){
        if (UIController.Instance.levelUpPanel.activeSelf == false){
            if (
                UIController.Instance.pausePanel.activeSelf == false && 
                UIController.Instance.gameOverPanel.activeSelf == false
                ){
                UIController.Instance.pausePanel.SetActive(true);
                Time.timeScale = 0f;
                AudioController.Instance.PlaySound(AudioController.Instance.pause);
            } else {
                UIController.Instance.pausePanel.SetActive(false);
                Time.timeScale = 1f;
                AudioController.Instance.PlaySound(AudioController.Instance.unpause);
            }
        }
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void GoToMainMenu(){
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1f;
    }

    public void WinGame()
    {
        gameActive = false;
        StartCoroutine(ShowWinScreen());
    }
    IEnumerator ShowWinScreen()
    {
        yield return new WaitForSeconds(2.0f);
        UIController.Instance.youWinPanel.SetActive(true);
    }
}
