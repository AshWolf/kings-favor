using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static PlayerManager[] players = { null, null, null, null };

    // Start is called before the first frame update
    void Start()
    {
        players[0] = gameObject.AddComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static PlayerManager GetPlayer(int index)
    {
        if (index < players.Length)
        {
            Debug.LogError("Player " + index + " does not exist");
            return null;
        }
        else return players[index];
    }
}
