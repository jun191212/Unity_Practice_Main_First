using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperienceSystem : MonoBehaviour
{
    public static ExperienceSystem Instance { get; private set; }

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    [SerializeField] private float expMultiplier = 1.5f;

    [Header("Stats")]
    [SerializeField] private int baseAttack = 20;
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int baseDefense = 0;

    [Header("UI")]
    [Tooltip("씬에서 이름으로 UI찾기")]
    [SerializeField] private bool autoFindUI = true;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text floorText;

    [Header("Level Up Effect")]
    [SerializeField] private GameObject levelUpEffectPrefab;

    private DungeonPlayerController playerController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
  
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 바뀔 때마다 UI 다시 찾기
        if (autoFindUI)
        {
            FindUIElements();
        }
        UpdateUI();
    }
    void Start()
    {
        playerController = GetComponent<DungeonPlayerController>();

        LoadDataFromGameManager();

        if (autoFindUI)
        {
            FindUIElements();
        }

        UpdateUI();
    }

    void LoadDataFromGameManager()
    {
        if (GameManager.Instance != null)
        {
            SaveData sd = GameManager.Instance.saveData;

            currentLevel = sd.currentLevel;
            currentExp = sd.currentExp;
            expToNextLevel = sd.expToNextLevel;
            expMultiplier = sd.expMultiplier;
            baseAttack = sd.baseAttack;
            baseHealth = sd.baseHealth;
            baseDefense = sd.baseDefense;

            if (playerController != null)
            {
                playerController.SetStats(baseAttack, baseHealth, baseDefense);
            }
        }
    }

    void SaveDataToGameManager()
    {
        if (GameManager.Instance != null)
        {
            SaveData sd = GameManager.Instance.saveData;

            sd.currentLevel = currentLevel;
            sd.currentExp = currentExp;
            sd.expToNextLevel = expToNextLevel;
            sd.expMultiplier = expMultiplier;
            sd.baseAttack = baseAttack;
            sd.baseHealth = baseHealth;
            sd.baseDefense = baseDefense;
        }
    }

    void FindUIElements()
    {
        if (levelText == null)
        {
            GameObject levelTextObj = GameObject.Find("LevelText");
            if (levelTextObj != null)
            {
                levelText = levelTextObj.GetComponent<TMP_Text>();
            }
        }

        if (expText == null)
        {
            GameObject expTextObj = GameObject.Find("ExpText");
            if (expTextObj != null)
            {
                expText = expTextObj.GetComponent<TMP_Text>();
            }
        }

        if (hpText == null)
        {
            GameObject hpTextObj = GameObject.Find("HPText");
            if (hpTextObj != null)
            {
                hpText = hpTextObj.GetComponent<TMP_Text>();
            }
        }

        if (floorText == null)
        {
            GameObject floorTextObj = GameObject.Find("FloorText");
            if (floorTextObj != null)
            {
                floorText = floorTextObj.GetComponent<TMP_Text>();
            }
        }
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        UpdateUI();
        SaveDataToGameManager();

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expMultiplier);

        ShowLevelUpEffect();
        IncreaseStats();
        SaveDataToGameManager();

        if (LevelUpChoiceUI.Instance != null)
        {
            LevelUpChoiceUI.Instance.ShowChoices();
        }

        UpdateUI();
    }

    void IncreaseStats()
    {
        baseAttack += 5;
        baseHealth += 20;
        baseDefense += 2;

        if (playerController != null)
        {
            playerController.SetStats(baseAttack, baseHealth, baseDefense);
        }
    }

    void ShowLevelUpEffect()
    {
        if (levelUpEffectPrefab != null)
        {
            GameObject effect = Instantiate(levelUpEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);
            Destroy(effect, 2f);
        }
    }


    public void LoadExp(int exp, int expNeeded)
    {
        currentExp = exp;
        expToNextLevel = expNeeded;
        UpdateUI();
    }

    public void RefreshUI()
    {
        if (autoFindUI)
        {
            FindUIElements();
        }

        UpdateUI();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetAttack()
    {
        return baseAttack;
    }

    public int GetDefense()
    {
        return baseDefense;
    }

    void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = $"LV.{currentLevel}";
        }

        if (expText != null)
        {
            expText.text = $"EXP: {currentExp}/{expToNextLevel}";
        }

        // HP 업데이트
        if (hpText != null && playerController != null)
        {
            hpText.text = $"HP: {playerController.GetCurrentHealth()}/{playerController.GetMaxHealth()}";
        }

        // Floor 업데이트
        if (floorText != null && GameManager.Instance != null)
        {
            floorText.text = $"Floor: {GameManager.Instance.GetCurrentFloor()}";
        }
    }

    public void UpdateHealthUI()
    {
        if (hpText != null && playerController != null)
        {
            hpText.text = $"HP: {playerController.GetCurrentHealth()}/{playerController.GetMaxHealth()}";
        }
    }

}