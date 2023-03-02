using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    [SerializeField] private GameObject deadBodyPrefab;
    [SerializeField] private float dyingForce;
    public void die(Vector2 direction)
    {
        Rigidbody2D deadBody = Instantiate(deadBodyPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        deadBody.AddForce(direction * dyingForce, ForceMode2D.Impulse);
        Destroy(gameObject);
    }
}
