using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverMessageText;

    [Header("Buttons")]
    [SerializeField] private Button returnToTownButton;
    [SerializeField] private Button restartFromFloor1Button;
    [SerializeField] private Button returnToStartButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Start()
    {
        // 버튼 
        if (returnToTownButton != null)
        {
            returnToTownButton.onClick.AddListener(OnReturnToTown);
        }

        if (restartFromFloor1Button != null)
        {
            restartFromFloor1Button.onClick.AddListener(OnRestartFromFloor1);
        }

        if (returnToStartButton != null)
        {
            returnToStartButton.onClick.AddListener(OnReturnToStart);
        }
    }

    // 게임오버 ui
    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;

            if (gameOverMessageText != null && GameManager.Instance != null)
            {
                int floor = GameManager.Instance.saveData.dungeonFloor;
                gameOverMessageText.text = $"Game Over\n{floor}층에서 사망...";
            }

        }
    }

    // 마을 씬으로
    void OnReturnToTown()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;


        DungeonPlayerController player = FindObjectOfType<DungeonPlayerController>();
        if (player != null)
        {
            player.RecoverHealth(); 
        }


        SaveGameData();

        PlayerPrefs.SetInt("RecoverHealth", 1);

        SceneManager.LoadScene("TownScene");
    }

    // 게임 1층부터 재시작
    void OnRestartFromFloor1()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        // 플레이어 속도 복구
        DungeonPlayerController player = FindObjectOfType<DungeonPlayerController>();
        if (player != null)
        {
            player.RecoverHealth();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetDungeonFloor();  
        }

        PlayerPrefs.SetInt("RecoverHealth", 1);
        SceneManager.LoadScene("MainScene");
    }

    // 최초 화면으로 이동
    void OnReturnToStart()
    {

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        DungeonPlayerController player = FindObjectOfType<DungeonPlayerController>();
        if (player != null)
        {
            player.RecoverHealth();
        }


        SaveGameData();
        SceneManager.LoadScene("StartScene");
    }
    // 데이터 저장
    void SaveGameData()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
        }
    }
}