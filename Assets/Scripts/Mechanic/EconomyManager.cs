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
    public SliderController sliderController;
    public StatsPanelController statsPanelController;

    private float budget;
    public float expenses;
    public float income;
    [Range(0f, 0.3f)]
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

    public float powerMaintenanceSpending;
    public float waterMaintenanceSpending;
    public float sewageMaintenanceSpending;

    private float incomePerCapita;

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
        incomePerCapita = settings.economy.incomePerCapita;

        tax = settings.economy.taxRate;
        budget = settings.economy.initialBudget;

        sliderController.initialTaxSlider(tax);
        displayBudget();
    }

    public void runSimulationTick(int counter)
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

    // === Logistics Handling ===`
    private void handleLogistics()
    {
        if (!populationManager.haveWorkers()) {
            return;
        }

        float currentDemand = resourcesManager.calculateCurrentDemand();

        produced = resourcesManager.produceGoods(populationManager.getEmploymentAndJobRatio());
        sold = resourcesManager.sellGoods(currentDemand);

        exported = handleExport();
        imported = handleImport(sold, currentDemand);
    }

    private float handleExport()
    {
        if (resourcesManager.isExportThreshold() && !triggeredExport)
        {
            toggleExport();
        }


        if (triggeredExport)
        {
            if (resourcesManager.isSurplusAvaialbe())
            {
                return resourcesManager.exportSurplus();
            }
            
            toggleExport();
        }
        
        return 0f;
    }

    public float calculateMaintenanceSpending()
    {
        return waterMaintenanceSpending + powerMaintenanceSpending + sewageMaintenanceSpending;
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
        float incomeCommerical = sold * salePricePerUnit; // Sales
        float incomeIndustrial = exported * exportRevenuePerUnit; // Exports
        float incomeResidential = 0;

        if(populationManager.getEmploymentRate() > 0f)
        {
            incomeResidential = populationManager.getCurrentPopulation() * incomePerCapita;
        }

        income = (incomeCommerical + incomeIndustrial + incomeResidential) * tax;
        
        statsPanelController.displayIncomeStats(incomeResidential * tax, incomeCommerical * tax, incomeIndustrial * tax);
    }

    private void calculateExpenses()
    {
        float productionCost = produced * productionCostPerUnit; // Industrial
        float importCost = imported * importCostPerUnit; // Commercial
        float maintenanceCost = calculateMaintenanceSpending(); // Services

        expenses = productionCost + importCost + maintenanceCost;
        statsPanelController.displayExpenseStats(importCost, productionCost, powerMaintenanceSpending, waterMaintenanceSpending, sewageMaintenanceSpending);
    }

    private void subtractMaintenanceCost()
    {
        budget -= calculateMaintenanceSpending();
    }

    // Helpers
    private void updateBudget()
    {
        float netIncome = income - expenses;
        budget += netIncome;
        statsPanelController.displaySummaryStats(income, expenses, netIncome);
        displayBudget();
    }

    private void displayBudget()
    {
        uiController.displayBudget(budget);
    }

    private void toggleExport()
    {
        triggeredExport = !triggeredExport;
    }

    public void updateTax(float newTax) {
        tax = newTax;
    }

    public void updateWaterSpending(float cost) {
        waterMaintenanceSpending += cost;
    }

    public void updatePowerSpending(float cost) {
        powerMaintenanceSpending += cost;
    }

    public void updateSewageSpending(float cost) {
        sewageMaintenanceSpending += cost;
    }
}
