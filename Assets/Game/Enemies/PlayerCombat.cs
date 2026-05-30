using Unity.Netcode;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
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
        // Only the owning player processes input
        if (!IsOwner) return;

        HandleTargeting();

        HandleAutoAttack();
    }

    // =========================================
    // TARGETING
    // =========================================

    void HandleTargeting()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition);

        if (!Physics.Raycast(
            ray, out RaycastHit hit, 100f))
        {
            return;
        }

        Targetable target =
            hit.collider.GetComponent<Targetable>();

        if (target != null)
        {
            SetTarget(target);
        }
    }

    // =========================================
    // SET TARGET
    // =========================================

    void SetTarget(Targetable target)
    {
        if (
            currentTarget != null &&
            currentTarget.targetRing != null)
        {
            currentTarget.targetRing
                .SetActive(false);
        }

        currentTarget = target;

        Targetable.CurrentTarget = target;

        if (
            currentTarget != null &&
            currentTarget.targetRing != null)
        {
            currentTarget.targetRing
                .SetActive(true);
        }
    }

    // =========================================
    // AUTO ATTACK
    // =========================================

    void HandleAutoAttack()
    {
        if (currentTarget == null) return;

        if (!currentTarget
            .gameObject.activeInHierarchy)
        {
            currentTarget = null;
            return;
        }

        attackTimer += Time.deltaTime;

        float interval =
            1f / Mathf.Max(
                playerStats.attackSpeed,
                0.01f);

        if (attackTimer < interval) return;

        attackTimer = 0f;

        TryAttackServerRpc(
            currentTarget
                .GetComponent<NetworkObject>()
                .NetworkObjectId);
    }

    // =========================================
    // ATTACK SERVER RPC
    // Server validates and applies damage
    // =========================================

    [ServerRpc]
    void TryAttackServerRpc(
        ulong targetNetworkId)
    {
        if (!NetworkManager.Singleton
            .SpawnManager.SpawnedObjects
            .TryGetValue(
                targetNetworkId,
                out NetworkObject targetObj))
        {
            return;
        }

        Targetable target =
            targetObj
                .GetComponent<Targetable>();

        if (target == null) return;

        float distance =
            Vector3.Distance(
                transform.position,
                target.transform.position);

        if (distance > playerStats.attackRange)
        {
            return;
        }

        // DAMAGE

        int damage =
            playerStats.CalculateDamage();

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

        EnemyStats enemy =
            targetObj
                .GetComponent<EnemyStats>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Notify owning client of hit
        PlayAttackAnimationClientRpc();
    }

    // =========================================
    // PLAY ATTACK ANIMATION (all clients)
    // =========================================

    [ClientRpc]
    void PlayAttackAnimationClientRpc()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // =========================================
    // GIZMOS
    // =========================================

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;

        float range = 2f;

        PlayerStats stats =
            GetComponent<PlayerStats>();

        if (stats != null)
        {
            range = stats.attackRange;
        }

        Gizmos.DrawWireSphere(
            attackPoint.position, range);
    }
}
