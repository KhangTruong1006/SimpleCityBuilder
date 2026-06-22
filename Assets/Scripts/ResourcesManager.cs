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
        float actualProduction = Mathf.Min(productionRatePerTimeUnit, availableStorage);

        currentStorage += actualProduction;

        // Surplus
        surplus += (productionRatePerTimeUnit - actualProduction);
        return productionRatePerTimeUnit;
    }

    public float sellGoods()
    {
        // When a new city starts
        if (currentStorage <= 0)
        {
            return 0;
        }
        
        float actualSales = Mathf.Min(salesRatePerTimeUnit, currentStorage);
        currentStorage -= actualSales;
        
        return actualSales;
    }

    public void debugTradeDeficit()
    {
        Debug.Log($"E: {exportRate} : I: {importDemand}");
    }

    // Import logics
    // === Implement similar logic from export
    public float importGoods(float demand)
    {
        float availableStorage = calculateAvailableStorage();
        float imported = Mathf.Min(demand, availableStorage); // Import Goods based on available storage

        currentStorage += imported;
        importDemand = imported;
        
        return imported;
    }

    public bool isStockAndProductionUnderDemand()
    {
        // Not enough stock and underproduction
        if (isStockUnderDemand() && isUnderProduction())
        {
            return true;
        }
        return false;
    }

    private bool isStockUnderDemand()
    {
        return currentStorage < salesRatePerTimeUnit;
    }
    private bool isUnderProduction()
    {
        return productionRatePerTimeUnit < salesRatePerTimeUnit;
    }

    //Export logics

    // === FIX THIS TO EXPORT GOODS UNTIL THE ENTIRE EXCESS IS 0! -  #Not finished
    // === TEST THIS
    public float exportSurplus()
    {
        // exporting rate < surplus -> export = rate
        // exporting rate > surplus -> empty surplus
        float exported = Mathf.Min(surplus, exportRate);
        surplus -= exported;

        return exported;
    }

    public bool isExportThreshold()
    {
        if (totalStorageCapacity <= 0)
        {
            return false;
        }

        if (surplus/totalStorageCapacity >= exportThreshold)
        {
            return true;
        }

        return false;
    }

    private bool isOverProduction()
    {
        return surplus >= totalStorageCapacity * productionThreshold;
    }

    private float calculateAvailableStorage()
    {
        return totalStorageCapacity - currentStorage;
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
