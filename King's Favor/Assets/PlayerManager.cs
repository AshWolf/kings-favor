using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int playerNum;

    private int health;
    private int coins;
    private int movement;

    private TextMeshProUGUI healthUI;
    private TextMeshProUGUI coinsUI;
    private TextMeshProUGUI movementUI;

    // Start is called before the first frame update
    void Start()
    {
        healthUI = GameObject.Find("Health").GetComponent<TextMeshProUGUI>();
        coinsUI = GameObject.Find("Coins").GetComponent<TextMeshProUGUI>();
        movementUI = GameObject.Find("Movement").GetComponent<TextMeshProUGUI>();
        UpdateHealth(10);
        UpdateCoins(10);
        UpdateMovement(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(int amount)
    {
        UpdateResource(ref healthUI, ref health, amount);
    }

    public void UpdateCoins(int amount)
    {
        UpdateResource(ref coinsUI, ref coins, amount);
    }

    public void UpdateMovement(int amount)
    {
        UpdateResource(ref movementUI, ref movement, amount);
    }

    void UpdateResource(ref TextMeshProUGUI text, ref int resource, int amount)
    {
        resource += amount;
        if (resource < 0) resource = 0;
        text.text = resource.ToString();
    }

}
