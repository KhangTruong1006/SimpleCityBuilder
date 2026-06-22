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

    private bool triggeredExport = false;


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
        
        updateBudget(); 
    }


    private void handleLogistics()
    {

        float produced = resourcesManager.produceGoods();
        float sold = resourcesManager.sellGoods();

        resourcesManager.importDemand = 0f;
        float exported = handleExport();
        float imported = handleImport(sold, resourcesManager.salesRatePerTimeUnit);

        calculateIncome(produced,sold,exported,imported);

    }

    private float handleExport()
    {
        if (resourcesManager.isExportThreshold() && !triggeredExport)
        {
            triggerExport();
        }


        if (triggeredExport)
        {
            if (resourcesManager.isSurplusAvaialbe())
            {
                return resourcesManager.exportSurplus();
            }
            
            deactiveExport();
        }
        
        return 0f;
    }

    private float handleImport(float sold, float goodsDemand)
    {
        if (!resourcesManager.isSoldUnderDemand(sold))
        {
            return 0f;
        }
        
        float demand = goodsDemand - sold;
        float imported = resourcesManager.importGoods(demand);
        
        return imported;
    }

    private void updateBudget()
    {
        budget += (income - spending);
        displayBudget();
    }

    private void calculateIncome(float produced, float sold, float exported, float imported)
    {
        float productionCost = produced * productionCostPerUnit;
        float salesRevenue = sold * salePricePerUnit;
        //float exportRevenue = exported * exportRevenuePerUnit;
        //float importCost = imported * productionCostPerUnit;

        float internalDeficit = calculateInternalDeficit(produced, sold);
        float tradeDeficit = calculateExportAndImportDeficit(exported, imported);

        //income = tradeDeficit + (salesRevenue - productionCost);
        income = tradeDeficit + internalDeficit;
    }

    private float calculateInternalDeficit(float produced, float sold)
    {
        float productionCost = produced * productionCostPerUnit;
        float salesRevenue = sold * salePricePerUnit;

        float deficit = salesRevenue - productionCost;

        return deficit;
    }

    private float calculateExportAndImportDeficit(float export, float import)
    {   
        float exportRevenue = export * exportRevenuePerUnit;
        float importCost = import * importCostPerUnit;
        
        float tradeDeficit = exportRevenue - importCost;
        
        return tradeDeficit;
    }


    private void displayBudget()
    {
        uiController.displayBudget(budget);
    }

    private void triggerExport()
    {
        triggeredExport = true;
    }

    private void deactiveExport()
    {
        triggeredExport = false;
    }
}
