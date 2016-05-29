using UnityEngine;

[RequireComponent(typeof(ColorModifier))]

public class Spawner : MonoBehaviour {
    public Stats.Team team;

	// Use this for initialization
	void Start ()
    {
        GetComponent<ColorModifier>().SetTeam(team);
	}

    public Vector3 getSpawnPosition(GameObject obj)
    {
        BoxCollider body = obj.GetComponent<BoxCollider>();
        float up = 1.0f;
        if (body)
            up = body.size.y * 0.55f; //Take the half size, but add something to let the player fall down at start.

        return (transform.position + transform.up * up);
    }

    public Quaternion getSpawnRotation(GameObject obj)
    {
        return transform.rotation;
    }
}
