using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CharacterFactory : NetworkBehaviour
{
    public List<GameObject> weaponized_characters;

    [Command]
    public void Cmd_CreatePlayer(int weapon_index, Stats.Team team, bool auto_destroy)
    {
        if (weapon_index < 0 || weapon_index >= weaponized_characters.Count)
        {
            Debug.Log(weapon_index + " outside weaponized_characters list");
            return;
        }

        GameObject character = Instantiate(weaponized_characters[weapon_index]);
        if (!character)
        {
            Debug.Log("Instantiate character failed");
            return;
        }

        character.transform.position = new Vector3(0, 0, 0);

        GameObject[] spawns = null;

        if (team == Stats.Team.None)
            spawns = GameObject.FindGameObjectsWithTag("WhiteSpawn");
        else if (team == Stats.Team.Blue)
            spawns = GameObject.FindGameObjectsWithTag("BlueSpawn");
        else if (team == Stats.Team.Orange)
            spawns = GameObject.FindGameObjectsWithTag("OrangeSpawn");

        if (spawns == null || spawns.Length <= 0)
            Debug.Log("Error : no " + team + " spawn register in CharacterFactory, player will spawn at (0;0;0)");
        else //Spawn at a random location
            spawns[Random.Range(0, spawns.Length)].GetComponent<Spawner>().Spawn(character);

        NetworkServer.ReplacePlayerForConnection(connectionToClient, character, playerControllerId);
        if (auto_destroy)
            NetworkServer.Destroy(gameObject);
    }
}
