﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ColorModifier))]

public class Spawner : MonoBehaviour {
    public Stats.Team team;

	// Use this for initialization
	void Start ()
    {
        GetComponent<ColorModifier>().SetTeam(team);
	}

    public void Spawn(GameObject obj)
    {
        PlaceGameObject(obj);
    }

    void PlaceGameObject(GameObject obj)
    {
        BoxCollider body = obj.GetComponent<BoxCollider>();
        float up = 1.0f;
        if (body)
            up = body.size.y * 0.55f; //Take the half size, but add something to let the player fall down at start.

        obj.transform.position = transform.position + transform.up * up;
        obj.transform.rotation = transform.rotation;

        Deplacement deplacement = obj.GetComponent<Deplacement>();
        if (deplacement)
        {
            deplacement.CorrectRotation();
            deplacement.sens = deplacement.findGlobalAxis(transform.up);
        }
    }
}
