using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
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

    public int baseIntellect = 1;

    public int baseVitality = 1;

    // =====================================
    // FINAL STATS
    // =====================================

    [Header("Final Stats")]
    public int strength;

    public int dexterity;

    public int intellect;

    public int vitality;

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
    // RESOURCES
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
    // START
    // =====================================

    void Start()
    {
        equipment =
            GetComponent<EquipmentContainer>();

        RecalculateStats();

        currentHealth = maxHealth;

        currentMana = maxMana;

        UpdateHealthUI();

        UpdateManaUI();
    }

    // =====================================
    // UPDATE
    // =====================================

    void Update()
    {
        HandleRegeneration();

        UpdateHealthUI();

        UpdateManaUI();
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
        // RESET PRIMARYS

        strength = baseStrength;

        dexterity = baseDexterity;

        intellect = baseIntellect;

        vitality = baseVitality;

        // RESET COMBAT

        baseDamage = 1;

        defense = 0;

        attackSpeed = 1f;

        attackRange = 2f;

        critChance = 0f;

        critDamage = 1.5f;

        // RESET SECONDARYS

        moveSpeed = 5f;

        capacity = 50f;

        hpRegenAmount = 1;

        mpRegenAmount = 1;

        pvpEnabled = false;

        // RESET RESOURCES

        maxHealth =
            100 + (vitality * 20);

        maxMana =
            100 + (intellect * 15);

        // =================================
        // EQUIPMENT
        // =================================

        if (equipment != null)
        {
            foreach (ItemStack stack in equipment.slots)
            {
                if (
                    stack == null ||
                    stack.item == null)
                {
                    continue;
                }

                ItemData item =
                    stack.item;

                // PRIMARYS

                strength +=
                    item.bonusStrength;

                dexterity +=
                    item.bonusDexterity;

                intellect +=
                    item.bonusIntellect;

                vitality +=
                    item.bonusVitality;

                // ARMOR

                defense +=
                    item.armor;

                // MAINHAND DEFINES WEAPON

                if (
                    item.equipmentSlotType ==
                    EquipmentSlotType.MainHand)
                {
                    baseDamage =
                        item.damage;

                    attackSpeed =
                        item.attackSpeed;

                    attackRange =
                        item.attackRange;
                }

                // BONUS COMBAT
                // TEMPORARILY DISABLED
                // UNTIL UNITY FULLY RECOMPILES

                // RESOURCES

                maxHealth +=
                    item.bonusHealth;

                maxMana +=
                    item.bonusMana;

                // REGEN

                hpRegenAmount +=
                    item.bonusHPRegen;

                mpRegenAmount +=
                    item.bonusMPRegen;

                // MOVEMENT

                moveSpeed +=
                    item.bonusMoveSpeed;

                capacity +=
                    item.bonusCapacity;

                // SPECIAL

                if (item.enablePvp)
                {
                    pvpEnabled = true;
                }
            }
        }

        // FINAL DAMAGE

        baseDamage +=
            strength * 2;

        // CLAMP

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth);

        currentMana =
            Mathf.Clamp(
                currentMana,
                0,
                maxMana);

        UpdateHealthUI();

        UpdateManaUI();

        Debug.Log(
            "Stats Recalculated | " +
            "DMG: " + baseDamage +
            " SPD: " + attackSpeed);
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
        amount -= defense;

        if (amount < 1)
        {
            amount = 1;
        }

        currentHealth -= amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // =====================================
    // HEAL
    // =====================================

    public void Heal(int amount)
    {
        currentHealth += amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth);

        UpdateHealthUI();
    }

    // =====================================
    // RESTORE MANA
    // =====================================

    public void RestoreMana(int amount)
    {
        currentMana += amount;

        currentMana =
            Mathf.Clamp(
                currentMana,
                0,
                maxMana);

        UpdateManaUI();
    }

    // =====================================
    // EXPERIENCE
    // =====================================

    public void GainExperience(int amount)
    {
        experience += amount;

        while (
            experience >=
            experienceToNextLevel)
        {
            experience -=
                experienceToNextLevel;

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

        RecalculateStats();
    }

    // =====================================
    // HEALTH UI
    // =====================================

    void UpdateHealthUI()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth /
                maxHealth;
        }
    }

    // =====================================
    // MANA UI
    // =====================================

    void UpdateManaUI()
    {
        if (manaFill != null)
        {
            manaFill.fillAmount =
                (float)currentMana /
                maxMana;
        }
    }

    // =====================================
    // DIE
    // =====================================

    void Die()
    {
        Debug.Log("Player died.");
    }
}