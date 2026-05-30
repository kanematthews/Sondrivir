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

    // =========================================
    // AWAKE
    // =========================================

    void Awake()
    {
        TMP_Text[] texts =
            GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (text.transform.parent.name == "Button")
            {
                continue;
            }

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

    // =========================================
    // SETUP
    // =========================================

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

    // =========================================
    // SET ROW
    // =========================================

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
    }

    // =========================================
    // SET VALUE
    // =========================================

    public void SetValue(
        string value)
    {
        if (valueText == null)
        {
            return;
        }

        valueText.text =
            value;
    }

    // =========================================
    // REFRESH BUTTON
    // =========================================

    public void RefreshButton()
    {
        if (addButton == null)
        {
            return;
        }

        bool canUpgrade =
            statName == "Strength"     ||
            statName == "Dexterity"    ||
            statName == "Intelligence" ||
            statName == "Health"       ||
            statName == "Mana"         ||
            statName == "Capacity"     ||
            statName == "HP Regen"     ||
            statName == "MP Regen";

        addButton.gameObject.SetActive(
            canUpgrade &&
            playerStats.statPoints > 0);

        addButton.onClick.RemoveAllListeners();

        addButton.onClick.AddListener(
            IncreaseStat);
    }

    // =========================================
    // INCREASE STAT
    // =========================================

    void IncreaseStat()
    {
        if (playerStats.statPoints <= 0) return;

        playerStats.SpendStatPointServerRpc(statName);

        RefreshButton();
    }

}