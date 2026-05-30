using UnityEngine;
using TMPro;

// =========================================================
// GOLD HUD DISPLAY
// =========================================================
// Reads gold coin count from PlayerInventory and updates
// the HUD gold widget. Falls back to PlayerStats.gold
// if no QuestManager.goldCoinItem is assigned.
// =========================================================

public class GoldHUDDisplay : MonoBehaviour
{
    public TMP_Text goldText;

    private PlayerInventory inventory;
    private QuestManager    questManager;

    private int lastGold = -1;

    private void Start()
    {
        inventory    = FindFirstObjectByType<PlayerInventory>();
        questManager = FindFirstObjectByType<QuestManager>();
    }

    private void Update()
    {
        if (goldText == null) return;

        int gold = GetGold();

        if (gold == lastGold) return;

        lastGold = gold;

        goldText.text = gold.ToString("N0") + " g";
    }

    private int GetGold()
    {
        // If a gold coin item is configured, count it in inventory
        if (questManager != null &&
            questManager.goldCoinItem != null &&
            inventory != null)
        {
            int total = 0;

            foreach (ItemStack stack in inventory.slots)
            {
                if (stack != null &&
                    stack.item == questManager.goldCoinItem)
                {
                    total += stack.amount;
                }
            }

            return total;
        }

        // Fallback — read PlayerStats.gold (currency wallet)
        PlayerStats stats =
            FindFirstObjectByType<PlayerStats>();

        return stats != null ? stats.gold : 0;
    }
}
