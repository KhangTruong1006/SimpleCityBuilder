using UnityEngine;

public class WaterAndElectricityService : MonoBehaviour
{
    [Header("Electricity")]
    public float electricitySupplyCapacity;
    public float electricityCurrentUsage;

    [Header("Water")]
    public float waterSupplyCapacity;
    public float waterCurrentUsage;

    [Header("Sewage")]
    public float sewageProcessingCapacity; // Sewage capacity depends on water treatment to process
    public float sewageCurrentUsage;


    public void runSimulationTick()
    {

    }

    // ===== Update methods =====
    public void updateConsumptions(float electric = 0, float water = 0)
    {
        electricityCurrentUsage += electric;
        waterCurrentUsage += water;
        sewageCurrentUsage = waterCurrentUsage;
    }

    public void updateWaterSupplyCapacity(float newSupply)
    {
        waterSupplyCapacity += newSupply;
    }

    public void updateElectricitySupplyCapacity(float newSupply)
    {
        electricitySupplyCapacity += newSupply;
    }

    public void updateSewageProcessingCapacity(float newCapacity)
    {
        sewageProcessingCapacity += newCapacity;
    }


    // ===== Calculate methods =====
    public float calulateWaterConsumptionRate()
    {
        if (waterSupplyCapacity <= 0)
        {
            return 0f; 
        }
        return waterCurrentUsage / waterSupplyCapacity;
    }

    public float calulateElectricityConsumptionRate() {
        if (electricitySupplyCapacity <= 0)
        {
            return 0f; 
        }
        return electricityCurrentUsage / electricitySupplyCapacity;
    }
}
