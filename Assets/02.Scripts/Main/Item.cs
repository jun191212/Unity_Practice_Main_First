using UnityEngine;

[System.Serializable]
public class Item
{
    public enum ItemType
    {
        AttackBoost,
        HealthBoost,
        DefenseBoost
    }

    public string itemName;
    public ItemType type;
    public int value;
    public Sprite icon;
    public string description;

    public Item(string name, ItemType itemType, int itemValue, string desc)
    {
        itemName = name;
        type = itemType;
        value = itemValue;
        description = desc;
    }
}