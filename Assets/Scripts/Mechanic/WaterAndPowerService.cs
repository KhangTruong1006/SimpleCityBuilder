using UnityEngine;

public class WaterAndPowerService : MonoBehaviour
{
    [SerializeField] private GameSettings settings;

    [Header("Power")]
    public float powerSupplyCapacity;
    public float powerCurrentUsage;

    [Header("Water")]
    public float waterSupplyCapacity;
    public float waterCurrentUsage;

    [Header("Sewage")]
    public float sewageProcessingCapacity; // Sewage capacity depends on water treatment to process
    public float sewageCurrentUsage;

    private float powerActualCurrentUsage;
    private float sewageActualCurrentUsage;
    private float waterActualCurrentUsage;

    private bool isPowerShortage = false;
    private bool isWaterShortage = false;
    private bool isSewageShortage = false;


    public void runSimulationTick()
    {
        checkShortages();

        if (isShortage())
        {
            applyPenalties();
        }
    }


    private void checkShortages()
    {
        float powerConsumptionRate = calulateElectricityConsumptionRate();
        float waterConsumptionRate = calulateWaterConsumptionRate();
        float sewageProcessingeRate = calulateSewageProcessingRate();

        isPowerShortage = checkSupplyThreshold(powerConsumptionRate, powerSupplyCapacity);
        isWaterShortage = checkSupplyThreshold(waterConsumptionRate, waterSupplyCapacity);
        isSewageShortage = checkSupplyThreshold(sewageProcessingeRate, sewageProcessingCapacity); 
    }

    
    private bool checkSupplyThreshold(float consumptionRate, float supply)
    {
        // Return true if the consumption rate exceeds the supply threshold or if the supply is zero or negative
        return consumptionRate > settings.service.supplyThreshold || supply <= 0;
    }

    private bool isShortage()
    {
        return isPowerShortage || isWaterShortage || isSewageShortage;
    }


    private void applyPenalties()
    {
        Debug.Log("Applying penalties due to shortages.");
        // REMEMBER: Implement the logic to apply penalties to the city based on shortages. This could involve reducing population growth, decreasing satisfaction, or other game mechanics.
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
