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

    private readonly string[] statOrder =
    {
        "Level",
        "EXP",
        "Stat Points",
        "Health",
        "Mana",
        "Damage",
        "Strength",
        "Dexterity",
        "Intellect",
        "Defense",
        "Capacity",
        "Range",
        "HP Regen",
        "MP Regen",
        "Atk Speed",
        "Move Speed",
        "PvP Status"
    };

    void Start()
    {
        GenerateRows();
    }

    void Update()
    {
        if (playerStats == null)
            return;

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

        Set(
            "Damage",
            playerStats.CalculateDamage()
            .ToString());

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
            "Defense",
            playerStats.defense.ToString());

        PlayerInventory inventory =
        playerStats.GetComponent
        <PlayerInventory>();

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

        Set(
            "Range",
            playerStats.attackRange
            .ToString("F1"));

        Set(
            "HP Regen",
            playerStats.hpRegen.ToString());

        Set(
            "MP Regen",
            playerStats.mpRegen.ToString());

        Set(
            "Atk Speed",
            playerStats.attackSpeed
            .ToString("F1"));

        Set(
            "Move Speed",
            playerStats.moveSpeed
            .ToString("F1"));

        Set(
            "PvP Status",
            playerStats.pvpEnabled
            ? "ON"
            : "OFF");

        RefreshButtons();
    }

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

    void Set(
        string stat,
        string value)
    {
        if (rows.ContainsKey(stat))
        {
            rows[stat].SetValue(value);
        }
    }

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