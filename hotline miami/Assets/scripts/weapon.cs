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
    [HideInInspector] public bool thrown;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float radius;
    [SerializeField] private float distance;

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!thrown)
            return;
        if(rb.velocity.magnitude > 0.5f) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0, enemyMask);
            if(hit) {
                hit.transform.GetComponent<enemy>().die();
                rb.velocity /= 5;
                thrown = false;
            }
        }
    }
    public void attack()
    {
        if(weaponType == weapon_type.bat) {
            RaycastHit2D[] hit =  Physics2D.CircleCastAll(transform.position, radius, transform.forward, distance, enemyMask);
            for (int i = 0; i < hit.Length; i++)
            {
                hit[i].transform.GetComponent<enemy>().die();
            }
        }
        else if(weaponType == weapon_type.rifle) {

        }
    }
}
