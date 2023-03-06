using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    [SerializeField] private GameObject deadBodyPrefab;
    [SerializeField] private float dyingForce;
    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        target = GameObject.Find("player").transform;
    }
    
    void Update()
    {
        agent.SetDestination(target.position);
    }

    public void die(Vector2 direction)
    {
        Rigidbody2D deadBody = Instantiate(deadBodyPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        deadBody.AddForce(direction * dyingForce, ForceMode2D.Impulse);
        Destroy(gameObject);
    }
}
