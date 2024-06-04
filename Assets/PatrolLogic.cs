using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolLogic : MonoBehaviour
{
    [SerializeField] private Transform playerRef;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float chaseDuration = 5f;
    [SerializeField] private float attackRange = 1f;
    private NavMeshAgent agentRef;
    private Animator animatorRef;
    private int currentPoint;
    private bool canSpot;
    private bool canAttack;

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
        currentPoint = 0;
        enemyState = EnemyState.Patrol;
        animatorRef = GetComponent<Animator>();
        agentRef = GetComponent<NavMeshAgent>();
        animatorRef.SetTrigger("isWalking");
    }

    private void FixedUpdate()
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
            enemyState = EnemyState.Chase;
        }
    }


    public void Move()
    {
        agentRef.speed = movementSpeed;
        Vector3 newPos = new(patrolPoints[currentPoint].patrolPoint.position.x, transform.position.y, patrolPoints[currentPoint].patrolPoint.position.z);

        if (Vector3.Distance(agentRef.transform.position, newPos) > 0.1f)
        {
            agentRef.destination = newPos;
        }
        else
        {
            currentPoint++;
            canSpot = true;
            canAttack = true;
            if (currentPoint >= patrolPoints.Count)
            {
                currentPoint = 0;
            }
        }
    }

    public void Chase()
    {
        if (!canSpot)
        {
            return;
        }

        canSpot = false;
        agentRef.speed = movementSpeed * 2;
        StartCoroutine(ChaseCoroutine());
    }

    private IEnumerator ChaseCoroutine()
    {
        float chaseTime = 0f;
        animatorRef.SetTrigger("isRunning");
        while (chaseTime < chaseDuration)
        {
            agentRef.destination = playerRef.position;

            if (Vector3.Distance(transform.position, playerRef.position) <= attackRange)
            {
                enemyState = EnemyState.Attack;
                yield break;
            }

            chaseTime += Time.deltaTime;
            yield return null;
        }

        animatorRef.SetTrigger("isWalking");
        enemyState = EnemyState.Patrol;
    }
    public void Attack()
    {
        if (!canAttack)
        {
            return;
        }

        canAttack = false;
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        agentRef.isStopped = true;
        animatorRef.SetTrigger("isShooting");
        yield return new WaitForSeconds(2f);
        agentRef.isStopped = false;
        animatorRef.SetTrigger("isWalking");
        enemyState = EnemyState.Patrol;
    }
}
