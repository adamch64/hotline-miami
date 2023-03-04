using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droppingShelf : MonoBehaviour
{
    private Animator anim;
    private Vector2 position;
    void Start() 
    {
        anim = GetComponent<Animator>();
    }
    public void Drop(Transform player)
    {
        anim.SetTrigger("dropped");
        if(player.position.x <= transform.position.x)
        {
            position = new Vector2(transform.position.x - 2, player.position.y);
        }
        else
        {
            position = new Vector2(transform.position.x + 2, player.position.y);
        }
        player.GetComponent<movement>().TriggerMoving(position);
    }
}
