using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public enum states {
        patrol,
        warned,
        chase
    }
    public states currentState;
    [SerializeField] private GameObject deadBodyPrefab;
    [SerializeField] private float dyingForce;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask playerMask, enemyMask;
    private NavMeshAgent agent;
    [SerializeField] private float detectionSpeed;
    private Vector2 warnedPosition;

    [SerializeField] private Transform[] checkpoints;
    private int currentCheckpoint = 0;
    private bool watching;
    private float watchingTime;
    [SerializeField] private float maxWatchingTime;
    [SerializeField] private float detectionDistance;

    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentState = states.patrol;
        agent.SetDestination(checkpoints[currentCheckpoint].position);
    }
    
    void Update()
    {
        if(target == null)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 100, Vector2.zero, 0, playerMask);
            if(hit)
            {
                target = hit.transform;
            }
            return;
        }
        
        if(currentState == states.patrol)
        {
            Vector2 dir = -(transform.position - target.position).normalized;
            RaycastHit2D checkEnemy = Physics2D.Raycast(transform.position, dir, 10, ~enemyMask);
            if(checkEnemy)
            {
                if(checkEnemy.transform.GetComponent<movement>())
                {
                    currentState = states.chase;
                    return;
                }
            }
            float distance = Vector2.Distance(transform.position, checkpoints[currentCheckpoint].position);
            if(distance < 0.5f)
            {
                currentCheckpoint++;
                agent.SetDestination(checkpoints[currentCheckpoint].position);
            }
        }
        else if(currentState == states.warned)
        {
            Vector2 dir = -(transform.position - target.position).normalized;
            RaycastHit2D checkEnemy = Physics2D.Raycast(transform.position, dir, 10, ~enemyMask);
            if(checkEnemy)
            {
                if(checkEnemy.transform.GetComponent<movement>())
                {
                    currentState = states.chase;
                    return;
                }
            }
            float distance = Vector2.Distance(transform.position, warnedPosition);
            if(distance < 4)
            {
                if(watching)
                {
                    watchingTime -= Time.deltaTime;
                }
                else
                {
                    watching = true;
                    watchingTime = maxWatchingTime;
                }
                if(watching && watchingTime <= 0)
                {
                    currentState = states.patrol;
                    agent.SetDestination(checkpoints[currentCheckpoint].position);
                }
            }
        }
        else if(currentState == states.chase)
        {
            agent.SetDestination(target.position);
        }
    }

    public void die(Vector2 direction)
    {
        Rigidbody2D deadBody = Instantiate(deadBodyPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        deadBody.AddForce(direction * dyingForce, ForceMode2D.Impulse);
        warnEnemies();
        Destroy(gameObject);
    }

    public void warn(Vector2 position)
    {
        if(currentState == states.chase)
            return;
        currentState = states.warned;
        warnedPosition = position;
        agent.SetDestination(warnedPosition);
    }

    public void warnEnemies()
    {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, detectionDistance, Vector2.zero, 0, enemyMask);
        for (int i = 0; i < hit.Length; i++)
        {
            if(hit[i])
            {
                hit[i].transform.GetComponent<enemy>().warn(transform.position);
            }
        }
    }
}
