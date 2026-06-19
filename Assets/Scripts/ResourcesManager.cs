using UnityEngine;

public class GoodsManager : MonoBehaviour
{
    // Unit: Tons
    public float totalStorageCapacity;
    public float currentStorage;

    public float productionRatePerTimeUnit;
    public float salesRatePerTimeUnit; // Will be replace with Demand when integrate with population

    [Header("Export and Import")]
    public float excessGoods;
    public float exportRate = 50f; // Unit: Tons per time unit
    public float importDemand = 0;

    [Header("Thresholds")]
    public float productionThreshold = 0.5f;
    public float exportThreshold = 0.4f;

    private bool isExportTriggered = false;

    public void produceGoods()
    {
        if (isOverProduction())
        {
            return;
        }

        currentStorage += productionRatePerTimeUnit;
        
        if (isStorageFull())
        {
            excessGoods += currentStorage - totalStorageCapacity;
            currentStorage = totalStorageCapacity;
        }
    }

    public void sellGoods()
    {
        if (currentStorage <= 0)
        {
            currentStorage = 0;
            return;
        }
        currentStorage -= salesRatePerTimeUnit;
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
    public float exportExcessGoods()
    {
        float exportedGoods;
        if (excessGoods <= exportRate) // when excess goods is smaller than export speed
        {
            exportedGoods = excessGoods;
            excessGoods = 0;
            isExportTriggered = false;
        }

        else
        {
            exportedGoods = exportRate;
            excessGoods -= exportedGoods;
        }

        return exportedGoods;
    }

    public bool isExportThreshold()
    {
        float excessRatio = excessGoods / totalStorageCapacity;
        if (excessRatio >=  exportThreshold && totalStorageCapacity > 0)
        {
            return true;
        }
        return false;
    }

    private bool isOverProduction()
    {
        return excessGoods >= totalStorageCapacity * productionThreshold;
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
