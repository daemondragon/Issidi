using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CharacterFactory : NetworkBehaviour
{
    public List<GameObject> weaponized_characters;

    [Command]
    public void Cmd_CreatePlayer(int weapon_index, Stats.Team team)
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

        Stats stats = character.GetComponent<Stats>();
        if (stats)
        {
            stats.team = team;
            stats.ReturnToSpawn();
        }

        NetworkServer.ReplacePlayerForConnection(connectionToClient, character, playerControllerId);
        NetworkServer.Destroy(gameObject);
    }
}
