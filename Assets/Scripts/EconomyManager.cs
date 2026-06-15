using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public GoodsManager goodsManager;

    private float budget;
    public float spending;
    public float income;
    public float tax = 0.1f;

    [Header("Simulation Settings")]
    public float tickRateInSeconds = 2.0f;
    private float tickTimer = 0.0f;

    private void Start()
    {
        budget = 10000f;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRateInSeconds)
        {
            runSimulationTick();
            tickTimer = 0.0f;
        }
    }

    private void runSimulationTick()
    {
        goodsManager.produceGoods();
        goodsManager.sellGoods();
        goodsManager.debugGoods();
    }

}
