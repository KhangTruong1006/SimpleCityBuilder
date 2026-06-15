using UnityEngine;

public class GoodsManager : MonoBehaviour
{
    // Unit: Tons
    public float totalStorageCapacity;
    public float currentStorage;

    public float productionRatePerTimeUnit;
    public float salesRatePerTimeUnit;

    public float excessGoods;


    public void produceGoods()
    {
        currentStorage += productionRatePerTimeUnit;
        
        if (isStorageFull())
        {
            excessGoods = currentStorage - totalStorageCapacity;
            currentStorage = totalStorageCapacity;
            return;
        }
    }

    public void sellGoods()
    {
        currentStorage-= salesRatePerTimeUnit;
        if(currentStorage < 0)
        {
            currentStorage = 0;
        }
    }

    public bool isStorageFull()
    {
        float spaceLeft = totalStorageCapacity - currentStorage;
        
        if (spaceLeft <= 0)
        {
            return true;
        }
        return false;
    }

    // To check storage modify later
    public void checkStorage()
    {
        if (isStorageFull())
        {
            Debug.Log("Storage is full! Consider building more storage or reducing production.");
        }
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
