using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform target;
    private movement player;
    public float speed;
    [SerializeField] private float xClamp, yClamp;
    public Vector3 offset;
    private Vector3 MousePosition;
    

    void Start()
    {
        if(target.GetComponent<movement>())
        {
            player = target.GetComponent<movement>();
        }
    }

    void Update() 
    {
        MousePosition = player.mouseWorldPosition;
    }
    void FixedUpdate()
    {
        Vector3 firstPosition = new Vector3((target.position.x + MousePosition.x)/2,(target.position.y + MousePosition.y)/2,0);
        Vector3 basicPosition = new Vector3((firstPosition.x + target.position.x)/2,(firstPosition.y + target.position.y)/2,0);
        Vector3 clampedPosition = new Vector3(Mathf.Clamp(basicPosition.x, target.position.x - xClamp, target.position.x + xClamp), Mathf.Clamp(basicPosition.y, target.position.y - yClamp, target.position.y + yClamp),0); 
        Vector3 desiredPosition = Vector3.Lerp(transform.position, clampedPosition + offset, speed * Time.deltaTime);
        transform.position = desiredPosition;
    }
}
