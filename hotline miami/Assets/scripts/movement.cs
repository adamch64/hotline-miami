using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    [Header("keybinds")]
    [SerializeField] private KeyCode pickupButton = KeyCode.Space;
    [Header("movement")]
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    float x,y;
    Vector2 mouseWorldPosition;
    [Header("weapons")]
    public Transform currentWeapon;
    private bool haveWeapon;
    [SerializeField] private LayerMask weaponMask;
    [SerializeField] private float range;
    [SerializeField] private Transform weaponPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        MousePosition();
        RotatePlayer();
        MovePlayer();
        PickingUpWeapons();
        holdWeapon();
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

    void PickingUpWeapons()
    {
        RaycastHit2D pickUp = Physics2D.CircleCast(transform.position, range, Vector2.zero, 0, weaponMask);
        if(!Input.GetKeyDown(pickupButton))
            return;
        if(pickUp)
        {
            currentWeapon = pickUp.transform;
            currentWeapon.parent = weaponPosition;
            haveWeapon = true;
        }
    }

    void holdWeapon()
    {
        if(!haveWeapon)
            return;
        currentWeapon.position = weaponPosition.position;
    }
}
