using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Core Stats")]
    public int strength = 5;

    public int agility = 5;

    public int intellect = 5;

    [Header("Combat")]
    public int weaponDamage = 5;

    public int bonusDamage = 0;

    // LOWER = FASTER ATTACKS
    // 1 = one attack per second
    // 0.5 = two attacks per second
    // 2 = one attack every two seconds
    public float attackSpeed = 1f;

    [Header("Combat Range")]
    // MELEE RANGE
    public float attackRange = 2f;

    [Header("Health")]
    public int maxHealth = 100;

    public int currentHealth = 100;

    [Header("Mana")]
    public int maxMana = 100;

    public int currentMana = 100;

    void Start()
    {
        currentHealth = maxHealth;

        currentMana = maxMana;
    }

    // FINAL DAMAGE CALCULATION
    public int CalculateDamage()
    {
        int finalDamage =
            weaponDamage +
            strength +
            bonusDamage;

        return finalDamage;
    }
}