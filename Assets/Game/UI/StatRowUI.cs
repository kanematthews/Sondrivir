using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatRowUI : MonoBehaviour
{
    private TMP_Text labelText;

    private TMP_Text valueText;

    private Button addButton;

    private string statName;

    private PlayerStats playerStats;

    [Header("Spacing")]
    public float startX = 18f;

    public float characterSpacing = 4.5f;

    public float minimumValueX = 70f;

    void Awake()
    {
        TMP_Text[] texts =
            GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            // IGNORE BUTTON TEXT
            if (text.transform.parent.name == "Button")
                continue;

            if (labelText == null)
            {
                labelText = text;
            }
            else if (valueText == null)
            {
                valueText = text;
            }
        }

        addButton =
            GetComponentInChildren<Button>(true);
    }

    public void Setup(
        string label,
        string value,
        PlayerStats stats)
    {
        statName = label;

        playerStats = stats;

        SetRow(label, value);

        RefreshButton();
    }

    public void SetRow(
        string label,
        string value)
    {
        if (
            labelText == null ||
            valueText == null)
        {
            return;
        }

        labelText.text =
            label + ":";

        valueText.text =
            value;

        AdjustSpacing();
    }

    public void SetValue(
        string value)
    {
        if (valueText == null)
            return;

        valueText.text =
            value;
    }

    public void RefreshButton()
    {
        if (addButton == null)
            return;

        bool canUpgrade =
            statName == "Strength" ||
            statName == "Dexterity" ||
            statName == "Intellect" ||
            statName == "Health" ||
            statName == "Mana" ||
            statName == "Capacity" ||
            statName == "HP Regen" ||
            statName == "MP Regen";

        addButton.gameObject.SetActive(
            canUpgrade &&
            playerStats.statPoints > 0);

        addButton.onClick.RemoveAllListeners();

        addButton.onClick.AddListener(
            IncreaseStat);
    }

    void IncreaseStat()
    {
        if (playerStats.statPoints <= 0)
            return;

        switch (statName)
        {
            case "Strength":
                playerStats.strength++;
                break;

            case "Dexterity":
                playerStats.dexterity++;
                break;

            case "Intellect":
                playerStats.intellect++;
                break;

            case "Health":
                playerStats.vitality++;
                break;

            case "Mana":
                playerStats.intellect++;
                break;

            case "HP Regen":
                playerStats.hpRegen++;
                break;

            case "MP Regen":
                playerStats.mpRegen++;
                break;
        }

        playerStats.statPoints--;

        playerStats.RecalculateStats();

        RefreshButton();
    }

    void AdjustSpacing()
    {
        if (
            labelText == null ||
            valueText == null)
        {
            return;
        }

        labelText.ForceMeshUpdate();

        int characterCount =
            labelText.text.Length;

        float targetX =
            startX +
            (characterCount * characterSpacing);

        targetX =
            Mathf.Max(
                targetX,
                minimumValueX);

        RectTransform valueRect =
            valueText.rectTransform;

        Vector2 pos =
            valueRect.anchoredPosition;

        pos.x = targetX;

        valueRect.anchoredPosition =
            pos;
    }
}