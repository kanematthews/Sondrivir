using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    public LayerMask enemyLayer;

    public Transform attackPoint;

    [Header("Animation")]
    public Animator animator;

    private PlayerStats playerStats;

    private Targetable currentTarget;

    private float attackTimer;

    // =========================================
    // START
    // =========================================

    void Start()
    {
        playerStats =
            GetComponent<PlayerStats>();

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }
    }

    // =========================================
    // UPDATE
    // =========================================

    void Update()
    {
        HandleTargeting();

        HandleAutoAttack();
    }

    // =========================================
    // TARGETING
    // =========================================

    void HandleTargeting()
    {
        // RIGHT CLICK
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray =
                Camera.main.ScreenPointToRay(
                    Input.mousePosition);

            if (
                Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    100f))
            {
                Targetable target =
                    hit.collider.GetComponent<Targetable>();

                // VALID TARGET
                if (target != null)
                {
                    SetTarget(target);
                }
            }
        }
    }

    // =========================================
    // SET TARGET
    // =========================================

    void SetTarget(Targetable target)
    {
        // CLEAR OLD TARGET
        if (
            currentTarget != null &&
            currentTarget.targetRing != null)
        {
            currentTarget.targetRing.SetActive(false);
        }

        currentTarget = target;

        Targetable.CurrentTarget =
            target;

        // ENABLE NEW TARGET RING
        if (
            currentTarget != null &&
            currentTarget.targetRing != null)
        {
            currentTarget.targetRing.SetActive(true);
        }

        Debug.Log(
            "Targeted: " +
            currentTarget.name);
    }

    // =========================================
    // AUTO ATTACK
    // =========================================

    void HandleAutoAttack()
    {
        // NO TARGET
        if (currentTarget == null)
        {
            return;
        }

        // TARGET DESTROYED
        if (!currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;

            return;
        }

        // TIMER
        attackTimer += Time.deltaTime;

        float interval =
            1f /
            Mathf.Max(
                playerStats.attackSpeed,
                0.01f);

        // WAIT
        if (attackTimer < interval)
        {
            return;
        }

        attackTimer = 0f;

        Attack(currentTarget);
    }

    // =========================================
    // ATTACK
    // =========================================

    void Attack(Targetable target)
    {
        if (target == null)
        {
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                target.transform.position);

        // OUT OF RANGE
        if (distance > playerStats.attackRange)
        {
            Debug.Log("Target out of range.");

            return;
        }

        // FACE TARGET
        Vector3 lookPosition =
            target.transform.position;

        lookPosition.y =
            transform.position.y;

        transform.LookAt(lookPosition);

        // PLAY ATTACK ANIMATION
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // DAMAGE
        int damage =
            playerStats.CalculateDamage();

        // CRIT
        bool crit =
            Random.value <=
            playerStats.critChance;

        if (crit)
        {
            damage =
                Mathf.RoundToInt(
                    damage *
                    playerStats.critDamage);
        }

        // ENEMY
        EnemyStats enemy =
            target.GetComponent<EnemyStats>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            Debug.Log(
                "Hit " +
                enemy.name +
                " for " +
                damage +
                (crit
                ? " CRITICAL!"
                : ""));
        }
    }

    // =========================================
    // GIZMOS
    // =========================================

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        float range = 2f;

        PlayerStats stats =
            GetComponent<PlayerStats>();

        if (stats != null)
        {
            range =
                stats.attackRange;
        }

        Gizmos.DrawWireSphere(
            attackPoint.position,
            range);
    }
}