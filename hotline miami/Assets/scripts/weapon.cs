using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public enum weapon_type {
        bat,
        apple,
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
    public movement currentUser;

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
            RaycastHit2D hit =  Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, transform.up, 1.5f, enemyMask);
            if(hit) 
                hit.transform.GetComponent<enemy>().die(-hit.normal);
        }
        if(!thrown)
            return;
        anim.enabled = false;
        if(rb.velocity.magnitude > 0.5f) {
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
        }
        else if(weaponType == weapon_type.apple) {
            Throwing(currentUser);
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

    public void pickingUp(movement _player)
    {
        currentUser = _player;
        _player.currentWeapon = transform;
        transform.parent = _player.transform;
        anim.enabled = true;
        thrown = false;
        player = _player.transform;
        gameObject.layer = 0;
        _player.haveWeapon = true;
        transform.GetComponent<weapon>().pickedUp = _player.haveWeapon;
    }

    public void Throwing(movement _player)
    {
        currentUser = null;
        _player.haveWeapon = false;
        rb.AddForce(_player.transform.up * _player.throwPower, ForceMode2D.Impulse);
        thrown = true;
        pickedUp = _player.haveWeapon;
        transform.parent = null;
        gameObject.layer = 6;
        _player.currentWeapon = null;
    }
}
