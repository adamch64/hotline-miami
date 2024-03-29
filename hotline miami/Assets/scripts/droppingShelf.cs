using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus;
using NavMeshPlus.Components;
public class droppingShelf : MonoBehaviour
{
    private Animator anim;
    private Vector2 position;
    private float distance = 2;
    public NavMeshPlus.Components.NavMeshSurface surface;

    private BoxCollider2D bc;
    [SerializeField] private LayerMask enemyMask;
    bool canKill;

    [SerializeField] private float detectionDistance;
    
    void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        surface = GameObject.Find("navigation surface").GetComponent<NavMeshPlus.Components.NavMeshSurface>();
        anim = GetComponent<Animator>();
        
    }

    void Update() 
    {
        if(!canKill)
            return;
        KillEnemies();
    }
    public void Drop(Transform player)
    {
        anim.SetTrigger("dropped");
        if(player.position.x <= transform.position.x)
        {
            position = new Vector2(transform.position.x - distance, player.position.y);
        }
        else
        {
            position = new Vector2(transform.position.x + distance, player.position.y);
        }
        player.GetComponent<movement>().TriggerMoving(position);
    }

    public void UpdateNavMesh()
    {
        surface.BuildNavMesh();
    }

    public void startKilling()
    {
        canKill = true;
    }

    public void StopKilling()
    {
        canKill = false;
    }

    void KillEnemies()
    {
        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size / 1.5f, 0, transform.up, 0.5f, enemyMask);
        if(hit)
            hit.transform.GetComponent<enemy>().die(-hit.normal);
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
