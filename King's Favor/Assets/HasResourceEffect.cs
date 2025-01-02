using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasResourceEffect : HasEffect
{

    public int healthAmount;
    public int coinAmount;
    public int movementAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        GameManager.GetPlayer(0).UpdateHealth(healthAmount);
        GameManager.GetPlayer(0).UpdateCoins(coinAmount);
        GameManager.GetPlayer(0).UpdateMovement(movementAmount);
    }
}
