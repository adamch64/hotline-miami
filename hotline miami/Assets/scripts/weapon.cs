using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public enum weapon_type {
        bat,
        rifle
    };
    public weapon_type weaponType;
    private Animator anim;
    private Rigidbody2D rb;
    public Transform player;
    [HideInInspector] public bool thrown;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float radius;
    [SerializeField] private float distance;
    private int state = -1;
    public bool pickedUp;

    void Start() 
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        anim.SetBool("picked up", pickedUp);
        if(!thrown)
            return;
        if(rb.velocity.magnitude > 0.5f) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0, enemyMask);
            if(hit) {
                hit.transform.GetComponent<enemy>().die(-hit.normal);
                rb.velocity /= 10;
                thrown = false;
            }
        }
    }
    public void attack()
    {
        if(weaponType == weapon_type.bat) {
            state++;
            if(state > 1)
                state = 0;
            anim.SetTrigger("attack");
            anim.SetInteger("state", state);
            RaycastHit2D[] hit =  Physics2D.CircleCastAll(transform.position, radius, transform.forward, distance, enemyMask);
            for (int i = 0; i < hit.Length; i++)
            {
                hit[i].transform.GetComponent<enemy>().die(-hit[i].normal);
            }
        }
        else if(weaponType == weapon_type.rifle) {

        }
    }
}
