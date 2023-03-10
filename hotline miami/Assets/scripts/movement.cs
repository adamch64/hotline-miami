using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    [Header("keybinds")]
    [SerializeField] private KeyCode pickupButton = KeyCode.Space;
    [SerializeField] private KeyCode leftAttackButton = KeyCode.Mouse0;
    [SerializeField] private KeyCode rightAttackButton = KeyCode.Mouse1;
    [SerializeField] private KeyCode useButton = KeyCode.E;
    [Header("movement")]
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    private bool canMove;
    private Vector2 droppedPalettePosition;
    [SerializeField] private Vector2 droppedPaletteRange;
    float x,y;
    float lerpedX, lerpedY;
    public Vector2 inputVector;
    Vector2 mouseWorldPosition;
    [Header("weapons")]
    public Transform currentWeapon;
    public bool haveWeapon;
    [SerializeField] private LayerMask weaponMask;
    [SerializeField] private LayerMask shoppingCartMask;
    [SerializeField] private LayerMask movableShelfMask;
    [SerializeField] private float range;
    [SerializeField] private Transform weaponPositionLeft, weaponPositionRight;
    public Transform leftWeapon, rightWeapon;
    public float throwPower;
    public bool twoWeapons;
    public bool isNigger;
    [Header("score")]
    private float score;
    private float scoreMultipier;
    private float scoreMultipierCooldown;
    [SerializeField] private float MaxScoreMultipierCooldown;
    [SerializeField] private Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        MousePosition();
        handleScore();
        printScore();
        if(leftWeapon != null && rightWeapon != null)
        {
            twoWeapons = true;
        }
        else
        {
            twoWeapons = false;
        }
        if(canMove)
        {
            RotatePlayer();
            MovePlayer();
        }
        else
        {
            StunnedPlayer();
        }
        PickingUpWeapon();
        attacking();
        useItem();
        DropPalette();
        movePosition(droppedPalettePosition);
    }

    void MyInput() 
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        lerpedX = Mathf.Lerp(lerpedX, x, 4 * Time.deltaTime);
        lerpedY = Mathf.Lerp(lerpedY, y, 4 * Time.deltaTime);
        inputVector = new Vector2(lerpedX, lerpedY);
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

    public void holdWeapon(Transform _weapon)
    {
        _weapon.TryGetComponent<weapon>(out weapon weapon);
        if(weapon.InRightHand)
        {
            weapon.twoWeapons = twoWeapons;
            _weapon.position = weaponPositionRight.position;
        }
        else
        {
            weapon.twoWeapons = twoWeapons;
            _weapon.position = weaponPositionLeft.position;
        }
    }

    void PickingUpWeapon()
    {
        RaycastHit2D pickUp = Physics2D.CircleCast(transform.position, range, Vector2.zero, 0, weaponMask);
        if(!Input.GetKeyDown(pickupButton))
            return;
        if(pickUp)
        {
            pickUp.transform.GetComponent<weapon>().pickingUp(transform.GetComponent<movement>());
        }
    }

    void attacking()
    {
        if(Input.GetKeyDown(leftAttackButton)) {
            if(leftWeapon != null)
                leftWeapon.GetComponent<weapon>().attack();
        }
        if(Input.GetKeyDown(rightAttackButton)) {
            if(rightWeapon != null)
                rightWeapon.GetComponent<weapon>().attack();
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

    void handleScore()
    {
        if(scoreMultipierCooldown <= 0)
        {
            scoreMultipier = 1;
        }
        else
        {
            scoreMultipierCooldown -= Time.deltaTime;
        }
    }

    void printScore()
    {
        scoreText.text = score.ToString() + "   " + scoreMultipier.ToString("F2") + "x";
    }

    public void AddScore()
    {
        scoreMultipierCooldown = MaxScoreMultipierCooldown;
        scoreMultipier += 0.25f;
        score += 100 * scoreMultipier;
    }
}
