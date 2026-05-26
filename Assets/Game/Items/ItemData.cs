using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Bag,
    Quest,
    Misc
}

[CreateAssetMenu(
    fileName = "New Item",
    menuName = "MMO/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemID;

    public string itemName;

    [TextArea]
    public string description;

    public ItemType itemType;

    public ItemRarity rarity;

    [Header("Visuals")]
    public Sprite icon;

    [Header("Economy")]
    public int goldValue = 1;

    public float weight = 1f;

    [Header("Stacking")]
    public bool stackable = false;

    public int maxStack = 1;

    [Header("Equipment")]
    public bool equippable = false;

    public bool twoHanded = false;

    [Header("Weapon Stats")]
    public int damage = 0;

    public float attackSpeed = 0f;

    [Header("Requirements")]
    public int requiredStrength = 0;

    public int requiredDexterity = 0;

    public int requiredIntelligence = 0;
}