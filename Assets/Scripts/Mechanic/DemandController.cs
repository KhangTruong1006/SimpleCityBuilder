using UnityEngine;
using static GameSettings;
using static UnityEditor.Experimental.GraphView.Port;

public class DemandController : MonoBehaviour
{
    public PopulationManager PopulationManager;
    public EconomyManager EconomyManager;
    public ResourcesManager ResourcesManager;

    public SliderController SliderController;

    public float residentialDemand = 1.0f;
    public float commercialDemand = 0.0f;
    public float industrialDemand = 0.0f;

    private bool isInitialSeeding = true;

    private float targetHousingVacancy = 0.05f;

    private void Start()
    {
        SliderController.updateZoneDemandBars(residentialDemand, commercialDemand, industrialDemand);
    }

    public void runSimulationTick()
    {
        updateDemand();
        SliderController.updateZoneDemandBars(residentialDemand, commercialDemand, industrialDemand);
    }

    public void updateDemand()
    {
        if (isInitialSeeding)
        {
            initialSeeding();
        }

        int population = PopulationManager.getCurrentPopulation();
        int populationCapacity = PopulationManager.getPopulationCapacity();
        int jobCapacity = PopulationManager.getJobCapacity();
        int employed = PopulationManager.getCurrentEmployedPopulation();
        int employable = PopulationManager.getEmployablePopulation();

        updateResidentialDemand(population, populationCapacity, employed, jobCapacity);
        updateCommercialDemand(population, employed);
        updateIndustrialDemand(employable,employed);       
    }


    private void initialSeeding()
    {
        if (PopulationManager.population > 5)
        {
            isInitialSeeding = false;
            return;
        }

        else
        {
            residentialDemand = 1.0f;
            commercialDemand = 0.0f;
            industrialDemand = 0.0f;
        }
    }

  
    // Demands Methods
    private void updateResidentialDemand(int population, int capcity, int employed, int jobCapacity)
    {

        float housingFactor = calculateHousingFactor(population, capcity);
        float jobRatio = calculateAvailableJobRatio(employed, jobCapacity);

        float rawDemand =  0.7f * jobRatio + 0.3f * housingFactor;
        
        residentialDemand = clamp01Input(rawDemand);
    }
    private void updateCommercialDemand(int population, int employed)
    {
        if(population <= 0)
        {
            commercialDemand = 0f;
            return;
        }

        float goodsDeficit = 1f - PopulationManager.goodsSatisfaction;
        float employmentRatio = calculateEmploymentRatio(employed, population);
        float rawDemand = (0.6f * goodsDeficit) + (0.4f * employmentRatio);

        commercialDemand = clamp01Input(rawDemand);
    }
    
    private void updateIndustrialDemand(int employablePopulation, int employed)
    {
        if (employablePopulation <= 0)
        {
            industrialDemand = 0f;
            return;
        }
        float unemploymentRate = calculateUnemploymentRate(employablePopulation, employed);
        float currentDemand = ResourcesManager.dynamicDemand;
        float importReliance = calculateImportReliance(currentDemand);

        float rawDemand = 0.2f * unemploymentRate + 0.8f * importReliance;

        industrialDemand = clamp01Input(rawDemand);
    }

    private float calculateHousingFactor(int population, int capcity)
    {
        if (capcity <= 0)
        {
            return 0f;
        }

        // Normalize against target vacancy (e.g., demand drops if vacancy exceeds target 5%)
        float vancancyRate = 1.0f - ((float)population / capcity);
        float housingFactor = 1.0f - (vancancyRate / (targetHousingVacancy * 2f));
        return clamp01Input(housingFactor);

    }


    private float calculateImportReliance(float currentDemand)
    {
        if (currentDemand <= 0)
        {
            return 0f;
        }

        float importReliance = ResourcesManager.importDemand / currentDemand;
        return clamp01Input(importReliance);
    }

    private float calculateAvailableJobRatio(int employed, int jobCapacity)
    {
        // New city - prevent 0 division
        if (jobCapacity <= 0)
        {
            return 0f;
        }

        int openJobs = Mathf.Max(0, jobCapacity - employed);
        float ratio = (float)openJobs / jobCapacity;
        return ratio;
    }

    private float calculateEmploymentRatio(int employed, int population)
    {
        if (population <= 0)
        {
            return 0f;
        }

        return employed/population;
    }

    private float calculateUnemploymentRate (int employable, int employed)
    {
        if (employable <= 0)
        {
            return 0f;
        }

        int unemployed = Mathf.Max(0, employable - employed);
        float unemployedRate = (float)unemployed / employable;

        return unemployedRate;
     
    }

    private float clamp01Input(float input)
    {
        return Mathf.Clamp01(input);
    }
}
