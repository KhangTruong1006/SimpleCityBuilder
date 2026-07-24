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

    private float exported;
    private float imported;
    private float produced;
    private float sold;

    private float servicesMaintenanceSpending;
    

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
        calculateIncome();
        calculateExpenses();
        updateBudget(); 
    }

    public void substractConstructionCost(float cost)
    {
        budget -= cost;
        displayBudget();
    }

    //!!! CHECK AND FIX LOGIC FOR CALCULATING EXPENSES AND INCOME
    // === Logistics Handling ===`
    private void handleLogistics()
    {
        if (!populationManager.haveWorkers()) {
            return;
        }

        float currentDemand = resourcesManager.calculateCurrentDemand();

        produced = resourcesManager.produceGoods(populationManager.getEmploymentRate());
        sold = resourcesManager.sellGoods(currentDemand);

        //resourcesManager.importDemand = 0f;

        exported = handleExport();
        imported = handleImport(sold, currentDemand);

        //calculateExpenses(produced, imported);
        //calculateIncome(sold,exported);
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

    public void addMaintenanceSpending(float cost)
    {
        servicesMaintenanceSpending += cost;
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
    private void calculateIncome()
    {
        float salesRevenue = sold * salePricePerUnit;
        float exportRevenue = exported * exportRevenuePerUnit;

        income = (salesRevenue + exportRevenue) * tax;
    }

    private void calculateExpenses()
    {
        float productionCost = produced * productionCostPerUnit;
        float importCost = imported * importCostPerUnit;

        expenses = productionCost + importCost;
        //expenses = productionCost + importCost + servicesMaintenanceSpending;
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

    public void updateTax(float newTax) {
        tax = newTax;
    }
}
