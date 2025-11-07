using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemListContent;
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("Stats Display")]
    [SerializeField] private TMP_Text statsText;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.V;

    private bool isOpen = false;

    void Awake()
    {

        Instance = this;
    }

    void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            UpdateInventoryDisplay();
        }
    }

    void UpdateInventoryDisplay()
    {
        if (itemListContent == null)
        {
            return;
        }

        // 기존 아이템 슬롯 제거
        foreach (Transform child in itemListContent)
        {
            Destroy(child.gameObject);
        }

        // 인벤토리 아이템 표시
        if (Inventory.Instance != null)
        {
            List<Item> items = Inventory.Instance.GetItems();

            foreach (Item item in items)
            {
                if (itemSlotPrefab != null)
                {
                    GameObject slot = Instantiate(itemSlotPrefab, itemListContent);
                    TMP_Text itemText = slot.GetComponentInChildren<TMP_Text>();

                    if (itemText != null)
                    {
                        itemText.text = $"{item.itemName} (+{item.value})";
                    }
                }
            }

            UpdateStatsDisplay();
        }
    }

    void UpdateStatsDisplay()
    {
        if (statsText != null && Inventory.Instance != null)
        {
            int attack = Inventory.Instance.GetTotalAttackBoost();
            int health = Inventory.Instance.GetTotalHealthBoost();
            int defense = Inventory.Instance.GetTotalDefenseBoost();

            statsText.text = $"총 보너스:\n공격력 +{attack}\n체력 +{health}\n방어력 +{defense}";
        }
    }
}