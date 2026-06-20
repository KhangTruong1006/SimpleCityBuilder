using System;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public ResourcesManager resourcesManager;
    public UIController uiController;

    private float budget;
    public float spending;
    public float income;
    [Range(0f, 1f)]
    public float tax = 0.1f;

    public float exportRevenuePerUnit = 10f;
    public float importCostPerUnit = 25f;

    public float productionCostPerUnit = 2f;
    public float salePricePerUnit = 3f;

    private float producedGoods;
    private float saledGoods;

    float exported = 0;
    float imported = 0;



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
        handleEconomy();
        
        displayBudget();
    }

    private void handleEconomy()
    {
        float tradeDeficit = calculateExportAndImportDeficit();
        float productionCost = calculateProductionCost();
        float saleRevenue = calculateSaleRevenue();
        
        calculateIncome(tradeDeficit, productionCost, saleRevenue);
    }

    public void handleLogistics()
    {
        // === FIX LOGICS
        producedGoods = resourcesManager.produceGoods();

        if (resourcesManager.isExportThreshold())
        {
            exported = resourcesManager.exportSurplus();
        }

        if (resourcesManager.isStockAndProductionUnderDemand()) 
        {
            imported = resourcesManager.importGoods();
        }
        
        saledGoods = resourcesManager.sellGoods();
    }

    private void calculateIncome(float tradeDeficit, float production, float sales)
    { 
        income = tradeDeficit + (sales - production);
        
        
        budget += income;
    }

    private float calculateProductionCost()
    {
        return producedGoods * productionCostPerUnit;
    }

    private float calculateSaleRevenue()
    {
        return saledGoods * salePricePerUnit;
    }

    private float calculateExportAndImportDeficit()
    {   
        float exportRevenue = exported * exportRevenuePerUnit;
        float importCost = imported * importCostPerUnit;
        
        float tradeDeficit = exportRevenue - importCost;
        
        return tradeDeficit;
    }


    private void displayBudget()
    {
        uiController.displayBudget(budget);
    }
}
