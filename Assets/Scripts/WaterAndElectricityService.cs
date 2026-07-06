using UnityEngine;

public class WaterAndElectricityService : MonoBehaviour
{
    public float waterSupplyCapacity;
    public float sewageCapacity; // Sewage capacity depends on water treatment to process
    public float electricitySupplyCapacity;
    
    public float electricityCurrentConsumption;
    public float waterCurrentConsumption;
    public float sewageCurrentAmount;


    public void updateConsumptions(float electric = 0, float water = 0)
    {
        electricityCurrentConsumption += electric;
        waterCurrentConsumption += water;
        sewageCapacity = waterCurrentConsumption;
    }

    public void updateWaterSupply(float newSupply)
    {
        waterSupplyCapacity += newSupply;
    }

    public void updateElectricitySupply(float newSupply)
    {
        electricitySupplyCapacity += newSupply;
    }

    public float calulateWaterSupplyRate()
    {
        if (waterSupplyCapacity <= 0)
        {
            return 0f; 
        }
        return waterCurrentConsumption / waterSupplyCapacity;
    }
}
