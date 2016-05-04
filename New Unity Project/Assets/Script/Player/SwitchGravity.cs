using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Deplacement))]

public class SwitchGravity : MonoBehaviour
{
    private float DistanceMax = 2.0f;
    Deplacement d;
    int layerMask = 9;
    
    void Start()
    {
        d = transform.GetComponent<Deplacement>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryChangeDirection();
    }

    private void TryChangeDirection()
    {
        float min_distance = Mathf.Infinity;
        Vector3 start_position = transform.position + transform.up * 0.2f;
        RaycastHit info;
        Deplacement.Flip direction = Deplacement.Flip.Back;

        //add forward * 0.50001f to get outside the player
        if (Physics.Raycast(start_position, transform.forward, out info, DistanceMax, layerMask) && info.distance < min_distance)
        {
            min_distance = info.distance;
            direction = Deplacement.Flip.Back;
        }
        else if (Physics.Raycast(start_position, -transform.forward, out info, DistanceMax, layerMask) && info.distance < min_distance)
        {
            min_distance = info.distance;
            direction = Deplacement.Flip.Front;
        }
        else if (Physics.Raycast(start_position, transform.right, out info, DistanceMax, layerMask) && info.distance < min_distance)
        {
            min_distance = info.distance;
            direction = Deplacement.Flip.Right;
        }
        else if (Physics.Raycast(start_position, -transform.right, out info, DistanceMax, layerMask) && info.distance < min_distance)
        {
            min_distance = info.distance;
            direction = Deplacement.Flip.Left;
        }

        //if can switch gravity, switch
        if (min_distance < Mathf.Infinity)
            d.DoFlip(direction);
    }
}
