using System;
using UnityEngine;

[Serializable]
public class LootDrop
{
    public ItemData item;

    [Header("Drop Chance (%)")]

    // SUPPORTS:
    // 100
    // 50
    // 1
    // 0.5
    // 0.01
    // ETC

    [Min(0f)]
    public float dropChance = 100f;

    [Header("Amount")]
    public int minAmount = 1;

    public int maxAmount = 1;
}