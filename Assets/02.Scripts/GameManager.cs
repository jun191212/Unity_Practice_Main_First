using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SaveData saveData;
    private string savePath;

    [Header("UI Groups")]
    [SerializeField] private GameObject startGroup;      // StartGroup
    [SerializeField] private GameObject selectGroup;     // SelectGroup

    [Header("Start Menu")]
    [SerializeField] private TextMeshProUGUI startText;  // 시작
    [SerializeField] private TextMeshProUGUI endText;    // 종료

    [Header("Select Menu")]
    [SerializeField] private TextMeshProUGUI continueText;  // 이어하기
    [SerializeField] private TextMeshProUGUI newGameText;   // 새로하기

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/savefile.json";
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 선택 화면 숨기기
        if (selectGroup != null)
        {
            selectGroup.SetActive(false);
        }

        // 클릭 이벤트 추가
        AddClickEvent(startText, OnStartTextClicked);
        AddClickEvent(endText, OnEndTextClicked);
        AddClickEvent(continueText, OnContinueTextClicked);
        AddClickEvent(newGameText, OnNewGameTextClicked);
    }

    void AddClickEvent(TextMeshProUGUI textComponent, UnityEngine.Events.UnityAction action)
    {
        if (textComponent == null) return;

        EventTrigger trigger = textComponent.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = textComponent.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { action(); });
        trigger.triggers.Add(entry);
    }

    // 시작 클릭
    void OnStartTextClicked()
    {
        Debug.Log("시작 클릭!");
        startGroup.SetActive(false);
        selectGroup.SetActive(true);
    }

    // 이어하기 클릭
    void OnContinueTextClicked()
    {
        Debug.Log("이어하기!");

        if (File.Exists(savePath))
        {
            LoadGame();
            selectGroup.SetActive(false);

            // TownScene으로 이동
            SceneManager.LoadScene("TownScene");
        }
        else
        {
            Debug.Log("저장 데이터 없음! 새 게임으로 시작합니다.");
            OnNewGameTextClicked();
        }
    }

    // 새로하기 클릭
    void OnNewGameTextClicked()
    {
        Debug.Log("새로하기!");
        saveData = new SaveData();
        selectGroup.SetActive(false);

        // TownScene으로 이동
        SceneManager.LoadScene("TownScene");
    }

    // 종료 클릭
    void OnEndTextClicked()
    {
        Debug.Log("게임 종료!");
        SaveGame();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("게임 저장됨!");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("게임 불러옴!");
        }
        else
        {
            saveData = new SaveData();
            Debug.Log("새 게임 시작!");
        }
    }
}

[System.Serializable]
public class SaveData
{
    public int score;
    public int level;
    public string playerName;
    public float playTime;
}