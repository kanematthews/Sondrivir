using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EnemyBehaviour
{
    Passive,
    Hostile
}

public class EnemyStats : NetworkBehaviour
{
    // =====================================
    // NETWORK VARIABLES
    // =====================================

    public NetworkVariable<int> netHealth =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    // =====================================
    // INSPECTOR
    // =====================================

    [Header("Enemy Info")]
    public string enemyName = "Skeleton";

    [Header("Behaviour")]
    public EnemyBehaviour behaviour =
        EnemyBehaviour.Hostile;

    public float aggroRange = 8f;

    [Header("Combat")]
    public int maxHealth = 250;

    public int currentHealth;

    public int damage = 5;

    public float attackRange = 2f;

    public float attackSpeed = 1f;

    [Header("Rewards")]
    public int experienceReward = 25;

    [Header("Loot")]
    public GameObject lootBagPrefab;

    public List<LootDrop> lootTable =
        new List<LootDrop>();

    [Header("UI")]
    public Image healthFill;

    [Header("Damage Text")]
    public GameObject damageTextPrefab;

    public Transform damageTextSpawnPoint;

    [Header("Respawn")]
    public float respawnTime = 5f;

    // =====================================
    // ON NETWORK SPAWN
    // =====================================

    public override void OnNetworkSpawn()
    {
        netHealth.OnValueChanged +=
            OnHealthChanged;

        if (IsServer)
        {
            currentHealth    = maxHealth;
            netHealth.Value  = maxHealth;
        }
        else
        {
            currentHealth = netHealth.Value;
        }

        UpdateHealthBar();
    }

    public override void OnNetworkDespawn()
    {
        netHealth.OnValueChanged -=
            OnHealthChanged;
    }

    void OnHealthChanged(int prev, int next)
    {
        currentHealth = next;
        UpdateHealthBar();
    }

    // =====================================
    // START
    // =====================================

    void Start()
    {
        if (!IsServer) return;

        currentHealth   = maxHealth;
        netHealth.Value = maxHealth;

        UpdateHealthBar();
    }

    // =====================================
    // TAKE DAMAGE (server only)
    // =====================================

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        int newHealth =
            Mathf.Clamp(
                netHealth.Value - amount,
                0,
                maxHealth);

        netHealth.Value = newHealth;

        currentHealth = newHealth;

        UpdateHealthBar();

        SpawnDamageTextClientRpc(amount);

        // Passive enemies become aggressive
        EnemyAI ai = GetComponent<EnemyAI>();

        if (ai != null) ai.Engage();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // =====================================
    // HEALTH BAR
    // =====================================

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth / maxHealth;
        }
    }

    // =====================================
    // DAMAGE TEXT (all clients)
    // =====================================

    [ClientRpc]
    void SpawnDamageTextClientRpc(int amount)
    {
        if (
            damageTextPrefab == null ||
            damageTextSpawnPoint == null)
        {
            return;
        }

        Vector3 offset =
            new Vector3(
                Random.Range(-0.4f, 0.4f),
                Random.Range(0f, 0.3f),
                0f);

        GameObject textObj =
            Instantiate(
                damageTextPrefab,
                damageTextSpawnPoint.position +
                    offset,
                Quaternion.identity);

        TMPro.TextMeshPro text =
            textObj.GetComponent
            <TMPro.TextMeshPro>();

        if (text != null)
        {
            text.color = Color.red;
        }

        DamageText damageText =
            textObj.GetComponent<DamageText>();

        if (damageText != null)
        {
            damageText.SetDamage(amount);
        }
    }

    // =====================================
    // LOOT (server only)
    // =====================================

    void SpawnLootBag()
    {
        if (!IsServer) return;

        if (lootBagPrefab == null) return;

        GameObject bagObj =
            Instantiate(
                lootBagPrefab,
                transform.position,
                Quaternion.Euler(90f, 0f, 0f));

        NetworkObject netObj =
            bagObj.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Destroy(bagObj);
            return;
        }

        LootBag lootBag =
            bagObj.GetComponent<LootBag>();

        if (lootBag == null)
        {
            Destroy(bagObj);
            return;
        }

        bool hasLoot = false;

        foreach (LootDrop drop in lootTable)
        {
            float roll = Random.Range(0f, 100f);

            if (roll > drop.dropChance) continue;

            int amount =
                Random.Range(
                    drop.minAmount,
                    drop.maxAmount + 1);

            ItemStack stack = new ItemStack();
            stack.item   = drop.item;
            stack.amount = amount;

            bool canRollRarity =
                drop.item.itemType ==
                    ItemType.Weapon ||
                drop.item.itemType ==
                    ItemType.Armor  ||
                drop.item.itemType ==
                    ItemType.Bag;

            stack.rarity =
                canRollRarity
                    ? RollItemRarity()
                    : ItemRarity.Common;

            if (lootBag.AddItem(stack))
            {
                hasLoot = true;
            }
        }

        if (!hasLoot)
        {
            Destroy(bagObj);
            return;
        }

        netObj.Spawn();
    }

    // =====================================
    // ITEM RARITY
    // =====================================

    ItemRarity RollItemRarity()
    {
        // 95% — Common
        if (Random.Range(0f, 100f) > 5f)
            return ItemRarity.Common;

        float roll = Random.Range(0f, 100f);

        if (roll < 75f) return ItemRarity.Uncommon;
        if (roll < 95f) return ItemRarity.Rare;
        if (roll < 99f) return ItemRarity.Epic;

        return ItemRarity.Legendary;
    }

    // =====================================
    // DIE (server only)
    // =====================================

    void Die()
    {
        if (!IsServer) return;

        Debug.Log(enemyName + " died.");

        SpawnLootBag();

        // Give nearest player experience
        PlayerStats player =
            FindFirstObjectByType<PlayerStats>();

        if (player != null)
        {
            player.GainExperience(experienceReward);
        }

        gameObject.SetActive(false);

        Invoke(nameof(Respawn), respawnTime);
    }

    // =====================================
    // RESPAWN
    // =====================================

    void Respawn()
    {
        if (!IsServer) return;

        currentHealth   = maxHealth;
        netHealth.Value = maxHealth;

        UpdateHealthBar();

        gameObject.SetActive(true);
    }
}
