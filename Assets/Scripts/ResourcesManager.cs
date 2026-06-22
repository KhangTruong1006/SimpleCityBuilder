using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    // Unit: Tons
    public float totalStorageCapacity;
    public float currentStorage;

    public float productionRatePerTimeUnit;
    public float salesRatePerTimeUnit; // Will be replace with Demand when integrate with population

    [Header("Export and Import")]
    public float surplus;
    public float exportRate = 50f; // Unit: Tons per time unit
    public float importDemand = 0;

    [Header("Thresholds")]
    public float productionThreshold = 0.5f;
    public float exportThreshold = 0.4f;


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

    public float sellGoods()
    {
        // When a new city starts
        if (currentStorage <= 0)
        {
            return 0;
        }
        
        float sold = Mathf.Min(salesRatePerTimeUnit, currentStorage);
        currentStorage -= sold;
        
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



    // Bool (checiking) functions
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
        return sold < salesRatePerTimeUnit; //salesRatePerTimeUnit will be replace by demand
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

    public void updateSalesRatePerTimeUnit(float change)
    {
        salesRatePerTimeUnit += change;
    }
}
