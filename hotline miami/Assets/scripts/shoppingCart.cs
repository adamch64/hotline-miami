using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoppingCart : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] private float speed;
    public bool used;
    // Update is called once per frame
    void Update()
    {
        if(used) {
            rb.velocity = transform.up * speed;
        }
    }
}
