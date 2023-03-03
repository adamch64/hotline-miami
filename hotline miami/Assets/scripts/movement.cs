using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    [Header("keybinds")]
    [SerializeField] private KeyCode pickupButton = KeyCode.Space;
    [SerializeField] private KeyCode attackButton = KeyCode.Mouse0;
    [SerializeField] private KeyCode useButton = KeyCode.E;
    [Header("movement")]
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    float x,y;
    Vector2 mouseWorldPosition;
    [Header("weapons")]
    public Transform currentWeapon;
    private bool haveWeapon;
    [SerializeField] private LayerMask weaponMask;
    [SerializeField] private LayerMask shoppingCartMask;
    [SerializeField] private float range;
    [SerializeField] private Transform weaponPositionLeft, weaponPositionRight;
    private Transform currentWeaponPosition;
    [SerializeField] private float throwPower;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentWeaponPosition = weaponPositionRight;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        MousePosition();
        RotatePlayer();
        MovePlayer();
        holdWeapon();
        if(haveWeapon)
            attacking();
        useItem();
    }

    void MyInput() 
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
    }

    void RotatePlayer()
    {
        Vector2 direction = (mouseWorldPosition - (Vector2) transform.position).normalized;
        transform.up = direction;
    }

    void MovePlayer()
    {
        Vector2 moveVector = new Vector2(x, y).normalized * speed;
        rb.velocity = moveVector;
    }

    void MousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
    }

    void holdWeapon()
    {
        if(!haveWeapon)
        {
            RaycastHit2D pickUp = Physics2D.CircleCast(transform.position, range, Vector2.zero, 0, weaponMask);
            if(!Input.GetKeyDown(pickupButton))
                return;
            if(pickUp)
            {
                currentWeapon = pickUp.transform;
                currentWeapon.parent = transform;
                currentWeapon.GetComponent<Animator>().enabled = true;
                currentWeapon.GetComponent<weapon>().player = transform;
                currentWeapon.gameObject.layer = 0;
                haveWeapon = true;
                currentWeapon.GetComponent<weapon>().pickedUp = haveWeapon;
            }
        }
        else 
        {    
            if(!Input.GetKeyDown(pickupButton))
            {
                currentWeapon.position = currentWeaponPosition.position;
                currentWeapon.rotation = transform.rotation;
                return;
            }
            else
            {
                haveWeapon = false;
                currentWeapon.GetComponent<Rigidbody2D>().AddForce(transform.up * throwPower, ForceMode2D.Impulse);
                currentWeapon.GetComponent<weapon>().thrown = true;
                currentWeapon.GetComponent<weapon>().pickedUp = haveWeapon;
                currentWeapon.parent = null;
                currentWeapon.gameObject.layer = 6;
                currentWeapon = null;
            }
        }
    }

    void attacking()
    {
        if(Input.GetKeyDown(attackButton)) {
            currentWeapon.GetComponent<weapon>().attack();
        }
    }

    void useItem()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, range, Vector2.zero, 0, shoppingCartMask);
        if(hit) {
            if(Input.GetKeyDown(useButton)) {
                hit.transform.GetComponent<shoppingCart>().used = true;
                hit.transform.gameObject.layer = 0;
            }
        }
    }
}
