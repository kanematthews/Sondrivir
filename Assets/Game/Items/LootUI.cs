using UnityEngine;

public class LootUI : MonoBehaviour
{
    public static LootUI instance;

    [Header("UI")]
    public GameObject window;

    public Transform slotContainer;

    public GameObject slotPrefab;

    private LootBag currentBag;

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        instance = this;

        Close();
    }

    // =====================================
    // OPEN
    // =====================================

    public void Open(
        LootBag bag)
    {
        currentBag = bag;

        window.SetActive(true);

        Refresh();
    }

    // =====================================
    // CLOSE
    // =====================================

    public void Close()
    {
        currentBag = null;

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

        // INVALID

        if (currentBag == null)
        {
            return;
        }

        // CREATE SLOTS

        for (int i = 0; i < currentBag.slots.Count; i++)
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
                currentBag;

            // REFRESH SLOT

            slotUI.Refresh();
        }
    }
}