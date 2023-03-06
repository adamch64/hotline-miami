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
    private bool canMove;
    private Vector2 droppedPalettePosition;
    [SerializeField] private Vector2 droppedPaletteRange;
    float x,y;
    Vector2 mouseWorldPosition;
    [Header("weapons")]
    public Transform currentWeapon;
    public bool haveWeapon;
    [SerializeField] private LayerMask weaponMask;
    [SerializeField] private LayerMask shoppingCartMask;
    [SerializeField] private LayerMask movableShelfMask;
    [SerializeField] private float range;
    [SerializeField] private Transform weaponPositionLeft, weaponPositionRight;
    private Transform currentWeaponPosition;
    public float throwPower;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody2D>();
        currentWeaponPosition = weaponPositionRight;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        MousePosition();
        if(canMove)
        {
            RotatePlayer();
            MovePlayer();
        }
        else
        {
            StunnedPlayer();
        }
        holdWeapon();
        if(haveWeapon)
            attacking();
        useItem();
        DropPalette();
        movePosition(droppedPalettePosition);
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
    void StunnedPlayer()
    {
        rb.velocity = Vector2.zero;
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
                pickUp.transform.GetComponent<weapon>().pickingUp(transform.GetComponent<movement>());
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
                currentWeapon.GetComponent<weapon>().Throwing(transform.GetComponent<movement>());
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

    void DropPalette()
    {
        if(!Input.GetKeyDown(useButton))
            return;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, droppedPaletteRange, 0, Vector2.zero, 0, movableShelfMask);
        if(!hit)
            return;
        hit.transform.GetComponent<droppingShelf>().Drop(transform);
    }

    public void TriggerMoving(Vector2 position)
    {
        canMove = false;
        droppedPalettePosition = position;
        transform.GetComponent<Collider2D>().enabled = false;
    }

    void movePosition(Vector2 position)
    {
        if(canMove)
        {
            return;
        }
        float distance = Vector2.Distance(transform.position, position);
        transform.position = Vector2.MoveTowards(transform.position, position, (speed/2) * Time.deltaTime);
        if(distance < 0.02f)
        {
            canMove = true;
            transform.GetComponent<Collider2D>().enabled = true;
        }
    }
}
