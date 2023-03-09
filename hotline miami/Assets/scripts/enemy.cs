using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public enum states {
        patrol,
        chase
    }
    private states currentState;
    [SerializeField] private GameObject deadBodyPrefab;
    [SerializeField] private float dyingForce;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask playerMask, enemyMask;
    private NavMeshAgent agent;
    [SerializeField] private float detectionSpeed;
    

    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentState = states.patrol;
        // target = GameObject.Find("player").transform;
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
                // else
                // {
                //     checkEnemy = Physics2D.CircleCast(transform.position, 4, Vector2.zero, 0, playerMask);
                //     if(checkEnemy)
                //     {
                //         if(Mathf.Abs(checkEnemy.transform.GetComponent<movement>().inputVector.magnitude) > detectionSpeed)
                //         {
                //             currentState = states.chase;
                //             return;
                //         }
                //     }
                // }
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
        Destroy(gameObject);
    }
}
