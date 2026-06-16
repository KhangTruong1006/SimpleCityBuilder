using UnityEngine;

public class GoodsManager : MonoBehaviour
{
    // Unit: Tons
    public float totalStorageCapacity;
    public float currentStorage;

    public float productionRatePerTimeUnit;
    public float salesRatePerTimeUnit;

    public float excessGoods;
    public float exportRate = 1f;
    public float importRate;

    public float exportThreshold = 0.5f;
    public float importThreshold = 0.5f;


    public void handleLogistics()
    {
        produceGoods();
        sellGoods();
    }

    private void produceGoods()
    {
        currentStorage += productionRatePerTimeUnit;
        
        if (isStorageFull())
        {
            excessGoods += currentStorage - totalStorageCapacity;
            currentStorage = totalStorageCapacity;
            if (excessGoods > exportThreshold * totalStorageCapacity)
            {
                exportExcessGoods();
            } 
        }
    }

    private void sellGoods()
    {
        if(currentStorage >= salesRatePerTimeUnit)
        {
            currentStorage -= salesRatePerTimeUnit;
            return;
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



    public float exportExcessGoods()
    {
        if (excessGoods <= 0)
        {
            return 0f; // No excess goods to export
        }
        excessGoods -= exportRate;
        
        return exportRate;
    }

    public void debugGoods()
    {
        Debug.Log($"Current Storage: {currentStorage} / {totalStorageCapacity}");
    }

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
