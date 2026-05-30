using UnityEngine;

[System.Serializable]
public class ChestLootEntry
{
    public ItemData item;

    public int amount = 1;
}

public class Chest : Container
{
    [Header("Chest Loot")]
    public ChestLootEntry[] startingLoot;

    public bool opened = false;

    // =====================================
    // AWAKE
    // =====================================

    protected override void Awake()
    {
        if (slotCount < startingLoot.Length)
            slotCount = startingLoot.Length;

        base.Awake();

        containerName = "Chest";

        PopulateLoot();
    }

    // =====================================
    // POPULATE LOOT
    // =====================================

    void PopulateLoot()
    {
        if (startingLoot == null) return;

        foreach (ChestLootEntry entry in startingLoot)
        {
            if (entry == null || entry.item == null)
            {
                continue;
            }

            AddItem(entry.item, entry.amount);
        }
    }
}
