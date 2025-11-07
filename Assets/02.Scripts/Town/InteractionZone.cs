using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InteractionZone : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName = "MainScene";

    [Header("UI Settings")]
    [SerializeField] private GameObject interactionUI; 
    [SerializeField] private TextMeshProUGUI interactionText; 
    [SerializeField] private string interactionMessage = "F키를 눌러 입장";

    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.F;

    private bool playerInZone = false;

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
        
        if (playerInZone && Input.GetKeyDown(interactionKey))
        {
            LoadScene();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            playerInZone = true;

 
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
            playerInZone = false;

            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }

        }
    }

    void LoadScene()
    {

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
        }

        SceneManager.LoadScene(targetSceneName);
    }

   void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); 

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
    }
}