using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [Header("UI")]
    public GameObject window;

    public Transform slotContainer;

    public GameObject slotPrefab;

    private PlayerInventory inventory;

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        instance = this;

        Close();
    }

    // =====================================
    // START
    // =====================================

    void Start()
    {
        inventory =
            FindFirstObjectByType
            <PlayerInventory>();

        Refresh();
    }

    // =====================================
    // UPDATE
    // =====================================

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

    // =====================================
    // OPEN
    // =====================================

    public void Open()
    {
        window.SetActive(true);

        Refresh();
    }

    // =====================================
    // CLOSE
    // =====================================

    public void Close()
    {
        window.SetActive(false);
    }

    // =====================================
    // REFRESH
    // =====================================

    public void Refresh()
    {
        // CLEAR OLD

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (inventory == null)
        {
            return;
        }

        // CREATE SLOTS

        for (int i = 0; i < inventory.slotCount; i++)
        {
            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotContainer);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            if (slotUI == null)
            {
                continue;
            }

            // SLOT INFO

            slotUI.slotIndex = i;

            slotUI.parentContainer =
                inventory;

            // REFRESH SLOT

            slotUI.Refresh();
        }
    }
}