using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;

    private EnemyStats enemyStats;

    private Animator animator;

    private Transform player;

    private PlayerStats playerStats;

    [Header("Enemy")]
    public string enemyID =
        "skeleton";

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

    void Start()
    {
        agent =
            GetComponent<NavMeshAgent>();

        enemyStats =
            GetComponent<EnemyStats>();

        animator =
            GetComponentInChildren<Animator>();

        PlayerStats foundPlayer =
            FindFirstObjectByType<PlayerStats>();

        if (foundPlayer != null)
        {
            playerStats = foundPlayer;

            player =
                foundPlayer.transform;
        }

        spawnPosition =
            transform.position;

        idleTimer = idleTime;
    }

    void Update()
    {
        // =====================================
        // DEBUG KILL
        // PRESS K TO TEST QUESTS
        // =====================================

        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }

        if (
            player == null ||
            playerStats == null)
        {
            return;
        }

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position);

        float distanceFromSpawn =
            Vector3.Distance(
                transform.position,
                spawnPosition);

        // PASSIVE ENEMIES ONLY AGGRO WHEN HIT

        if (
            enemyStats.behaviour ==
            EnemyBehaviour.Hostile)
        {
            if (
                distanceToPlayer <=
                enemyStats.aggroRange)
            {
                engaged = true;
            }
        }

        // =====================================
        // CHASING
        // =====================================

        if (engaged)
        {
            chaseTimer += Time.deltaTime;

            // GIVE UP CONDITIONS

            if (
                distanceFromSpawn >
                maxChaseDistance ||

                chaseTimer >=
                giveUpTime)
            {
                ReturnToSpawn();

                return;
            }

            agent.isStopped = false;

            agent.SetDestination(
                player.position);

            // WALK ANIMATION

            SetMoving(true);

            // ATTACK RANGE

            if (
                distanceToPlayer <=
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
    // HANDLE ROAMING
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
            Vector3 randomDirection =
                Random.insideUnitSphere *
                roamRadius;

            randomDirection +=
                spawnPosition;

            NavMeshHit hit;

            if (
                NavMesh.SamplePosition(
                    randomDirection,
                    out hit,
                    roamRadius,
                    NavMesh.AllAreas))
            {
                roaming = true;

                agent.isStopped = false;

                agent.SetDestination(
                    hit.position);

                SetMoving(true);
            }
        }

        // REACHED DESTINATION

        if (
            roaming &&
            !agent.pathPending &&
            agent.remainingDistance <=
            0.5f)
        {
            roaming = false;

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

        if (
            attackTimer >=
            enemyStats.attackSpeed)
        {
            attackTimer = 0f;

            // ATTACK ANIMATION

            if (animator != null)
            {
                animator.ResetTrigger(
                    "Attack");

                animator.SetTrigger(
                    "Attack");
            }

            playerStats.TakeDamage(
                enemyStats.damage);

            Debug.Log(
                enemyStats.enemyName +
                " hit player for " +
                enemyStats.damage);
        }
    }

    // =====================================
    // ENGAGE
    // =====================================

    public void Engage()
    {
        engaged = true;

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

        agent.SetDestination(
            spawnPosition);

        SetMoving(true);
    }

    // =====================================
    // SET MOVING
    // =====================================

    void SetMoving(bool moving)
    {
        if (animator != null)
        {
            animator.SetBool(
                "Moving",
                moving);
        }
    }

    // =====================================
    // DIE
    // =====================================

    public void Die()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player");

        if (player != null)
        {
            QuestManager manager =
                player.GetComponent
                <QuestManager>();

            if (manager != null)
            {
                manager.RegisterKill(
                    enemyID);
            }
        }

        Debug.Log(
            enemyID +
            " died.");

        Destroy(gameObject);
    }
}