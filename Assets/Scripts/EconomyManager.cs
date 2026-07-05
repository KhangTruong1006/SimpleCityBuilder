using System;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public GameSettings settings;

    public ResourcesManager resourcesManager;
    public PopulationManager populationManager;
    public UIController uiController;

    private float budget;
    public float expenses;
    public float income;
    [Range(0f, 1f)]
    public float tax;

    public float exportRevenuePerUnit;
    public float importCostPerUnit;

    public float productionCostPerUnit;
    public float salePricePerUnit;

    private bool triggeredExport = false;

    private void Awake()
    {
        resourcesManager = GetComponent<ResourcesManager>();
    }
    private void Start()
    {
        exportRevenuePerUnit = settings.economy.exportRevenuePerUnit;
        importCostPerUnit = settings.economy.importCostPerUnit;
        productionCostPerUnit = settings.economy.productionCostPerUnit;
        salePricePerUnit = settings.economy.salePricePerUnit;

        tax = settings.economy.taxRate;
        budget = settings.economy.initialBudget;
        displayBudget();
    }

    public void runSimulationTick()
    {
        handleLogistics();
        
        updateBudget(); 
    }

    public void substractConstructionCost(float cost)
    {
        budget -= cost;
        displayBudget();
    }

    //!!! CHECK AND FIX LOGIC FOR CALCULATING EXPENSES AND INCOME
    public void addExpenseBudge(float newExpense)
    {
        expenses += newExpense;
    }

    // === Logistics Handling ===`
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

    // === Budget Calculation ===
    private void calculateIncome(float sold, float exported)
    {
        float salesRevenue = sold * salePricePerUnit;
        float exportRevenue = exported * exportRevenuePerUnit;

        income = (salesRevenue + exportRevenue) * tax;
    }

    private void calculateExpenses(float produced, float imported)
    {
        float productionCost = produced * productionCostPerUnit;
        float importCost = imported * productionCostPerUnit;

        expenses = productionCost + importCost;
    }


    // Helpers
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
