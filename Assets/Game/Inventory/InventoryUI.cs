using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [Header("UI")]
    public GameObject window;

    public Transform slotContainer;

    public GameObject slotPrefab;

    public TMP_Text weightText;

    private PlayerInventory inventory;

    void Awake()
    {
        instance = this;

        Close();
    }

    void Start()
    {
        inventory =
            FindFirstObjectByType
            <PlayerInventory>();

        Refresh();
    }

    void Update()
    {
        // TOGGLE INVENTORY
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (window.activeSelf)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    public void Open()
    {
        window.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        window.SetActive(false);
    }

    public void Refresh()
    {
        // CLEAR OLD SLOTS
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (inventory == null)
        {
            return;
        }

        // CREATE INVENTORY SLOTS
        // CREATE FIXED INVENTORY SLOTS
        for (int i = 0; i < inventory.slotCount; i++)
        {
            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotContainer);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            // EMPTY SLOT
            if (i >= inventory.items.Count)
            {
                if (
                    slotUI != null &&
                    slotUI.icon != null)
                {
                    slotUI.icon.enabled = false;
                }

                continue;
            }

            ItemStack stack =
                inventory.items[i];

            if (
                slotUI != null &&
                slotUI.icon != null &&
                stack.item != null)
            {
                slotUI.icon.sprite =
                    stack.item.icon;
                    slotUI.icon.enabled = true;

                slotUI.stack =
                    stack;
            }

            LootSlotHover hover =
                slot.GetComponent<LootSlotHover>();

            if (hover != null)
            {
                hover.stack =
                    stack;
            }
        }

        // UPDATE WEIGHT
        PlayerStats stats =
            inventory.GetComponent
            <PlayerStats>();

        if (
            weightText != null &&
            stats != null)
        {
            float remainingCapacity =
                stats.capacity -
                inventory.GetCurrentWeight();

            weightText.text =
                "Capacity: " +
                remainingCapacity
                .ToString("0.0") +
                " / " +
                stats.capacity
                .ToString("0.0");
        }
    }
}