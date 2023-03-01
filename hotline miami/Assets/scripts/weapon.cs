using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public enum weapon_type {
        bat,
        rifle
    };
    public weapon_type weaponType;
    private Animator anim;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float radius;
    [SerializeField] private float distance;

    public void attack()
    {
        if(weaponType == weapon_type.bat) {
            RaycastHit2D[] hit =  Physics2D.CircleCastAll(transform.position, radius, transform.forward, distance, enemyMask);
            for (int i = 0; i < hit.Length; i++)
            {
                hit[i].transform.GetComponent<enemy>().die();
            }
        }
        else if(weaponType == weapon_type.rifle) {

        }
    }
}
