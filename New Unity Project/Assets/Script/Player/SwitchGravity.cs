using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Deplacement))]

public class SwitchGravity : MonoBehaviour
{
    private float DistanceMax = 2.0f;
    Deplacement d;
    Stats stats;
    int layerMask = 9;

    void Start()
    {
        d = GetComponent<Deplacement>();
        stats = GetComponent<Stats>();
    }


    void Update()
    {
        if (stats && !stats.CanMovePlayer)
            return;

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
        if (Physics.Raycast(start_position, transform.forward, out info, DistanceMax, layerMask))
        {
            min_distance = MinCustom(min_distance, info.distance, ref direction, Deplacement.Flip.Back);
        }
        if (Physics.Raycast(start_position, -transform.forward, out info, DistanceMax, layerMask))
        {
            min_distance = MinCustom(min_distance, info.distance, ref direction, Deplacement.Flip.Front);
        }
        if (Physics.Raycast(start_position, transform.right, out info, DistanceMax, layerMask))
        {
            min_distance = MinCustom(min_distance, info.distance, ref direction, Deplacement.Flip.Right);
        }
        if (Physics.Raycast(start_position, -transform.right, out info, DistanceMax, layerMask))
        {
            min_distance = MinCustom(min_distance, info.distance, ref direction, Deplacement.Flip.Left);
        }

        //if can switch gravity, switch
        if (min_distance < Mathf.Infinity)
            d.DoFlip(direction);
    }

    float MinCustom(float OldMinimum, float NewMinimum, ref Deplacement.Flip ResultDeplacement, Deplacement.Flip NewDeplacement)
    {
        if (OldMinimum < NewMinimum)
        {
            return OldMinimum;
        }
        else
        {
            ResultDeplacement = NewDeplacement;
            return NewMinimum;
        }
    }

}
