using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    // =====================================
    // NETWORK VARIABLES
    // Synced to all clients
    // =====================================

    public NetworkVariable<int> netHealth =
        new NetworkVariable<int>(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public NetworkVariable<int> netMaxHealth =
        new NetworkVariable<int>(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public NetworkVariable<int> netMana =
        new NetworkVariable<int>(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public NetworkVariable<int> netMaxMana =
        new NetworkVariable<int>(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public NetworkVariable<int> netLevel =
        new NetworkVariable<int>(
            1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    // =====================================
    // PROGRESSION (local)
    // =====================================

    [Header("Progression")]
    public int level = 1;

    public int experience = 0;

    public int experienceToNextLevel = 100;

    public int statPoints = 0;

    // =====================================
    // BASE STATS
    // =====================================

    [Header("Base Stats")]
    public int baseStrength = 1;

    public int baseDexterity = 1;

    public int baseIntelligence = 1;

    public int baseHealth = 0;

    public int baseMana = 0;

    public int baseCapacity = 0;

    public int baseHPRegen = 0;

    public int baseMPRegen = 0;

    // =====================================
    // FINAL STATS (calculated)
    // =====================================

    [Header("Final Stats")]
    public int strength;

    public int dexterity;

    public int intelligence;

    public int magicDamage;

    // =====================================
    // COMBAT
    // =====================================

    [Header("Combat")]
    public int baseDamage = 1;

    public int defense = 0;

    public float attackSpeed = 1f;

    public float attackRange = 2f;

    public float critChance = 0f;

    public float critDamage = 1.5f;

    // =====================================
    // MOVEMENT
    // =====================================

    [Header("Movement")]
    public float moveSpeed = 5f;

    public float capacity = 50f;

    // =====================================
    // REGEN
    // =====================================

    [Header("Regeneration")]
    public int hpRegenAmount = 1;

    public int mpRegenAmount = 1;

    public float hpRegenInterval = 30f;

    public float mpRegenInterval = 10f;

    private float hpTimer;

    private float mpTimer;

    // =====================================
    // RESOURCES (local mirror of net vars)
    // =====================================

    [Header("Resources")]
    public int maxHealth;

    public int currentHealth;

    public int maxMana;

    public int currentMana;

    // =====================================
    // SPECIAL
    // =====================================

    public bool pvpEnabled = false;

    // =====================================
    // CURRENCY
    // =====================================

    [Header("Currency")]
    public int gold = 0;

    // =====================================
    // UI
    // =====================================

    [Header("UI")]
    public Image healthFill;

    public Image manaFill;

    // =====================================
    // REFERENCES
    // =====================================

    private EquipmentContainer equipment;

    // =====================================
    // ON NETWORK SPAWN
    // =====================================

    public override void OnNetworkSpawn()
    {
        netHealth.OnValueChanged    += OnHealthChanged;
        netMaxHealth.OnValueChanged += OnMaxHealthChanged;
        netMana.OnValueChanged      += OnManaChanged;
        netMaxMana.OnValueChanged   += OnMaxManaChanged;
        netLevel.OnValueChanged     += OnLevelChanged;

        if (IsServer)
        {
            equipment = GetComponent<EquipmentContainer>();

            RecalculateStats();

            netHealth.Value    = maxHealth;
            netMaxHealth.Value = maxHealth;
            netMana.Value      = maxMana;
            netMaxMana.Value   = maxMana;
            netLevel.Value     = level;
        }
    }

    // =====================================
    // ON NETWORK DESPAWN
    // =====================================

    public override void OnNetworkDespawn()
    {
        netHealth.OnValueChanged    -= OnHealthChanged;
        netMaxHealth.OnValueChanged -= OnMaxHealthChanged;
        netMana.OnValueChanged      -= OnManaChanged;
        netMaxMana.OnValueChanged   -= OnMaxManaChanged;
        netLevel.OnValueChanged     -= OnLevelChanged;
    }

    // =====================================
    // NET VAR CALLBACKS
    // =====================================

    void OnHealthChanged(int prev, int next)
    {
        currentHealth = next;
        UpdateHealthUI();
    }

    void OnMaxHealthChanged(int prev, int next)
    {
        maxHealth = next;
        UpdateHealthUI();
    }

    void OnManaChanged(int prev, int next)
    {
        currentMana = next;
        UpdateManaUI();
    }

    void OnMaxManaChanged(int prev, int next)
    {
        maxMana = next;
        UpdateManaUI();
    }

    void OnLevelChanged(int prev, int next)
    {
        level = next;
    }

    // =====================================
    // START
    // =====================================

    void Start()
    {
        if (!IsServer) return;

        equipment = GetComponent<EquipmentContainer>();

        RecalculateStats();

        currentHealth = maxHealth;
        currentMana   = maxMana;

        UpdateHealthUI();
        UpdateManaUI();
    }

    // =====================================
    // UPDATE
    // =====================================

    void Update()
    {
        if (!IsServer) return;

        HandleRegeneration();

        if (IsOwner)
        {
            UpdateHealthUI();
            UpdateManaUI();
        }
    }

    // =====================================
    // REGEN
    // =====================================

    void HandleRegeneration()
    {
        hpTimer += Time.deltaTime;

        if (hpTimer >= hpRegenInterval)
        {
            hpTimer = 0f;
            Heal(hpRegenAmount);
        }

        mpTimer += Time.deltaTime;

        if (mpTimer >= mpRegenInterval)
        {
            mpTimer = 0f;
            RestoreMana(mpRegenAmount);
        }
    }

    // =====================================
    // RECALCULATE STATS
    // =====================================

    public void RecalculateStats()
    {
        strength     = baseStrength;
        dexterity    = baseDexterity;
        intelligence = baseIntelligence;

        baseDamage  = 1;
        defense     = 0;
        attackSpeed = 1f;
        attackRange = 2f;
        critChance  = 0f;
        critDamage  = 1.5f;

        moveSpeed = 5f;
        capacity  = 50f + (baseCapacity * 20f);

        hpRegenAmount = 1 + baseHPRegen;
        mpRegenAmount = 1 + baseMPRegen;

        pvpEnabled = false;

        maxHealth = 100 + (baseHealth * 10);
        maxMana   = 100 + (intelligence * 15) + (baseMana * 10);

        if (equipment != null)
        {
            foreach (ItemStack stack in equipment.slots)
            {
                if (stack == null || stack.item == null)
                    continue;

                ItemData item = stack.item;

                strength     += item.bonusStrength;
                dexterity    += item.bonusDexterity;
                intelligence += item.bonusIntellect;

                defense += item.armor;

                if (item.equipmentSlotType ==
                    EquipmentSlotType.MainHand)
                {
                    baseDamage  = item.damage;
                    attackSpeed = item.attackSpeed;
                    attackRange = item.attackRange;
                }

                maxHealth += item.bonusHealth;
                maxMana   += item.bonusMana;

                hpRegenAmount += item.bonusHPRegen;
                mpRegenAmount += item.bonusMPRegen;

                moveSpeed += item.bonusMoveSpeed;
                capacity  += item.bonusCapacity;

                if (item.enablePvp)
                {
                    pvpEnabled = true;
                }
            }
        }

        // PRIMARY STAT EFFECTS

        baseDamage =
            (int)(baseDamage * (1f + strength * 0.02f));

        attackSpeed += dexterity * 0.02f;
        moveSpeed   += dexterity * 0.05f;

        magicDamage = intelligence * 3;

        // CLAMP CURRENT

        currentHealth =
            Mathf.Clamp(currentHealth, 0, maxHealth);

        currentMana =
            Mathf.Clamp(currentMana, 0, maxMana);

        // SYNC TO NET VARS (server only)

        if (IsServer)
        {
            netMaxHealth.Value = maxHealth;
            netMaxMana.Value   = maxMana;
            netLevel.Value     = level;

            netHealth.Value =
                Mathf.Clamp(
                    netHealth.Value, 0, maxHealth);

            netMana.Value =
                Mathf.Clamp(
                    netMana.Value, 0, maxMana);
        }

        UpdateHealthUI();
        UpdateManaUI();
    }

    // =====================================
    // SPEND STAT POINT (server RPC)
    // =====================================

    [ServerRpc]
    public void SpendStatPointServerRpc(
        string statName)
    {
        if (statPoints <= 0) return;

        switch (statName)
        {
            case "Strength":     baseStrength++;     break;
            case "Dexterity":    baseDexterity++;    break;
            case "Intelligence": baseIntelligence++; break;
            case "Health":       baseHealth++;       break;
            case "Mana":         baseMana++;         break;
            case "Capacity":     baseCapacity++;     break;
            case "HP Regen":     baseHPRegen++;      break;
            case "MP Regen":     baseMPRegen++;      break;
        }

        statPoints--;

        RecalculateStats();
    }

    // =====================================
    // DAMAGE
    // =====================================

    public int CalculateDamage()
    {
        return baseDamage;
    }

    // =====================================
    // TAKE DAMAGE
    // =====================================

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        amount -= defense;

        if (amount < 1) amount = 1;

        int newHealth =
            Mathf.Clamp(
                netHealth.Value - amount,
                0,
                netMaxHealth.Value);

        netHealth.Value = newHealth;

        currentHealth = newHealth;

        UpdateHealthUI();

        if (netHealth.Value <= 0)
        {
            Die();
        }
    }

    // =====================================
    // HEAL
    // =====================================

    public void Heal(int amount)
    {
        if (!IsServer) return;

        netHealth.Value =
            Mathf.Clamp(
                netHealth.Value + amount,
                0,
                netMaxHealth.Value);

        currentHealth = netHealth.Value;

        UpdateHealthUI();
    }

    // =====================================
    // RESTORE MANA
    // =====================================

    public void RestoreMana(int amount)
    {
        if (!IsServer) return;

        netMana.Value =
            Mathf.Clamp(
                netMana.Value + amount,
                0,
                netMaxMana.Value);

        currentMana = netMana.Value;

        UpdateManaUI();
    }

    // =====================================
    // GOLD
    // =====================================

    public void AddGold(int amount)
    {
        gold += amount;

        Debug.Log(
            "Gold: +" + amount +
            " (Total: " + gold + ")");
    }

    // =====================================
    // EXPERIENCE
    // =====================================

    public void GainExperience(int amount)
    {
        if (!IsServer) return;

        experience += amount;

        while (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel;
            LevelUp();
        }
    }

    // =====================================
    // LEVEL UP
    // =====================================

    void LevelUp()
    {
        level++;
        statPoints += 3;
        experienceToNextLevel *= 2;

        netLevel.Value = level;

        RecalculateStats();

        // Notify HUD
        LevelUpNotifier notifier =
            FindFirstObjectByType<LevelUpNotifier>();

        if (notifier != null)
        {
            notifier.ShowLevelUpMessage(
                level, statPoints);
        }
    }

    // =====================================
    // HEALTH UI
    // =====================================

    void UpdateHealthUI()
    {
        if (healthFill == null) return;

        if (maxHealth <= 0) return;

        healthFill.fillAmount =
            (float)currentHealth / maxHealth;
    }

    // =====================================
    // MANA UI
    // =====================================

    void UpdateManaUI()
    {
        if (manaFill == null) return;

        if (maxMana <= 0) return;

        manaFill.fillAmount =
            (float)currentMana / maxMana;
    }

    // =====================================
    // DIE
    // =====================================

    void Die()
    {
        Debug.Log("Player died.");
    }
}
