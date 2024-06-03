using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolLogic : MonoBehaviour
{
    [SerializeField] private Transform playerRef;
    [SerializeField] private float movementSpeed;
    [SerializeField] private NavMeshAgent agentRef;
    [SerializeField] private bool canSpot;
    [SerializeField] private float chaseDuration = 5f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private int maxAttacks = 1;
    private int currentPoint;
    private Collider detectionZone;
    private Coroutine chaseCoroutine;
    private Coroutine attackCoroutine;
    private int attackCount;

    [System.Serializable]
    public struct PatrolStruct
    {
        public Transform patrolPoint;
    }

    public List<PatrolStruct> patrolPoints;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public EnemyState enemyState;

    void Start()
    {
        detectionZone = GetComponent<Collider>();
        currentPoint = 0;
        enemyState = EnemyState.Patrol;
        agentRef.speed = movementSpeed;
    }

    private void Update()
    {
        ProcessLogic();
    }

    public void ProcessLogic()
    {
        switch (enemyState)
        {
            case EnemyState.Patrol:
                Move();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canSpot || enemyState == EnemyState.Attack)
        {
            return;
        }

        if (other.transform == playerRef)
        {
            if (chaseCoroutine != null)
            {
                StopCoroutine(chaseCoroutine);
            }

            chaseCoroutine = StartCoroutine(ChasePlayer());
        }
    }

    private IEnumerator ChasePlayer()
    {
        enemyState = EnemyState.Chase;
        float chaseTime = 0f;

        while (chaseTime < chaseDuration)
        {
            agentRef.destination = playerRef.position;

            if (Vector3.Distance(transform.position, playerRef.position) <= attackRange)
            {
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                }

                attackCoroutine = StartCoroutine(AttackPlayer());
                yield break;
            }

            chaseTime += Time.deltaTime;
            yield return null;
        }

        enemyState = EnemyState.Patrol;
    }

    private IEnumerator AttackPlayer()
    {
        enemyState = EnemyState.Attack;
        attackCount = 0;

        while (attackCount < maxAttacks)
        {
            //gdzies tu animacja
            attackCount++;
            yield return new WaitForSeconds(1f); 
        }

        enemyState = EnemyState.Patrol;
        canSpot = false;
    }

    public void Move()
    {
        Vector3 newPos = new Vector3(patrolPoints[currentPoint].patrolPoint.position.x, transform.position.y, patrolPoints[currentPoint].patrolPoint.position.z);

        if (Vector3.Distance(agentRef.transform.position, newPos) > 0.1f)
        {
            agentRef.destination = newPos;
        }
        else
        {
            currentPoint++;
            canSpot = true;
            if (currentPoint >= patrolPoints.Count)
            {
                currentPoint = 0;
            }
        }
    }

    public void Chase()
    {
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
        }
        chaseCoroutine = StartCoroutine(ChasePlayer());
    }

    public void Attack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(AttackPlayer());
    }
}
