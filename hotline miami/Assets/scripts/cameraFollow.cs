using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform target;
    public float speed;
    public Vector3 offset;


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 basicPosition = target.position + offset;
        Vector3 desiredPosition = Vector3.Lerp(transform.position, basicPosition, speed * Time.deltaTime);
        transform.position = desiredPosition;
    }
}
