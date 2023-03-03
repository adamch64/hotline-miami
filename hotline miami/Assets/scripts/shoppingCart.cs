using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoppingCart : MonoBehaviour
{
    [SerializeField] private float speed;
    public bool used = false;
    private bool canUse;
    private BoxCollider2D bc;
    [SerializeField] private LayerMask wallMask;
    private Vector3 moveSpeed;
    // Update is called once per frame

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if(used) {
            RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size,0, transform.up, 0, wallMask);
            if(hit) {
                used = false;
            }
        }
    }
    void FixedUpdate() 
    {
        if(used) {
            moveSpeed += transform.up * speed * Time.deltaTime;
        }
    }
}
