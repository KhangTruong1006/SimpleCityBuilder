using UnityEngine;

public class WaterAndPowerService : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    public StatsPanelController statsPanelController;
    public PanelController panelController;

    public PopulationManager populationManager;

    [Header("Power")]
    public float powerSupplyCapacity;
    public float powerCurrentUsage;

    [Header("Water")]
    public float waterSupplyCapacity;
    public float waterCurrentUsage;

    [Header("Sewage")]
    public float sewageProcessingCapacity; // Sewage capacity depends on water treatment to process
    public float sewageCurrentUsage;

    private bool isPowerShortage = false;
    private bool isWaterShortage = false;
    private bool isSewageShortage = false;


    public void runSimulationTick()
    {
        // When starts a new city
        if (populationManager.getCurrentPopulation() < 1)
        {
            return;
        }

        checkShortages();
        displayShortages();

        statsPanelController.displayServiceUsageStats(waterCurrentUsage, sewageCurrentUsage, powerCurrentUsage);
        statsPanelController.displayServiceGeneratingStats(waterSupplyCapacity, sewageProcessingCapacity, powerSupplyCapacity);
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

    public bool haveShortages()
    {
        return isPowerShortage || isWaterShortage || isSewageShortage;
    }

    private void displayShortages()
    {
        displayShortageType(isPowerShortage, panelController.warningPowerPanel);
        displayShortageType(isWaterShortage, panelController.warningWaterPanel);
        displayShortageType(isSewageShortage, panelController.warningSewagePanel);
    }

    // Display shortage message in the console
    private void displayShortageType(bool shortage, GameObject panel)
    {
        if (shortage && !panel.activeSelf)
        {
            panelController.enablePanel(panel);
            return;
        }
        if (!shortage && panel.activeSelf) 
        {
            panelController.disablePanel(panel); 
        }            
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

    public float calulateElectricityConsumptionRate()
    {
        if (powerSupplyCapacity <= 0)
        {
            return 0f;
        }
        return powerCurrentUsage / powerSupplyCapacity;
    }

    public float calulateSewageProcessingRate()
    {
        if (sewageProcessingCapacity <= 0)
        {
            return 0f;
        }
        return sewageCurrentUsage / sewageProcessingCapacity;
    }
}
