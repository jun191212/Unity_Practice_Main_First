using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Inventory")]
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private int maxItems = 20;

    [Header("Stats")]
    private int totalAttackBoost = 0;
    private int totalHealthBoost = 0;
    private int totalDefenseBoost = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ClearItems()
    {
        items.Clear();
        totalAttackBoost = 0;
        totalHealthBoost = 0;
        totalDefenseBoost = 0;
    }
    public bool AddItem(Item item)
    {
        if (items.Count >= maxItems)
        {
      
            return false;
        }

        items.Add(item);
        ApplyItemEffect(item);

        return true;
    }

    void ApplyItemEffect(Item item)
    {
        DungeonPlayerController player = FindObjectOfType<DungeonPlayerController>();

        if (player == null)
        {
     
            return;
        }

        switch (item.type)
        {
            case Item.ItemType.AttackBoost:
                totalAttackBoost += item.value;
                player.AddAttack(item.value);
                break;

            case Item.ItemType.HealthBoost:
                totalHealthBoost += item.value;
                player.IncreaseMaxHealth(item.value);
                break;

            case Item.ItemType.DefenseBoost:
                totalDefenseBoost += item.value;
                break;
        }
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public int GetTotalAttackBoost()
    {
        return totalAttackBoost;
    }

    public int GetTotalHealthBoost()
    {
        return totalHealthBoost;
    }
    public int GetTotalDefenseBoost()
    {
        return totalDefenseBoost;
    }
}