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
    // =========================================
    // BASIC
    // =========================================

    [Header("Basic Info")]
    public string itemID;

    public string itemName;

    [TextArea]
    public string description;

    public ItemType itemType;

    // =========================================
    // CONTAINER
    // =========================================

    [Header("Container")]
    public bool isContainer = false;

    public int containerSlots = 4;

    // =========================================
    // VISUALS
    // =========================================

    [Header("Visuals")]
    public Sprite icon;

    // =========================================
    // ECONOMY
    // =========================================

    [Header("Economy")]
    public int goldValue = 1;

    public float weight = 1f;

    // =========================================
    // STACKING
    // =========================================

    [Header("Stacking")]
    public bool stackable = false;

    public int maxStack = 1;

    // =========================================
    // EQUIPMENT
    // =========================================

    [Header("Equipment")]
    public bool equippable = false;

    public EquipmentSlotType equipmentSlotType;

    public bool twoHanded = false;

    // =========================================
    // WEAPON STATS
    // =========================================

    [Header("Weapon Stats")]
    public int damage = 0;

    public float attackSpeed = 1f;

    public float attackRange = 2f;

    // =========================================
    // ARMOR
    // =========================================

    [Header("Armor")]
    public int armor = 0;

    // =========================================
    // PRIMARY STATS
    // =========================================

    [Header("Primary Stats")]
    public int bonusStrength = 0;

    public int bonusDexterity = 0;

    public int bonusIntellect = 0;

    public int bonusVitality = 0;

    // =========================================
    // RESOURCES
    // =========================================

    [Header("Resources")]
    public int bonusHealth = 0;

    public int bonusMana = 0;

    // =========================================
    // REGEN
    // =========================================

    [Header("Regeneration")]
    public int bonusHPRegen = 0;

    public int bonusMPRegen = 0;

    // =========================================
    // CRITS
    // =========================================

    [Header("Crit")]
    public float bonusCritChance = 0f;

    public float bonusCritDamage = 0f;

    // =========================================
    // MOVEMENT
    // =========================================

    [Header("Movement")]
    public float bonusMoveSpeed = 0f;

    public float bonusCapacity = 0f;

    // =========================================
    // SPECIAL
    // =========================================

    [Header("Special")]
    public bool enablePvp = false;

    // =========================================
    // REQUIREMENTS
    // =========================================

    [Header("Requirements")]
    public int requiredStrength = 0;

    public int requiredDexterity = 0;

    public int requiredIntelligence = 0;
}