using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drivingEnemy : MonoBehaviour
{
    [Header("declarations")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private BoxCollider2D enemyCollider;
    [SerializeField] private LayerMask playerMask;
    [Header("movement")]
    private float speed;
    [SerializeField] private float patrolSpeed, chaseSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform[] checkpoint;
    private int currentCheckpoint = 0;
    public enum State {
        patrol,
        chase
    }
    public State currentState;
    [Header("patrol")]
    [SerializeField] private float detectDistance;
    [SerializeField] private float minimumCheckpointDistance;
    [Header("dying")]
    public bool Alive;

    void Start() 
    {
        transform.position = checkpoint[currentCheckpoint].position;
        enemyCollider.transform.GetComponent<enemy>().drivingEnemy = this;
    }

    void Update()
    {
        if(!Alive)
            return;
        RaycastHit2D hittedPlayer = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.zero, 0, playerMask);
        if(hittedPlayer)
        {
            movement player;
            if(player = hittedPlayer.transform.GetComponent<movement>())
            {
                player.die();
            }
        }
        RaycastHit2D checkPlayer = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size + new Vector3(1.5f, 0, 0), 0, transform.up, detectDistance,playerMask);
        if(checkPlayer)
        {
            currentState = State.chase;
        }
        else
        {
            currentState = State.patrol;
        }
        float distance = Vector2.Distance(transform.position, checkpoint[currentCheckpoint].position);
        if(distance <= minimumCheckpointDistance)
        {
            currentCheckpoint++;
            if(currentCheckpoint > checkpoint.Length - 1)
            {
                currentCheckpoint = 0;
            }
        }
        else
        {
            Vector3 basicRotation = checkpoint[currentCheckpoint].position - transform.position;
            Vector3 desiredRotation = Vector2.MoveTowards(transform.up, basicRotation, rotationSpeed * Time.deltaTime); 
            transform.up = desiredRotation;
            transform.position = Vector2.MoveTowards(transform.position, checkpoint[currentCheckpoint].position, speed * Time.deltaTime);
        }
        if(currentState == State.patrol)
        {
            speed = patrolSpeed;
        }
        else if(currentState == State.chase)
        {
            speed = chaseSpeed;
        }
    }
}
