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

    public bool isExportTriggered = false;

    public float produceGoods()
    {
        if (isOverProduction())
        {
            return 0;
        }

        currentStorage += productionRatePerTimeUnit;
        
        if (isStorageFull())
        {
            surplus += currentStorage - totalStorageCapacity;
            currentStorage = totalStorageCapacity;
        }

        return productionRatePerTimeUnit;
    }

    public float sellGoods()
    {
        // When a new city starts
        if (currentStorage <= 0 && totalStorageCapacity <= 0)
        {
            return 0;
        }
        
        currentStorage -= salesRatePerTimeUnit;
        
        return salesRatePerTimeUnit;
    }

    public void debugTradeDeficit()
    {
        Debug.Log($"E: {exportRate} : I: {importDemand}");
    }
    private bool isStorageFull()
    {     
        if (currentStorage >= totalStorageCapacity)
        {
            return true;
        }
        return false;
    }

    // Import logics
    // === Implement similar logic from export
    public float importGoods()
    {
        importDemand = salesRatePerTimeUnit - productionRatePerTimeUnit;
        currentStorage += importDemand;
        return importDemand;
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
    public void triggerExport()
    {
        isExportTriggered = true;
    }
    public float exportSurplus()
    {
        float exportedGoods;
        if (surplus <= exportRate) // when excess goods is smaller than export speed
        {
            exportedGoods = surplus;
            surplus = 0;

        }

        else
        {
            exportedGoods = exportRate;
            surplus -= exportedGoods;
        }

        return exportedGoods;
    }

    public bool isExportThreshold()
    {
        float excessRatio = surplus / totalStorageCapacity;
        if (excessRatio >=  exportThreshold && totalStorageCapacity > 0)
        {
            return true;
        }
        return false;
    }

    private bool isOverProduction()
    {
        return surplus >= totalStorageCapacity * productionThreshold;
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
