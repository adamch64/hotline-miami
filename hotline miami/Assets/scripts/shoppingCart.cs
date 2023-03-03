using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoppingCart : MonoBehaviour
{
    [SerializeField] private float speed;
    public bool used = false;
    private bool canUse;
    private BoxCollider2D bc;
    [SerializeField] private LayerMask wallMask, enemyMask;
    private Vector3 moveSpeed;
    // Update is called once per frame

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if(used) {
            RaycastHit2D hitWall = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, transform.up, 0, wallMask);
            if(hitWall) {
                used = false;
            }
            else {
                RaycastHit2D hitEnemy = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, transform.up, 0, enemyMask);
                if(hitEnemy) {
                    hitEnemy.transform.GetComponent<enemy>().die(-hitEnemy.normal);
                }
            }
        }
    }
    void FixedUpdate() 
    {
        if(used) {
            moveSpeed += transform.up * speed * Time.deltaTime;
            transform.position += moveSpeed;
        }
    }
}
