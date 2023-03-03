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
    public Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    public Transform player;
    public bool thrown;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float radius;
    [SerializeField] private float distance;
    [SerializeField] private float maxCooldown;
    private float cooldown;
    private int state = -1;
    public bool pickedUp;
    public bool attacking = false;

    void Start() 
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if(cooldown > 0)
            cooldown -= Time.deltaTime;
        anim.SetBool("picked up", pickedUp);
        if(attacking)
        {
            RaycastHit2D hit =  Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.zero, 0, enemyMask);
            if(hit) 
                hit.transform.GetComponent<enemy>().die(-hit.normal);
        }
        if(!thrown)
            return;
        anim.enabled = false;
        if(rb.velocity.magnitude > 0.5f) {
            // rb.rotation += 500 * Time.deltaTime;
            RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.zero, 0, enemyMask);
            if(hit) {
                hit.transform.GetComponent<enemy>().die(-hit.normal);
                rb.velocity /= 10;
                
            }
        }
        else
            thrown = false;

    }
    public void attack()
    {
        if(weaponType == weapon_type.bat) {
            if(cooldown > 0)
                return;
            cooldown = maxCooldown;
            state++;
            if(state > 1)
                state = 0;
            anim.SetTrigger("attack");
            anim.SetInteger("state", state);
            // RaycastHit2D hit =  Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0, enemyMask);
            // if(hit) 
            //     hit.transform.GetComponent<enemy>().die(-hit.normal);
            // for (int i = 0; i < hit.Length; i++)
            // {
            //     hit[i].transform.GetComponent<enemy>().die(-hit[i].normal);
            // }
        }
        else if(weaponType == weapon_type.rifle) {

        }
    }

    public void startAttacking()
    {
        attacking = true;
    }

    public void stopAttacking()
    {
        attacking = false;
    }

    public void pickingUp(movement player)
    {
        player.currentWeapon = transform;
        transform.parent = player.transform;
        anim.enabled = true;
        transform.GetComponent<weapon>().player = player.transform;
        gameObject.layer = 0;
        player.haveWeapon = true;
        transform.GetComponent<weapon>().pickedUp = player.haveWeapon;
    }

    public void Throwing(movement player)
    {
        player.haveWeapon = false;
        rb.AddForce(player.transform.up * player.throwPower, ForceMode2D.Impulse);
        transform.GetComponent<weapon>().thrown = true;
        transform.GetComponent<weapon>().pickedUp = player.haveWeapon;
        transform.parent = null;
        gameObject.layer = 6;
        player.currentWeapon = null;
    }
}
