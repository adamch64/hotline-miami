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
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    public Transform player;
    public movement currentUser;
    public bool thrown;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float radius;
    [SerializeField] private float distance;
    [SerializeField] private float maxCooldown, twoWeaponCooldown;
    private float cooldown;
    private int state = -1;
    public bool pickedUp;
    public bool attacking = false;
    public bool InRightHand = true;
    public bool twoWeapons = false;
    [SerializeField] private float detectionDistance;


    void Start() 
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if(currentUser != null)
        {
            currentUser.GetComponent<movement>().holdWeapon(transform);
            anim.SetBool("two weapons", twoWeapons);
            anim.SetBool("right hand", InRightHand);
            if(weaponType == weapon_type.bat && InRightHand && twoWeapons)
            {
                Transform z = currentUser.leftWeapon;
                InRightHand = false;
                currentUser.leftWeapon = currentUser.rightWeapon;
                currentUser.rightWeapon = z;
                z.GetComponent<weapon>().InRightHand = true;
            }
        }
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
            warnEnemies();

    }
    public void attack()
    {   
        if(cooldown > 0)
                return;
        if(weaponType == weapon_type.bat) {
            state++;
            if(state > 1)
                state = 0;
            anim.SetTrigger("attack");
            anim.SetInteger("state", state);
        }
        else if(weaponType == weapon_type.apple) {
            Throwing(currentUser);
        }
        if(twoWeapons)
            cooldown = twoWeaponCooldown;
        else
            cooldown = maxCooldown;
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
        if(_player.rightWeapon == null)
        {
            InRightHand = true;
            _player.rightWeapon = transform;
        }
        else if(_player.leftWeapon == null)
        {
            InRightHand = false;
            _player.leftWeapon = transform;
        }
        else
        {
            return;
        }
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
        if(InRightHand)
        {
            _player.rightWeapon = null;
            if(_player.leftWeapon != null)
            {
                _player.rightWeapon = _player.leftWeapon;
                _player.rightWeapon.GetComponent<weapon>().InRightHand = true;
                _player.leftWeapon = null;
            }
        }
        else
        {
            _player.leftWeapon = null;
        }
        _player.twoWeapons = false;
        currentUser = null;
        rb.AddForce(_player.transform.up * _player.throwPower, ForceMode2D.Impulse);
        thrown = true;
        pickedUp = false;
        transform.parent = null;
        gameObject.layer = 6;
        _player.currentWeapon = null;
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
