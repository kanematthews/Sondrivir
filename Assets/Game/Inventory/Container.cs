using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [Header("Container")]
    public string containerName =
        "Container";

    public int slotCount = 8;

    // FIXED SLOTS
    public List<ItemStack> slots =
        new List<ItemStack>();

    protected virtual void Awake()
    {
        // CREATE EMPTY SLOTS
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(null);
        }
    }

    // =========================================
    // ADD ITEM
    // =========================================

    public virtual bool AddItem(
        ItemData item,
        int amount)
    {
        if (item == null)
        {
            return false;
        }

        // STACK FIRST
        if (item.stackable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                ItemStack stack =
                    slots[i];

                if (
                    stack != null &&
                    stack.item == item)
                {
                    stack.amount += amount;

                    return true;
                }
            }
        }

        // FIND EMPTY SLOT
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
            {
                ItemStack newStack =
                    new ItemStack();

                newStack.item =
                    item;

                newStack.amount =
                    amount;

                // CREATE BAG CONTAINER
                if (item.isContainer)
                {
                    GameObject obj =
                        new GameObject(
                            item.itemName + "_Container");

                    ItemContainerInstance container =
                        obj.AddComponent
                        <ItemContainerInstance>();

                    container.Initialize(item);

                    newStack.containerInstance =
                        container;
                }

                slots[i] =
                    newStack;

                return true;
            }
        }

        return false;
    }

    // =========================================
    // MOVE ITEM
    // =========================================

    public virtual void MoveItem(
        int fromSlot,
        int toSlot)
    {
        // INVALID
        if (
            fromSlot < 0 ||
            toSlot < 0 ||
            fromSlot >= slots.Count ||
            toSlot >= slots.Count)
        {
            return;
        }

        // SAME SLOT
        if (fromSlot == toSlot)
        {
            return;
        }

        ItemStack fromStack =
            slots[fromSlot];

        ItemStack toStack =
            slots[toSlot];

        // NOTHING TO MOVE
        if (fromStack == null)
        {
            return;
        }

        // EMPTY SLOT
        if (toStack == null)
        {
            slots[toSlot] =
                fromStack;

            slots[fromSlot] =
                null;

            return;
        }

        // STACK MERGE
        if (
            fromStack.item ==
            toStack.item &&
            fromStack.item.stackable)
        {
            toStack.amount +=
                fromStack.amount;

            slots[fromSlot] =
                null;

            return;
        }

        // SWAP
        slots[toSlot] =
            fromStack;

        slots[fromSlot] =
            toStack;
    }

    // =========================================
    // REMOVE ITEM
    // =========================================

    public virtual void RemoveItem(
        int slotIndex)
    {
        if (
            slotIndex < 0 ||
            slotIndex >= slots.Count)
        {
            return;
        }

        slots[slotIndex] = null;
    }
}