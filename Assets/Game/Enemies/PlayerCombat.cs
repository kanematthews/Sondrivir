using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerStats playerStats;

    private bool autoAttacking = false;

    private float attackTimer = 0f;

    private Targetable currentCombatTarget;

    private Animator animator;

    void Start()
    {
        playerStats =
            GetComponent<PlayerStats>();

        // FIXED
        animator =
            GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleInput();

        if (autoAttacking)
        {
            attackTimer += Time.deltaTime;

            // 1 = once per second
            if (
                attackTimer >=
                playerStats.attackSpeed)
            {
                attackTimer = 0f;

                PerformAttack();
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray =
                Camera.main
                .ScreenPointToRay(
                    Input.mousePosition);

            if (
                Physics.Raycast(
                    ray,
                    out RaycastHit hit))
            {
                Targetable target =
                    hit.collider
                    .GetComponent<Targetable>();

                if (target != null)
                {
                    if (
                        currentCombatTarget ==
                        target)
                    {
                        return;
                    }

                    StopAutoAttack();

                    currentCombatTarget =
                        target;

                    target.SelectTarget();

                    autoAttacking = true;

                    attackTimer = 0f;

                    CombatTargetVisual visual =
                        target.GetComponent
                        <CombatTargetVisual>();

                    if (visual != null)
                    {
                        visual.StartPulse();
                    }
                }
            }
        }
    }

    void PerformAttack()
    {
        if (currentCombatTarget == null)
        {
            StopAutoAttack();
            return;
        }

        EnemyStats enemy =
            currentCombatTarget
            .GetComponent<EnemyStats>();

        if (enemy == null)
        {
            StopAutoAttack();
            return;
        }

        // RANGE CHECK
        float distance =
            Vector3.Distance(
                transform.position,
                enemy.transform.position);

        if (
            distance >
            playerStats.attackRange)
        {
            return;
        }

        // ATTACK ANIMATION
        if (animator != null)
        {
            animator.ResetTrigger(
                "Attack");

            animator.SetTrigger(
                "Attack");
        }

        // DAMAGE
        int damage =
            playerStats.CalculateDamage();

        enemy.TakeDamage(damage);

        Debug.Log(
            "Hit for " +
            damage);

        if (enemy.currentHealth <= 0)
        {
            StopAutoAttack();
        }
    }

    public void StopAutoAttack()
    {
        autoAttacking = false;

        attackTimer = 0f;

        if (currentCombatTarget != null)
        {
            CombatTargetVisual visual =
                currentCombatTarget
                .GetComponent
                <CombatTargetVisual>();

            if (visual != null)
            {
                visual.StopPulse();
            }
        }

        currentCombatTarget = null;
    }
}