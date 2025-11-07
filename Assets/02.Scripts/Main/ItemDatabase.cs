using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("Item Pool")]
    public List<Item> allItems = new List<Item>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeItems();
        }
    }

    void InitializeItems()
    {
        allItems = new List<Item>
        {
            // 공격력 아이템
            new Item("낡은 검", Item.ItemType.AttackBoost, 3, "공격력 +3"),
            new Item("강철 검", Item.ItemType.AttackBoost, 5, "공격력 +5"),
            new Item("마법 검", Item.ItemType.AttackBoost, 10, "공격력 +10"),
            
            // 체력 아이템
            new Item("작은 물약", Item.ItemType.HealthBoost, 10, "최대 체력 +10"),
            new Item("큰 물약", Item.ItemType.HealthBoost, 20, "최대 체력 +20"),
            new Item("생명의 결정", Item.ItemType.HealthBoost, 30, "최대 체력 +30"),
            
            // 방어력 아이템
            new Item("가죽 갑옷", Item.ItemType.DefenseBoost, 3, "방어력 +3"),
            new Item("강철 갑옷", Item.ItemType.DefenseBoost, 5, "방어력 +5"),
            new Item("미스릴 갑옷", Item.ItemType.DefenseBoost, 10, "방어력 +10")
        };
    }

    public List<Item> GetRandomItems(int count)
    {
        List<Item> randomItems = new List<Item>();
        List<Item> availableItems = new List<Item>(allItems);

        for (int i = 0; i < count && availableItems.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            randomItems.Add(availableItems[randomIndex]);
            availableItems.RemoveAt(randomIndex);
        }

        return randomItems;
    }
}