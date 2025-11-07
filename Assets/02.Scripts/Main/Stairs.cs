using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Stairs : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private string interactionMessage = "F키: 다음 층으로";

    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.F;

    private bool playerNearby = false;

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (interactionText != null)
        {
            interactionText.text = interactionMessage;
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(interactionKey))
        {
            GoToNextFloor();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    void GoToNextFloor()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 던전 층수 증가
        GameManager.Instance.IncreaseDungeonFloor();

        // 씬 로드 콜백 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 씬 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // UI 갱신
        if (ExperienceSystem.Instance != null)
        {
            ExperienceSystem.Instance.RefreshUI();
        }
    }
}