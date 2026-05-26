using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EnemyBehaviour
{
    Passive,
    Hostile
}

public class EnemyStats : MonoBehaviour
{
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

    // 1 = once per second
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

    void Start()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();
    }

    // =========================================
    // DAMAGE
    // =========================================

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth);

        Debug.Log(
            enemyName +
            " took " +
            amount +
            " damage.");

        UpdateHealthBar();

        SpawnDamageText(amount);

        // PASSIVE ENEMIES BECOME AGGRESSIVE
        EnemyAI ai =
            GetComponent<EnemyAI>();

        if (ai != null)
        {
            ai.Engage();
        }

        // DEAD
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // =========================================
    // HEALTH BAR
    // =========================================

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth /
                maxHealth;
        }
    }

    // =========================================
    // DAMAGE TEXT
    // =========================================

    void SpawnDamageText(int amount)
    {
        if (
            damageTextPrefab == null ||
            damageTextSpawnPoint == null)
        {
            return;
        }

        // RANDOM OFFSET
        Vector3 randomOffset =
            new Vector3(
                Random.Range(-0.4f, 0.4f),
                Random.Range(0f, 0.3f),
                0f);

        // SPAWN POSITION
        Vector3 spawnPosition =
            damageTextSpawnPoint.position +
            randomOffset;

        GameObject textObj =
            Instantiate(
                damageTextPrefab,
                spawnPosition,
                Quaternion.identity);

        // MAKE TEXT RED
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

    // =========================================
    // LOOT
    // =========================================

    void SpawnLootBag()
    {
        if (lootBagPrefab == null)
        {
            return;
        }

        // SPAWN BAG
        GameObject bagObj =
            Instantiate(
                lootBagPrefab,
                transform.position,
                Quaternion.Euler(
                    90f,
                    0f,
                    0f));

        // GET LOOT BAG
        LootBag lootBag =
            bagObj.GetComponent<LootBag>();

        if (lootBag == null)
        {
            Destroy(bagObj);

            return;
        }

        bool hasLoot = false;

        // ROLL LOOT TABLE
        foreach (LootDrop drop in lootTable)
        {
            double roll =
                Random.Range(0f, 100f);

            if (roll <= drop.dropChance)
            {
                int amount =
                    Random.Range(
                        drop.minAmount,
                        drop.maxAmount + 1);

                bool added =
                    lootBag.AddItem(
                        drop.item,
                        amount);

                if (added)
                {
                    hasLoot = true;
                }
            }
        }

        // DESTROY EMPTY BAG
        if (!hasLoot)
        {
            Destroy(bagObj);
        }
    }

    // =========================================
    // DEATH
    // =========================================

    void Die()
    {
        Debug.Log(
            enemyName + " died.");

        // SPAWN LOOT BAG
        SpawnLootBag();

        // GIVE PLAYER EXP
        PlayerStats player =
            FindFirstObjectByType<PlayerStats>();

        if (player != null)
        {
            player.GainExperience(
                experienceReward);
        }

        gameObject.SetActive(false);

        Invoke(
            nameof(Respawn),
            respawnTime);
    }

    // =========================================
    // RESPAWN
    // =========================================

    void Respawn()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();

        gameObject.SetActive(true);
    }
}