using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Progression")]
    public int level = 1;

    public int experience = 0;

    public int experienceToNextLevel = 100;

    public int statPoints = 0;

    [Header("Primary Stats")]
    public int strength = 5;

    public int dexterity = 5;

    public int intellect = 5;

    public int vitality = 5;

    public int capacity = 5;

    public int hpRegen = 1;

    public int mpRegen = 1;

    [Header("Resources")]
    public int maxHealth;

    public int currentHealth;

    public int maxMana;

    public int currentMana;

    [Header("Combat")]
    public int defense = 0;

    // 1 = melee range
    public float attackRange = 2f;

    // 1 = one attack per second
    public float attackSpeed = 1f;

    public float moveSpeed = 5f;

    public bool pvpEnabled = false;

    [Header("Damage")]
    public int baseDamage = 10;

    [Header("UI")]
    public Image healthFill;

    public Image manaFill;

    void Start()
    {
        RecalculateStats();

        currentHealth = maxHealth;

        currentMana = maxMana;

        UpdateHealthUI();

        UpdateManaUI();
    }

    void Update()
    {
        UpdateHealthUI();

        UpdateManaUI();
    }

    public void RecalculateStats()
    {
        // HEALTH
        maxHealth =
            100 + (vitality * 20);

        // MANA
        maxMana =
            100 + (intellect * 15);

        // MOVE SPEED
        moveSpeed =
            5f + (dexterity * 0.05f);

        // ATTACK SPEED
        // 1 = once per second
        attackSpeed = 1f;

        // KEEP CURRENT VALUES VALID
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
    }

    public int CalculateDamage()
    {
        // BASE DAMAGE + STRENGTH SCALING
        int damage =
            baseDamage +
            (strength * 2);

        return damage;
    }

    public void TakeDamage(int amount)
    {
        // DEFENSE REDUCTION
        amount -= defense;

        // ALWAYS AT LEAST 1 DAMAGE
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

        Debug.Log(
            "Player took " +
            amount +
            " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

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

    public void GainExperience(int amount)
    {
        experience += amount;

        Debug.Log(
            "Gained " +
            amount +
            " EXP.");

        while (
            experience >=
            experienceToNextLevel)
        {
            experience -=
                experienceToNextLevel;

            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;

        statPoints += 3;

        experienceToNextLevel += 25;

        Debug.Log(
            "LEVEL UP! Level " +
            level);

        RecalculateStats();
    }

    void UpdateHealthUI()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth /
                maxHealth;
        }
    }

    void UpdateManaUI()
    {
        if (manaFill != null)
        {
            manaFill.fillAmount =
                (float)currentMana /
                maxMana;
        }
    }

    void Die()
    {
        Debug.Log("Player died.");
    }
}