using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public enum states {
        patrol,
        suspicious,
        warned,
        chase
    }
    public states currentState;
    [SerializeField] private GameObject deadBodyPrefab;
    [SerializeField] private float dyingForce;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask playerMask, enemyMask;
    private NavMeshAgent agent;
    [SerializeField] private float patrolSpeed, warnedSpeed, chaseSpeed;
    private Vector2 warnedPosition;

    [SerializeField] private Transform[] checkpoints;
    private int currentCheckpoint = 0;
    private bool watching;
    private float watchingTime;
    [SerializeField] private float maxWatchingTime;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float attackRange;
    private bool attacking;
    RaycastHit2D checkEnemy;
    [SerializeField] private bool isDriving;
    public drivingEnemy drivingEnemy;

    void Awake() 
    {
        if(isDriving)
            return;
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

        if(isDriving)
        {
            return;
        }

        Vector2 dir = -(transform.position - target.position).normalized;
        checkEnemy = Physics2D.Raycast(transform.position, dir, 10, ~enemyMask);
        if(checkEnemy)
        {
            if(checkEnemy.transform.GetComponent<movement>())
            {
                if(currentState != states.chase)
                {
                    currentState = states.chase;
                }
            }
        }
        
        if(currentState == states.patrol)
        {
            agent.speed = patrolSpeed;
            float distance = Vector2.Distance(transform.position, checkpoints[currentCheckpoint].position);
            if(distance < 0.5f)
            {
                currentCheckpoint++;
                agent.SetDestination(checkpoints[currentCheckpoint].position);
            }
        }
        else if(currentState == states.suspicious)
        {
            agent.SetDestination(transform.position);
        }
        else if(currentState == states.warned)
        {
            agent.speed = warnedSpeed;
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
            agent.speed = chaseSpeed;
            agent.SetDestination(target.position);
            if(attacking)
                return;
            float distance = Vector2.Distance(transform.position, target.position);
            if(distance < 1)
            {
                StartCoroutine(attack());
            }
        }
    }

    IEnumerator attack()
    {
        attacking = true;
        yield return new WaitForSeconds(0.2f);
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, attackRange, Vector2.zero, 0, playerMask);
        if(hit)
        {
            hit.transform.GetComponent<movement>().die();
        }
        attacking = false;
    }

    public void die(Vector2 direction)
    {
        if(isDriving)
        {
            drivingEnemy.Alive = false;
        }
        target.transform.GetComponent<movement>().AddScore();
        Rigidbody2D deadBody = Instantiate(deadBodyPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        deadBody.AddForce(direction * dyingForce, ForceMode2D.Impulse);
        warnEnemies();
        Destroy(gameObject);
    }

    public IEnumerator warn(Vector2 position)
    {
        if(isDriving)
            yield break;
        if(currentState != states.chase)
        {
            currentState = states.suspicious;
            yield return new WaitForSeconds(2);
            if(currentState != states.chase)
            {
                currentState = states.warned;
                warnedPosition = position;
                agent.SetDestination(warnedPosition);
            }
        }
    }

    public void warnEnemies()
    {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, detectionDistance, Vector2.zero, 0, enemyMask);
        for (int i = 0; i < hit.Length; i++)
        {
            if(hit[i])
            {
                StartCoroutine(hit[i].transform.GetComponent<enemy>().warn(transform.position));
            }
        }
    }
}
