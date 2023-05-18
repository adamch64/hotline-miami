using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{

    public enum enemyType {
        normal,
        driving,
        bodyguard
    }
    public enum states {
        patrol,
        suspicious,
        warned,
        chase
    }
    public states currentState;
    public enemyType _enemyType;
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
    public drivingEnemy drivingEnemy;
    [SerializeField] private GameObject bullet;
    Vector2 dir;
    public float bulletSpeed;
    public bool stunned;
    public bool hittedEnemy;
    Transform bulletTransform;
    public float AttackCooldownBodyguard, AttackCooldownNormal;
    private bool canAttack;
    void Awake() 
    {
        if(_enemyType == enemyType.driving)
            return;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentState = states.patrol;
        agent.SetDestination(checkpoints[currentCheckpoint].position);
        canAttack = true;
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

        if(_enemyType == enemyType.driving)
        {
            return;
        }
        if(stunned)
        {
            agent.SetDestination(transform.position);
            if(bulletTransform != null)
            {
                float bulletDistance = Vector2.Distance(transform.position, bulletTransform.position);
                RaycastHit2D stunPlayer = Physics2D.Raycast(transform.position, bulletTransform.position - transform.position, bulletDistance, playerMask);
                if(stunPlayer)
                {
                    if(!hittedEnemy)
                    {
                        StartCoroutine(stunPlayer.transform.GetComponent<movement>().freeze(this)); 
                    }
                }
            }
            return;
        }
        dir = -(transform.position - target.position).normalized;
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
            if(_enemyType == enemyType.bodyguard)
            {
                if(distance < 5)
                {
                    StartCoroutine(shoot(AttackCooldownBodyguard));
                }
            }
            if(distance < 1)
            {
                StartCoroutine(attack(AttackCooldownNormal));
            }
        }
    }

    IEnumerator attack(float cooldown)
    {
        attacking = true;
        yield return new WaitForSeconds(cooldown);
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, attackRange, Vector2.zero, 0, playerMask);
        if(hit)
        {
            hit.transform.GetComponent<movement>().die();
        }
        attacking = false;
    }
    
    IEnumerator shoot(float cooldown)
    {
        attacking = true;
        if(_enemyType == enemyType.bodyguard)
        {
            if(canAttack)
            {
                stunned = true;
                bulletTransform = Instantiate(bullet, transform.position, transform.rotation).transform;
                Rigidbody2D bulletRb = bulletTransform.GetComponent<Rigidbody2D>();
                bullet Bullet = bulletTransform.GetComponent<bullet>();
                Bullet.Enemy = transform;
                yield return new WaitForFixedUpdate();
                bulletRb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);
                yield return new WaitForSeconds(cooldown);
                stunned = false;
                hittedEnemy = false;
                canAttack = false;
                StartCoroutine(AttackingCooldown());
            }
        }
        attacking = false;
    }

    IEnumerator AttackingCooldown()
    {
        yield return new WaitForSeconds(5);
        canAttack = true;
    }

    public void die(Vector2 direction)
    {
        if(_enemyType == enemyType.driving)
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
        if(_enemyType == enemyType.driving)
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
