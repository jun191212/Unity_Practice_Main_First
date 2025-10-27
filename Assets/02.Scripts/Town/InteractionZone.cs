using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InteractionZone : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName = "MainScene"; // 이동할 씬 이름

    [Header("UI Settings")]
    [SerializeField] private GameObject interactionUI; // "F키를 눌러 입장" UI
    [SerializeField] private TextMeshProUGUI interactionText; // 텍스트
    [SerializeField] private string interactionMessage = "F키를 눌러 입장";

    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.F;

    private bool playerInZone = false;

    void Start()
    {
        // UI 숨기기
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        // 텍스트 설정
        if (interactionText != null)
        {
            interactionText.text = interactionMessage;
        }
    }

    void Update()
    {
        // 플레이어가 영역 안에 있고 F키를 누르면
        if (playerInZone && Input.GetKeyDown(interactionKey))
        {
            LoadScene();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            // UI 표시
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }

            Debug.Log("상호작용 가능!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 나갔는지 확인
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            // UI 숨기기
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }

            Debug.Log("상호작용 불가능");
        }
    }

    void LoadScene()
    {
        Debug.Log($"{targetSceneName}으로 이동!");

        // 게임 저장 (선택사항)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
        }

        // 씬 이동
        SceneManager.LoadScene(targetSceneName);
    }

    // 영역 시각화 (디버깅용)
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // 초록색 반투명

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
    }
}