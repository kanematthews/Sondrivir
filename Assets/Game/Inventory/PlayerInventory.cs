using UnityEngine;

public class PlayerInventory : Container
{
    private PlayerStats playerStats;

    protected override void Awake()
    {
        base.Awake();

        playerStats =
            GetComponent<PlayerStats>();
    }

    public override bool AddItem(
        ItemData item,
        int amount)
    {
        if (
            item == null ||
            playerStats == null)
        {
            return false;
        }

        float addedWeight =
            item.weight * amount;

        // CAPACITY CHECK
        if (
            GetCurrentWeight() + addedWeight >
            playerStats.capacity)
        {
            Debug.Log(
                "Not enough capacity.");

            return false;
        }

        return base.AddItem(
            item,
            amount);
    }

    public float GetCurrentWeight()
    {
        float totalWeight = 0f;

        foreach (ItemStack stack in slots)
        {
            if (
                stack == null ||
                stack.item == null)
            {
                continue;
            }

            totalWeight +=
                stack.item.weight *
                stack.amount;
        }

        return totalWeight;
    }
}