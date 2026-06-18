using System;
using Unity.VisualScripting;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public GoodsManager goodsManager;
    public UIController uiController;

    private float budget;
    public float spending;
    public float income;
    [Range(0f, 1f)]
    public float tax = 0.1f;

    public float exportRevenuePerUnit = 10f;
    public float importCostPerUnit = 25f;

    public float productionCostPerUnit = 1f;
    public float productCostPerUnit = 2f;

    private float tradeDeficit;

    [Header("Simulation Settings")]
    public float tickRateInSeconds = 2.0f;
    private float tickTimer = 0.0f;

    private void Start()
    {
        budget = 10000f;
        displayBudget();
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
        handleLogistics();
        calculateIncome();
        displayBudget();
    }


    public void handleLogistics()
    {
        goodsManager.produceGoods();
        float exported = goodsManager.exportExcessGoods();
        float imported = goodsManager.importGoods();
        goodsManager.sellGoods();

        calculateExportAndImportDeficit(exported, imported);
    }

    private void calculateIncome()
    { 
        income = tradeDeficit;
        
        
        budget += income;
    }

    private void calculateExportAndImportDeficit(float exported, float imported)
    {   
        float exportRevenue = exported * exportRevenuePerUnit;
        float importCost = imported * importCostPerUnit;
        
        tradeDeficit = exportRevenue - importCost;
    }


    private void displayBudget()
    {
        uiController.displayBudget(budget);
    }
}
