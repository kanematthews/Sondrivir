using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    [Header("Container")]
    public Container container;

    [Header("UI")]
    public Transform slotParent;

    public GameObject slotPrefab;

    // =====================================
    // START
    // =====================================

    void Start()
    {
        Refresh();
    }

    // =====================================
    // REFRESH
    // =====================================

    public void Refresh()
    {
        // CLEAR OLD SLOTS

        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        // INVALID

        if (
            container == null ||
            container.slots == null)
        {
            return;
        }

        // CREATE SLOTS

        for (int i = 0; i < container.slots.Count; i++)
        {
            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotParent);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            if (slotUI == null)
            {
                continue;
            }

            // SLOT INFO

            slotUI.slotIndex = i;

            slotUI.parentContainer =
                container;

            // REFRESH SLOT

            slotUI.Refresh();
        }

    }
}