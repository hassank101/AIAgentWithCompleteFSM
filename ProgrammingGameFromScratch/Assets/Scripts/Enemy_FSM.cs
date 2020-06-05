using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_FSM : MonoBehaviour
{
    public enum enemyState {PATROL, ATTACK, CHASE};

    //Property to access the current state
    public enemyState CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;

            //Stop all coroutines
            StopAllCoroutines();

            switch (currentState)
            {
                case enemyState.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case enemyState.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
                case enemyState.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
            }
        }
    }

    [SerializeField]
    private enemyState currentState;

    //Some references
    private CheckMyVision checkMyVision = null;    //previously created script

    private NavMeshAgent agent = null;

    private Transform playerTransform = null;

    private Transform patrolDestination = null;

    private Health playerHealth = null;

    public float maxDamage = 10.0f;

    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerTransform = playerHealth.GetComponent<Transform>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //Find a random destination
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();

        
        CurrentState = enemyState.PATROL;
    }

    public IEnumerator EnemyPatrol()
    {
        while(currentState == enemyState.PATROL)
        {
            checkMyVision.sensitivity = CheckMyVision.enumSensitivity.HIGH;
            
            agent.isStopped = false;
            agent.SetDestination(patrolDestination.position);

            while (agent.pathPending)
            {
                yield return null;  //this is to ensure we wait for path completion
            }
            if (checkMyVision.targetInSight)
            {
                //Debug.Log("Found you, changing to CHASE state");
                agent.isStopped = true;
                CurrentState = enemyState.CHASE;
                yield break;
            }
            yield return null;
        }
    }

    public IEnumerator EnemyAttack()
    {
        while(currentState == enemyState.ATTACK)
        {
            //Debug.Log("I am attacking");
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            while (agent.pathPending)
            {
                yield return null;
            }
            if(agent.remainingDistance > 5)
            {
                CurrentState = enemyState.CHASE;
            }
            if(agent.remainingDistance < 1)
            {
                playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
            }
            yield return null;
        }
        yield break;
    }

    public IEnumerator EnemyChase()
    {
        //Debug.Log("Enemy CHASE starting");
        while(currentState == enemyState.CHASE)
        {
            //Keep the sensitivity low
            checkMyVision.sensitivity = CheckMyVision.enumSensitivity.LOW;
            
            //The idea of chase is to go to the last known position
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastKnownLocation);

            while (agent.pathPending)
            {
                yield return null;  //this is to ensure we wait for path completion
            }

            //While chasing, we need to keep checking if we reached
            
            if (agent.remainingDistance <= 5)
            {
                agent.isStopped = true;
                
                //If we rached destination and find player
                if (checkMyVision.targetInSight)
                {
                    //Debug.Log("Target in sight, so going to attack");
                    CurrentState = enemyState.ATTACK;

                }
                else
                {
                    //Debug.Log("Target not in sight, so patrolling");
                    CurrentState = enemyState.PATROL;
                }
                yield break;
            }
            yield return null;
        }
    }
}
