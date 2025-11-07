using UnityEngine;
using UnityEngine.SceneManagement;

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {



        if (PlayerPrefs.GetInt("RecoverHealth", 0) == 1)
        {
            Invoke("RecoverPlayerHealth", 0.1f);
        }
    }


    void RecoverPlayerHealth()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            DungeonPlayerController playerController = player.GetComponent<DungeonPlayerController>();
            if (playerController != null)
            {
                playerController.RecoverHealth();
            }
        }
        PlayerPrefs.SetInt("RecoverHealth", 0);
    }
}