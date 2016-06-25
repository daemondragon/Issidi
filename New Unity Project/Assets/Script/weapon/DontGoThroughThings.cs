using UnityEngine;
using System.Collections;

public class DontGoThroughThings : MonoBehaviour
{
    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Rigidbody myRigidbody;
    private Collider myCollider;

    private float MinTime = 0;
    //initialize values 
    void Start()
    {
        previousPosition = transform.position;
        myCollider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        if (MinTime < 125)
        {
            MinTime += Time.deltaTime;
            previousPosition = transform.position;
            return;
        }



        float Dist = Vector3.Distance(previousPosition, transform.position);


        RaycastHit hitInfo;
        if (Physics.Raycast(previousPosition, transform.forward * -1, out hitInfo))
        {


            if (!hitInfo.collider || (hitInfo.distance > Dist))
            {
                previousPosition = transform.position;
                return;
            }


            Debug.Log(hitInfo.transform.name);

            if (hitInfo.collider.isTrigger)
            {
                hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);
            }

            if (myCollider.isTrigger)
            {
                myCollider.SendMessage("OnTriggerEnter", hitInfo.collider);
            }
            previousPosition = transform.position;
        }


    }
}