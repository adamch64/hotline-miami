using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoppingDoor : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Vector2 range;
    private Animator anim;

    void Start() 
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, range, 0, Vector2.zero, 0, playerMask);
        anim.SetBool("playerNearby", hit);
    }
}
