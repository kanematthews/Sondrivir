using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    public int baseDamage = 10;

    public float attackSpeed = 1.5f;

    private bool autoAttacking;

    private float nextAttackTime;

    void Update()
    {
        HandleInput();

        if (autoAttacking)
        {
            AutoAttack();
        }
    }

    void HandleInput()
    {
        // RIGHT CLICK = START AUTO ATTACK
        if (Input.GetMouseButtonDown(1))
        {
            if (Targetable.CurrentTarget != null)
            {
                autoAttacking = true;

                CombatTargetVisual visual =
                    Targetable.CurrentTarget
                    .GetComponent<CombatTargetVisual>();

                if (visual != null)
                {
                    visual.StartPulse();
                }
            }
        }
    }

    void AutoAttack()
    {
        if (Targetable.CurrentTarget == null)
        {
            StopAutoAttack();
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            EnemyStats enemy =
                Targetable.CurrentTarget
                .GetComponent<EnemyStats>();

            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);

                Debug.Log("Hit for " + baseDamage);

                if (enemy.currentHealth <= 0)
                {
                    StopAutoAttack();
                }
            }

            nextAttackTime =
                Time.time + attackSpeed;
        }
    }

    public void StopAutoAttack()
    {
        autoAttacking = false;

        if (Targetable.CurrentTarget != null)
        {
            CombatTargetVisual visual =
                Targetable.CurrentTarget
                .GetComponent<CombatTargetVisual>();

            if (visual != null)
            {
                visual.StopPulse();
            }
        }
    }
}