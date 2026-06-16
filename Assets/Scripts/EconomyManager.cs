using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public GoodsManager goodsManager;
    public UIController uiController;

    private float budget;
    public float spending;
    public float income;
    public float tax = 0.1f;

    public float exportRevenuePerUnit = 100f;
    public float importCostPerUnit = 50f;


    [Header("Simulation Settings")]
    public float tickRateInSeconds = 2.0f;
    private float tickTimer = 0.0f;

    private void Start()
    {
        budget = 10000f;
        updateBudget();
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
        goodsManager.handleLogistics();

        updateBudget();
    }

    private void updateBudget()
    {
        float exportIncome = goodsManager.exportExcessGoods();
        budget += exportIncome * exportRevenuePerUnit;
        
        
        uiController.displayBudget(budget);
    }
}
