using UnityEngine;

public class WaterAndPowerService : MonoBehaviour
{
    [Header("Power")]
    public float powerSupplyCapacity;
    public float powerCurrentUsage;

    [Header("Water")]
    public float waterSupplyCapacity;
    public float waterCurrentUsage;

    [Header("Sewage")]
    public float sewageProcessingCapacity; // Sewage capacity depends on water treatment to process
    public float sewageCurrentUsage;

    
    public void runSimulationTick()
    {
        float electricityConsumptionRate = calulateElectricityConsumptionRate();
        float waterConsumptionRate = calulateWaterConsumptionRate();
        float sewageProcessingeRate = calulateSewageProcessingRate();


    }

    // ===== Update methods =====
    public void updateConsumptions(float power = 0, float water = 0)
    {
        powerCurrentUsage += power;
        waterCurrentUsage += water;
        sewageCurrentUsage = waterCurrentUsage;
    }

    public void updateWaterSupplyCapacity(float newSupply)
    {
        waterSupplyCapacity += newSupply;
    }

    public void updateElectricitySupplyCapacity(float newSupply)
    {
        powerSupplyCapacity += newSupply;
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
        if (powerSupplyCapacity <= 0)
        {
            return 0f; 
        }
        return powerCurrentUsage / powerSupplyCapacity;
    }

    public float calulateSewageProcessingRate() {
        if (sewageProcessingCapacity <= 0)
        {
            return 0f; 
        }
        return sewageCurrentUsage / sewageProcessingCapacity;
    }
}
