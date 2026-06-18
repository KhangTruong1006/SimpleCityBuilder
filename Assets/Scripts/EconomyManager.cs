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
        //goodsManager.sellGoods();
        //goodsManager.exportExcessGoods();


    }

    private void calculateIncome()
    {
        float tradeDeficit = calculateExportAndImportDeficit();

       
        
        income = tradeDeficit;
        
        
        budget += income;
    }

    private float calculateExportAndImportDeficit()
    {
        float exportedGoods = goodsManager.exportExcessGoods();
        float importedGoods = 0;
        
        float exportRevenue = exportedGoods * exportRevenuePerUnit;
        float importCost = importedGoods * importCostPerUnit;
        
        float deficit = exportRevenue - importCost;

        return deficit;
    }


    private void displayBudget()
    {
        uiController.displayBudget(budget);
    }
}
