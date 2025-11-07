using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BossClearUI : MonoBehaviour
{
    public static BossClearUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private TextMeshProUGUI clearMessageText;

    [Header("Buttons")]
    [SerializeField] private Button returnToTownButton;
    [SerializeField] private Button returnToStartButton;
    [SerializeField] private Button restartFromFloor1Button;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }
    }

    void Start()
    {
        // 버튼 이벤트
        if (returnToTownButton != null)
        {
            returnToTownButton.onClick.AddListener(OnReturnToTown);
        }

        if (returnToStartButton != null)
        {
            returnToStartButton.onClick.AddListener(OnReturnToStart);
        }

        if (restartFromFloor1Button != null)
        {
            restartFromFloor1Button.onClick.AddListener(OnRestartFromFloor1);
        }
    }

    public void ShowClearUI()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지

            if (clearMessageText != null && GameManager.Instance != null)
            {

                int floor = GameManager.Instance.saveData.dungeonFloor;
                clearMessageText.text = $"던전 클리어\n{floor}층 보스 처치 성공!...";
              
            }

        }
    }

    void OnReturnToTown()
    {
      
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        // 시간 재개
        Time.timeScale = 1f;

        SaveGameData();
        SceneManager.LoadScene("TownScene");
    }

    void OnReturnToStart()
    {
 
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        // 시간 재개
        Time.timeScale = 1f;

        SaveGameData();
        SceneManager.LoadScene("StartScene");
    }

    void OnRestartFromFloor1()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        Time.timeScale = 1f;


        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetDungeonFloor();
        }

        PlayerPrefs.SetInt("RecoverHealth", 1);
        SceneManager.LoadScene("MainScene");
    }
    void SaveGameData()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Debug.Log("게임 저장 완료!");
        }
    }
}