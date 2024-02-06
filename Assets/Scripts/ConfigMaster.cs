using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigMaster
{
    // Set Spawnrate for Coins
    public float coinSpawnRateMin = 0.5f;
    public float coinSpawnRateMax = 1.2f;

    //value Coins
    public int coinValueMin = 5;
    public int coinValueMax = 10;

    //prices for items
    public int priceBanana = 25;
    public int priceEnergy = 30;
    public int priceTelevision = 65;
    public int priceToolBelt = 75;

    public int[] itemPrices = new int[4];  

    //Duration Times for Items
    public float durationTimeTV = 15.0f;
    public float durationTimeEnergy = 8.0f;
    public float durationTimeBanana = 3.0f;
    public int runsToolBelt = 5;
    public float removalTimeTV = 3.5f;

    //speeds
    public int workerDefaultSpeed = 1;
    public int playerDefaultSpeed = 1;


    //progress for a complete work (%)
    public float workprogressForWorker = 0.05f;
    public float workingTime = 3f;


    public ConfigMaster(){
        itemPrices[0] = priceBanana;    
        itemPrices[1] = priceEnergy;
        itemPrices[2] = priceTelevision;
        itemPrices[3] = priceToolBelt;  
    }
}
