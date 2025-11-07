using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SaveData saveData;
    private string savePath;

    [Header("UI Groups")]
    [SerializeField] private GameObject startGroup;
    [SerializeField] private GameObject selectGroup;

    [Header("Start Menu")]
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private TextMeshProUGUI endText;

    [Header("Select Menu")]
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI newGameText;

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

    public void SaveGame()
    {
        if (Inventory.Instance != null)
        {
            saveData.inventoryItems.Clear();

            List<Item> items = Inventory.Instance.GetItems();
            foreach (Item item in items)
            {
                saveData.inventoryItems.Add(new ItemData(item));
            }
        }

        if (ExperienceSystem.Instance != null)
        {
            saveData.currentLevel = ExperienceSystem.Instance.currentLevel; // playerLevel -> currentLevel로 변경
            saveData.currentExp = ExperienceSystem.Instance.currentExp;
            saveData.expToNextLevel = ExperienceSystem.Instance.expToNextLevel;
        }

        string json = JsonUtility.ToJson(saveData, true);
        System.IO.File.WriteAllText(savePath, json);
    }

    public void LoadExperience()
    {
        if (ExperienceSystem.Instance != null)
        {
            ExperienceSystem.Instance.LoadExp(
                saveData.currentExp,
                saveData.expToNextLevel
            );
            ExperienceSystem.Instance.currentLevel = saveData.currentLevel; // playerLevel -> currentLevel로 변경
        }
    }

    public void LoadGame()
    {
        if (System.IO.File.Exists(savePath))
        {
            string json = System.IO.File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
        }
    }

    public void LoadInventory()
    {
        if (Inventory.Instance != null && saveData.inventoryItems != null)
        {
            Inventory.Instance.ClearItems();

            foreach (ItemData itemData in saveData.inventoryItems)
            {
                Inventory.Instance.AddItem(itemData.ToItem());
            }
        }
    }

    // 던전 층수 증가
    public void IncreaseDungeonFloor()
    {
        saveData.dungeonFloor++;
        SaveGame();
    }

    // 던전 층수 리셋
    public void ResetDungeonFloor()
    {
        saveData.dungeonFloor = 1;
        SaveGame();
    }

    public int GetCurrentFloor()
    {
        return saveData.dungeonFloor;
    }

    void Start()
    {
        if (selectGroup != null)
        {
            selectGroup.SetActive(false);
        }

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

    void OnStartTextClicked()
    {
        startGroup.SetActive(false);
        selectGroup.SetActive(true);
    }

    void OnContinueTextClicked()
    {
        if (File.Exists(savePath))
        {
            LoadGame();
            selectGroup.SetActive(false);
            SceneManager.LoadScene("TownScene");
        }
        else
        {
            OnNewGameTextClicked();
        }
    }

    void OnNewGameTextClicked()
    {
        saveData = new SaveData();
        selectGroup.SetActive(false);
        SceneManager.LoadScene("TownScene");
    }

    void OnEndTextClicked()
    {
        SaveGame();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬으로 다시 돌아왔을떄 재연결
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
 
        if (scene.name == "StartScene")
        {
   
            startGroup = GameObject.Find("StartGroup");
            selectGroup = GameObject.Find("SelectGroup");

     
            GameObject startTextObj = GameObject.Find("StartText");
            if (startTextObj != null)
                startText = startTextObj.GetComponent<TextMeshProUGUI>();

            GameObject endTextObj = GameObject.Find("EndText");
            if (endTextObj != null)
                endText = endTextObj.GetComponent<TextMeshProUGUI>();

            GameObject continueTextObj = GameObject.Find("ContinueText");
            if (continueTextObj != null)
                continueText = continueTextObj.GetComponent<TextMeshProUGUI>();

            GameObject newGameTextObj = GameObject.Find("NewGameText");
            if (newGameTextObj != null)
                newGameText = newGameTextObj.GetComponent<TextMeshProUGUI>();


            if (startGroup != null)
            {
                startGroup.SetActive(true);
            }

            if (selectGroup != null)
            {
                selectGroup.SetActive(false);
            }

            AddClickEvent(startText, OnStartTextClicked);
            AddClickEvent(endText, OnEndTextClicked);
            AddClickEvent(continueText, OnContinueTextClicked);
            AddClickEvent(newGameText, OnNewGameTextClicked);
        }
    }

}

[System.Serializable]
public class SaveData
{
    public int score;
    public string playerName;
    public float playTime;

    // 플레이어 레벨 (경험치 레벨)
    public int currentLevel = 1; 
    public int currentExp = 0;
    public int expToNextLevel = 100;
    public float expMultiplier = 1.5f;
    public int baseAttack = 20;
    public int baseHealth = 100;
    public int baseDefense = 0;

    // 던전 층수
    public int dungeonFloor = 1;

    public List<ItemData> inventoryItems = new List<ItemData>();
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int itemType;
    public int value;
    public string description;

    public ItemData(Item item)
    {
        itemName = item.itemName;
        itemType = (int)item.type;
        value = item.value;
        description = item.description;
    }

    public Item ToItem()
    {
        return new Item(itemName, (Item.ItemType)itemType, value, description);
    }


}