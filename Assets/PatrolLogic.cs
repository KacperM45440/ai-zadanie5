using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolLogic : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private NavMeshAgent agentRef;
    private int currentPoint;
    private Coroutine patrolRoutine;

    [System.Serializable]
    public struct PatrolStruct
    {
        public Transform patrolPoint;
        //public bool waitAtPoint;
    }

    public List<PatrolStruct> patrolPoints;

    void Start()
    {
        currentPoint = 0;
        patrolRoutine = StartCoroutine(MovementCoroutine());
    }

    public void StopMoving()
    {
        if (patrolRoutine == null)
        {
            return;
        }

        StopCoroutine(patrolRoutine);
        patrolRoutine = null;
    }

    public void StartMoving()
    {
        if (patrolRoutine != null)
        {
            return;
        }

        patrolRoutine = StartCoroutine(MovementCoroutine());
    }
    private IEnumerator MovementCoroutine()
    {
        while (true)
        {
            Vector3 newPos = new(patrolPoints[currentPoint].patrolPoint.position.x, transform.position.y, patrolPoints[currentPoint].patrolPoint.position.z);

            if (agentRef.transform.position != newPos)
            {
                agentRef.destination = newPos;
            }
            else
            {
                currentPoint++;
                if (currentPoint > patrolPoints.Count - 1)
                {
                    currentPoint = 0;
                }
            }
            yield return null;
        }
    }
}