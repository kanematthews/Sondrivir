using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int slotCount = 8;

    public List<ItemStack> items =
        new List<ItemStack>();

    [Header("Weight")]
    public float currentWeight = 0f;

    private PlayerStats playerStats;

    void Awake()
    {
        playerStats =
            GetComponent<PlayerStats>();
    }

    public bool AddItem(
        ItemData item,
        int amount)
    {
        if (item == null)
        {
            return false;
        }

        // CALCULATE TOTAL WEIGHT
        float addedWeight =
            item.weight * amount;

        // CHECK CAPACITY
        if (
            currentWeight + addedWeight >
            playerStats.capacity)
        {
            Debug.Log(
                "Not enough capacity.");

            return false;
        }

        // TRY STACKING
        if (item.stackable)
        {
            foreach (ItemStack stack in items)
            {
                if (
                    stack.item == item &&
                    stack.amount < item.maxStack)
                {
                    stack.amount += amount;

                    currentWeight +=
                        addedWeight;

                    return true;
                }
            }
        }

        // INVENTORY FULL
        if (items.Count >= slotCount)
        {
            Debug.Log(
                "Inventory Full");

            return false;
        }

        // CREATE NEW STACK
        ItemStack newStack =
            new ItemStack();

        newStack.item =
            item;

        newStack.amount =
            amount;

        items.Add(newStack);

        currentWeight +=
            addedWeight;

        return true;
    }

    public void RemoveItem(
        ItemStack stack)
    {
        if (
            stack == null ||
            stack.item == null)
        {
            return;
        }

        currentWeight -=
            stack.item.weight *
            stack.amount;

        items.Remove(stack);

        currentWeight =
            Mathf.Max(
                0f,
                currentWeight);
    }

    public float GetRemainingCapacity()
    {
        if (playerStats == null)
        {
            return 0f;
        }

        return
            playerStats.capacity -
            currentWeight;
    }
}