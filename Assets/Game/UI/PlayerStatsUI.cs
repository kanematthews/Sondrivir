using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;

    public GameObject statRowPrefab;

    public Transform contentParent;

    public Transform hudRoot;

    private TMP_Text levelUpNotificationText;

    private Dictionary<string, StatRowUI> rows =
        new Dictionary<string, StatRowUI>();

    private bool showingNotification = false;

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

        CreateLevelUpNotification();
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

        Set(
            "Capacity",
            playerStats.capacity.ToString());

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

        // SHOW LEVEL UP MESSAGE
        if (
            playerStats.statPoints > 0 &&
            !showingNotification)
        {
            StartCoroutine(
                ShowLevelUpNotification());
        }
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

    void CreateLevelUpNotification()
    {
        if (hudRoot == null)
        {
            Debug.LogWarning(
                "HUDRoot not assigned.");

            return;
        }

        GameObject notificationObj =
            new GameObject(
                "LevelUpNotification");

        notificationObj.transform.SetParent(
            hudRoot,
            false);

        RectTransform rect =
            notificationObj
            .AddComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.anchoredPosition =
            new Vector2(0, 120);

        rect.sizeDelta =
            new Vector2(700, 60);

        levelUpNotificationText =
            notificationObj
            .AddComponent<TextMeshProUGUI>();

        levelUpNotificationText.text =
            "";

        levelUpNotificationText.fontSize =
            20;

        levelUpNotificationText.alignment =
            TextAlignmentOptions.Center;

        levelUpNotificationText.color =
            new Color32(
                255,
                230,
                120,
                255);

        // OUTLINE
        levelUpNotificationText
            .fontMaterial
            .EnableKeyword("OUTLINE_ON");

        levelUpNotificationText
            .outlineWidth = 0.2f;

        levelUpNotificationText
            .outlineColor = Color.black;

        notificationObj.SetActive(false);
    }

    IEnumerator ShowLevelUpNotification()
    {
        showingNotification = true;

        if (levelUpNotificationText != null)
        {
            levelUpNotificationText
                .gameObject
                .SetActive(true);

            levelUpNotificationText.text =
                "You have "
                + playerStats.statPoints
                + " stat points to spend!";

            yield return
                new WaitForSeconds(15f);

            levelUpNotificationText
                .gameObject
                .SetActive(false);
        }

        showingNotification = false;
    }
}