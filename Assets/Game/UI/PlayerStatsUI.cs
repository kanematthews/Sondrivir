using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;

    public GameObject statRowPrefab;

    public Transform contentParent;

    private Dictionary<string, StatRowUI> rows =
        new Dictionary<string, StatRowUI>();

    // =========================================
    // STAT ORDER
    // =========================================

    private readonly string[] statOrder =
    {
        "Level",
        "EXP",
        "Stat Points",

        "Health",
        "Mana",

        "Damage",
        "Attack Speed",
        "Attack Range",

        "Strength",
        "Dexterity",
        "Intellect",
        "Vitality",

        "Defense",

        "Capacity",

        "HP Regen",
        "MP Regen",

        "Move Speed",

        "Crit Chance",

        "PvP Status"
    };

    // =========================================
    // START
    // =========================================

    void Start()
    {
        GenerateRows();
    }

    // =========================================
    // UPDATE
    // =========================================

    void Update()
    {
        if (playerStats == null)
        {
            return;
        }

        // =====================================
        // PROGRESSION
        // =====================================

        Set(
            "Level",
            playerStats.level.ToString());

        Set(
            "EXP",
            playerStats.experience +
            "/" +
            playerStats.experienceToNextLevel);

        Set(
            "Stat Points",
            playerStats.statPoints.ToString());

        // =====================================
        // RESOURCES
        // =====================================

        Set(
            "Health",
            playerStats.currentHealth +
            "/" +
            playerStats.maxHealth);

        Set(
            "Mana",
            playerStats.currentMana +
            "/" +
            playerStats.maxMana);

        // =====================================
        // COMBAT
        // =====================================

        Set(
            "Damage",
            playerStats.CalculateDamage().ToString());

        Set(
            "Attack Speed",
            playerStats.attackSpeed
            .ToString("F2"));

        Set(
            "Attack Range",
            playerStats.attackRange
            .ToString("F1"));

        // =====================================
        // PRIMARY STATS
        // =====================================

        Set(
            "Strength",
            playerStats.strength.ToString());

        Set(
            "Dexterity",
            playerStats.dexterity.ToString());

        Set(
            "Intellect",
            playerStats.intellect.ToString());

        Set(
            "Vitality",
            playerStats.vitality.ToString());

        // =====================================
        // DEFENSE
        // =====================================

        Set(
            "Defense",
            playerStats.defense.ToString());

        // =====================================
        // CAPACITY
        // =====================================

        PlayerInventory inventory =
            playerStats.GetComponent<PlayerInventory>();

        float currentWeight = 0f;

        if (inventory != null)
        {
            currentWeight =
                inventory.GetCurrentWeight();
        }

        float remainingCapacity =
            playerStats.capacity -
            currentWeight;

        Set(
            "Capacity",
            remainingCapacity.ToString("0.0") +
            "/" +
            playerStats.capacity.ToString("0.0"));

        // =====================================
        // REGEN
        // =====================================

        Set(
            "HP Regen",
            playerStats.hpRegenAmount.ToString());

        Set(
            "MP Regen",
            playerStats.mpRegenAmount.ToString());

        // =====================================
        // MOVEMENT
        // =====================================

        Set(
            "Move Speed",
            playerStats.moveSpeed
            .ToString("F1"));

        // =====================================
        // CRITS
        // =====================================

        Set(
            "Crit Chance",
            (playerStats.critChance * 100f)
            .ToString("F0") + "%");

        // =====================================
        // PVP
        // =====================================

        Set(
            "PvP Status",
            playerStats.pvpEnabled
            ? "ON"
            : "OFF");

        RefreshButtons();
    }

    // =========================================
    // GENERATE ROWS
    // =========================================

    void GenerateRows()
    {
        foreach (string stat in statOrder)
        {
            GameObject rowObj =
                Instantiate(
                    statRowPrefab,
                    contentParent);

            rowObj.name =
                stat + " Row";

            StatRowUI row =
                rowObj.GetComponent<StatRowUI>();

            row.Setup(
                stat,
                "0",
                playerStats);

            rows.Add(stat, row);
        }
    }

    // =========================================
    // SET VALUE
    // =========================================

    void Set(
        string stat,
        string value)
    {
        if (
            rows.ContainsKey(stat) &&
            rows[stat] != null)
        {
            rows[stat].SetValue(value);
        }
    }

    // =========================================
    // REFRESH BUTTONS
    // =========================================

    void RefreshButtons()
    {
        foreach (
            KeyValuePair<string, StatRowUI>
            row in rows)
        {
            row.Value.RefreshButton();
        }
    }
}