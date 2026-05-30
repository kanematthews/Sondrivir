using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : NetworkBehaviour
{
    private NavMeshAgent agent;

    private EnemyStats enemyStats;

    private Animator animator;

    private Transform targetPlayer;

    private PlayerStats targetPlayerStats;

    [Header("Enemy")]
    public string enemyID = "skeleton";

    [Header("Roaming")]
    public float roamRadius = 5f;

    public float idleTime = 3f;

    private float idleTimer;

    private Vector3 spawnPosition;

    private bool roaming = false;

    [Header("Leash")]
    public float maxChaseDistance = 15f;

    public float giveUpTime = 10f;

    private float chaseTimer;

    private bool engaged = false;

    private float attackTimer;

    // =====================================
    // START
    // =====================================

    void Start()
    {
        // AI only runs on server
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        agent       = GetComponent<NavMeshAgent>();
        enemyStats  = GetComponent<EnemyStats>();
        animator    = GetComponentInChildren<Animator>();
        spawnPosition = transform.position;
        idleTimer     = idleTime;

        FindNearestPlayer();
    }

    // =====================================
    // UPDATE (server only)
    // =====================================

    void Update()
    {
        if (
            targetPlayer == null ||
            targetPlayerStats == null)
        {
            FindNearestPlayer();
            return;
        }

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                targetPlayer.position);

        float distanceFromSpawn =
            Vector3.Distance(
                transform.position,
                spawnPosition);

        if (enemyStats.behaviour ==
            EnemyBehaviour.Hostile)
        {
            if (distanceToPlayer <=
                enemyStats.aggroRange)
            {
                engaged = true;
            }
        }

        if (engaged)
        {
            chaseTimer += Time.deltaTime;

            if (
                distanceFromSpawn >
                    maxChaseDistance ||
                chaseTimer >= giveUpTime)
            {
                ReturnToSpawn();
                return;
            }

            agent.isStopped = false;
            agent.SetDestination(
                targetPlayer.position);

            SetMoving(true);

            if (distanceToPlayer <=
                enemyStats.attackRange)
            {
                agent.isStopped = true;
                SetMoving(false);
                AttackPlayer();
            }
        }
        else
        {
            HandleRoaming();
        }
    }

    // =====================================
    // FIND NEAREST PLAYER
    // =====================================

    void FindNearestPlayer()
    {
        PlayerStats[] players =
            FindObjectsByType<PlayerStats>(
                FindObjectsSortMode.None);

        float closest = float.MaxValue;

        foreach (PlayerStats p in players)
        {
            float dist =
                Vector3.Distance(
                    transform.position,
                    p.transform.position);

            if (dist < closest)
            {
                closest            = dist;
                targetPlayerStats  = p;
                targetPlayer       = p.transform;
            }
        }
    }

    // =====================================
    // ROAMING
    // =====================================

    void HandleRoaming()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer > 0)
        {
            SetMoving(false);
            return;
        }

        if (!roaming)
        {
            Vector3 randomDir =
                Random.insideUnitSphere *
                roamRadius;

            randomDir += spawnPosition;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                randomDir,
                out hit,
                roamRadius,
                NavMesh.AllAreas))
            {
                roaming = true;

                agent.isStopped = false;
                agent.SetDestination(hit.position);

                SetMoving(true);
            }
        }

        if (
            roaming &&
            !agent.pathPending &&
            agent.remainingDistance <= 0.5f)
        {
            roaming   = false;
            idleTimer = idleTime;

            SetMoving(false);
        }
    }

    // =====================================
    // ATTACK PLAYER
    // =====================================

    void AttackPlayer()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer < enemyStats.attackSpeed)
            return;

        attackTimer = 0f;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        targetPlayerStats.TakeDamage(
            enemyStats.damage);
    }

    // =====================================
    // ENGAGE
    // =====================================

    public void Engage()
    {
        engaged    = true;
        chaseTimer = 0f;
    }

    // =====================================
    // RETURN TO SPAWN
    // =====================================

    void ReturnToSpawn()
    {
        engaged = false;
        roaming = false;
        chaseTimer = 0f;

        agent.isStopped = false;
        agent.SetDestination(spawnPosition);

        SetMoving(true);

        // Re-target when back at spawn
        FindNearestPlayer();
    }

    // =====================================
    // SET MOVING
    // =====================================

    void SetMoving(bool moving)
    {
        if (animator != null)
        {
            animator.SetBool("Moving", moving);
        }
    }

    // =====================================
    // DIE
    // =====================================

    public void Die()
    {
        Debug.Log(enemyID + " died.");
        Destroy(gameObject);
    }
}
