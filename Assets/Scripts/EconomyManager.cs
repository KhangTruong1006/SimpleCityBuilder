using System;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public ResourcesManager resourcesManager;
    public PopulationManager populationManager;
    public UIController uiController;

    private float budget;
    public float expenses;
    public float income;
    [Range(0f, 1f)]
    public float tax = 0.1f;

    public float exportRevenuePerUnit = 10f;
    public float importCostPerUnit = 25f;

    public float productionCostPerUnit = 2f;
    public float salePricePerUnit = 3f;

    private bool triggeredExport = false;


    private void Start()
    {
        budget = 10000f;
        displayBudget();
    }

    public void runSimulationTick()
    {
        handleLogistics();
        
        updateBudget(); 
    }


    private void handleLogistics()
    {
        if (!populationManager.haveWorkers()) {
            return;
        }

        float currentDemand = resourcesManager.calculateCurrentDemand();

        float produced = resourcesManager.produceGoods();
        float sold = resourcesManager.sellGoods(currentDemand);

        resourcesManager.importDemand = 0f;
        float exported = handleExport();
        float imported = handleImport(sold, currentDemand);

        calculateExpenses(produced, imported);
        calculateIncome(sold,exported);
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

    private void calculateIncome(float sold, float exported)
    {
        float salesRevenue = sold * salePricePerUnit;
        float exportRevenue = exported * exportRevenuePerUnit;

        income = salesRevenue + exportRevenue;
    }

    public void calculateExpenses(float produced, float imported)
    {
        float productionCost = produced * productionCostPerUnit;
        float importCost = imported * productionCostPerUnit;

        expenses = productionCost + importCost;
    }

    private void updateBudget()
    {
        budget += (income - expenses);
        displayBudget();
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
