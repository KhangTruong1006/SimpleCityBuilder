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
    public float importRate = 10f;

    [Header("Thresholds")]
    public float productionThreshold = 0.5f;
    public float exportThreshold = 0.4f;
    public float importThreshold = 0.5f;


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
        
        currentStorage -= salesRatePerTimeUnit;
        if (currentStorage <= 0)
        {
            currentStorage = 0;
        }
    }

    private bool isStorageFull()
    {     
        if (currentStorage >= totalStorageCapacity)
        {
            return true;
        }
        return false;
    }

    // ADD FUNCTIONS FOR IMPORTING

    public float exportExcessGoods()
    {
        if (!isExportThreshold())
        {
            return 0;
        }

        excessGoods -= exportRate;
        return exportRate;
    }

    private bool isExportThreshold()
    {
        return excessGoods >= totalStorageCapacity * exportThreshold;
    }

    private bool isOverProduction()
    {
        return excessGoods >= totalStorageCapacity * productionThreshold;
    }

    // Ignore for now

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
