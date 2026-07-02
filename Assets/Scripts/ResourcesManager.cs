using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    public PopulationManager populationManager;

    // Unit: Tons
    public float totalStorageCapacity;
    public float currentStorage;

    public float productionRatePerTimeUnit;

    public float goodsDemandPerCapita = 0.05f;
    public float dynamicDemand;

    [Header("Export and Import")]
    public float surplus;
    public float exportRate = 50f; // Unit: Tons per time unit
    public float importDemand = 0;

    [Header("Thresholds")]
    public float productionThreshold;
    public float exportThreshold;

    private void Start()
    {
        productionThreshold = settings.threshold.productionThreshold;
        exportThreshold = settings.threshold.exportThreshold;
    }

    public float produceGoods()
    {
        if (isOverProduction())
        {
            return 0;
        }
        float availableStorage = calculateAvailableStorage();
        float production = Mathf.Min(productionRatePerTimeUnit, availableStorage);


        currentStorage += production;

        // Surplus
        surplus += (productionRatePerTimeUnit - production);
        return productionRatePerTimeUnit;
    }

    public float calculateCurrentDemand()
    {
        dynamicDemand = populationManager.population * goodsDemandPerCapita;
        return dynamicDemand;
    }

    public float sellGoods(float demand)
    {
        // When a new city starts
        if (currentStorage <= 0)
        {
            // If fail to deliver goods
            if(populationManager!= null)
            {
                populationManager.updateGoodsSatisfaction(0f);
            }
            return 0;
        }
        
        
        float sold = Mathf.Min(demand, currentStorage);
        currentStorage -= sold;

        //Recalculate satisfaction score
        if (populationManager != null && demand > 0)
        {
            float satifaction = sold / demand;
            populationManager.updateGoodsSatisfaction(satifaction);
        }

        // In case of population = 0
        else if (demand == 0) {
            populationManager.updateGoodsSatisfaction(0f);
        }

            return sold;
    }


    // Import logics
    // === Implement similar logic from export
    public float importGoods(float demand)
    {
        float availableStorage = calculateAvailableStorage();
        float imported = Mathf.Min(demand, availableStorage); // Import Goods based on available storage

        currentStorage += imported;
        importDemand = imported; // For display in inspector
        
        return imported;
    }

    //Export logics
    public float exportSurplus()
    {
        // exporting rate < surplus -> export = rate
        // exporting rate > surplus -> empty surplus
        float exported = Mathf.Min(surplus, exportRate);
        surplus -= exported;

        return exported;
    }

    private float calculateAvailableStorage()
    {
        return totalStorageCapacity - currentStorage;
    }



    // Bool (checking) functions
    public bool isExportThreshold()
    {
        if (totalStorageCapacity <= 0)
        {
            return false;
        }

        if (surplus / totalStorageCapacity >= exportThreshold)
        {
            return true;
        }

        return false;
    }

    private bool isOverProduction()
    {
        return surplus >= totalStorageCapacity * productionThreshold;
    }
    public bool isSoldUnderDemand(float sold)
    {
        return sold < dynamicDemand;
    }

    public bool isSurplusAvaialbe()
    {
        return surplus > 0;
    }

    // Update general data
    public void updateTotalStorageCapacity(float change)
    {
        totalStorageCapacity += change;
    }

    public void updateProductionRatePerTimeUnit(float change)
    {
        productionRatePerTimeUnit += change;
    }
}
