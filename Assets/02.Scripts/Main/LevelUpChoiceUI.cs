using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpChoiceUI : MonoBehaviour
{
    public static LevelUpChoiceUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject[] choiceButtons;

    [Header("Button Content")]
    [SerializeField] private TextMeshProUGUI[] itemNameTexts;
    [SerializeField] private TextMeshProUGUI[] itemDescTexts;
    [SerializeField] private Image[] itemIcons;

    [Header("Settings")]
    [SerializeField] private int numberOfChoices = 3;

    private List<Item> currentChoices;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }

    void Start()
    {
        SetupButtons();
    }

    void SetupButtons()
    {
        if (choiceButtons == null || choiceButtons.Length == 0)
        {
            return;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null) continue;

            int index = i; 

            Button btn = choiceButtons[i].GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    OnChoiceSelected(index);
                });


            }
        }
    }

    public void ShowChoices()
    {
        if (choicePanel == null)
        {

            return;
        }

        Time.timeScale = 0f;

        // 랜덤 아이템 생성
        if (ItemDatabase.Instance != null)
        {
            currentChoices = ItemDatabase.Instance.GetRandomItems(numberOfChoices);

        }
        else
        {

            return;
        }

        // UI 업데이트
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentChoices.Count)
            {
                choiceButtons[i].SetActive(true);
                UpdateChoiceButton(i, currentChoices[i]);
            }
            else
            {
                choiceButtons[i].SetActive(false);
            }
        }

        choicePanel.SetActive(true);
    }

    void UpdateChoiceButton(int index, Item item)
    {
        if (itemNameTexts[index] != null)
        {
            itemNameTexts[index].text = item.itemName;
        }

        if (itemDescTexts[index] != null)
        {
            itemDescTexts[index].text = item.description;
        }

        if (itemIcons[index] != null && item.icon != null)
        {
            itemIcons[index].sprite = item.icon;
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {


        if (currentChoices == null || currentChoices.Count == 0)
        {

            return;
        }

        if (choiceIndex < 0 || choiceIndex >= currentChoices.Count)
        {

            return;
        }

        Item selectedItem = currentChoices[choiceIndex];


        // 인벤토리에 아이템 추가
        if (Inventory.Instance != null)
        {
            bool added = Inventory.Instance.AddItem(selectedItem);

        }

        HideChoices();
    }

    public void HidePanel()
    {
        if (choicePanel != null && choicePanel.activeSelf)
        {
            choicePanel.SetActive(false);
            Time.timeScale = 1f;

        }
    }

    void HideChoices()
    {
        HidePanel();
    }
}