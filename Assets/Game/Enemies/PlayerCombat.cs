using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private bool autoAttacking = false;

    private bool isAttacking = false;

    private float attackTimer = 0f;

    private Targetable currentCombatTarget;

    private Animator animator;

    private PlayerStats playerStats;

    void Start()
    {
        // FIND ANIMATOR ON CHILD MODEL
        animator =
            GetComponentInChildren<Animator>();

        // GET PLAYER STATS
        playerStats =
            GetComponent<PlayerStats>();
    }

    void Update()
    {
        HandleInput();

        if (autoAttacking && !isAttacking)
        {
            attackTimer += Time.deltaTime;

            // ATTACK TIMER
            if (attackTimer >= playerStats.attackSpeed)
            {
                attackTimer = 0f;

                PerformAttack();
            }
        }
    }

    void HandleInput()
    {
        // RIGHT CLICK = START AUTO ATTACK
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray =
                Camera.main.ScreenPointToRay(
                    Input.mousePosition
                );

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Targetable target =
                    hit.collider.GetComponent<Targetable>();

                if (target != null)
                {
                    currentCombatTarget = target;

                    autoAttacking = true;

                    attackTimer =
                        playerStats.attackSpeed;

                    target.SelectTarget();

                    CombatTargetVisual visual =
                        target.GetComponent<CombatTargetVisual>();

                    if (visual != null)
                    {
                        visual.StartPulse();
                    }

                    Debug.Log("AUTO ATTACK STARTED");
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

        // CHECK RANGE
        float distance =
            Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

        // TOO FAR AWAY
        if (distance > playerStats.attackRange)
        {
            Debug.Log("Target out of range");

            return;
        }

        isAttacking = true;

        // SCALE ANIMATION SPEED
        animator.speed =
            1f / playerStats.attackSpeed;

        // PLAY ATTACK ANIMATION
        animator.Play("Attack", 0, 0f);

        // SMALL HIT DELAY
        Invoke(nameof(ApplyDamage), 0.25f);

        // END ATTACK
        Invoke(nameof(EndAttack), playerStats.attackSpeed);
    }

    void ApplyDamage()
    {
        if (currentCombatTarget == null)
            return;

        EnemyStats enemy =
            currentCombatTarget
            .GetComponent<EnemyStats>();

        if (enemy == null)
            return;

        int damage =
            playerStats.CalculateDamage();

        enemy.TakeDamage(damage);

        Debug.Log("Hit for " + damage);

        // DEAD
        if (enemy.currentHealth <= 0)
        {
            StopAutoAttack();
        }
    }

    void EndAttack()
    {
        isAttacking = false;

        // RESET ANIMATOR SPEED
        animator.speed = 1f;

        // RETURN TO IDLE
        animator.Play("Idle");
    }

    public void StopAutoAttack()
    {
        autoAttacking = false;

        isAttacking = false;

        attackTimer = 0f;

        // RESET ANIMATOR
        if (animator != null)
        {
            animator.speed = 1f;

            animator.Play("Idle");
        }

        // STOP TARGET PULSE
        if (currentCombatTarget != null)
        {
            CombatTargetVisual visual =
                currentCombatTarget
                .GetComponent<CombatTargetVisual>();

            if (visual != null)
            {
                visual.StopPulse();
            }
        }

        currentCombatTarget = null;

        Debug.Log("AUTO ATTACK STOPPED");
    }
}