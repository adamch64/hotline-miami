using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public Transform Enemy;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        destroySelf(Enemy.GetComponent<enemy>().AttackCooldownBodyguard);
    }
    void Update()
    {
        if(Enemy != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, Enemy.position);
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    public void destroySelf(float Cooldown)
    {
        Destroy(gameObject, Cooldown);
    }
}
