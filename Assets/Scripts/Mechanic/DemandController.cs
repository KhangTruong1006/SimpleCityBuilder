using UnityEngine;
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

    private void Start()
    {
        SliderController.updateZoneDemandBars(residentialDemand, commercialDemand, industrialDemand);
    }

    public void updateDemand()
    {
        if (isInitialSeeding)
        {
            initialSeeding();
        }

        int population = PopulationManager.population;
        int populationCapacity = PopulationManager.populationCapacity;
        int jobCapacity = PopulationManager.jobCapacity;
        int employedPopulation = PopulationManager.employedPopulation;

        updateResidentialDemand(population, populationCapacity, employedPopulation, jobCapacity);
        updateCommercialDemand(population);
        updateIndustrialDemand(population, employedPopulation, jobCapacity);

        SliderController.updateZoneDemandBars(residentialDemand, commercialDemand, industrialDemand);
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


    
    // Residential Demand
    private void updateResidentialDemand(int population, int capcity, int employedPopulation, int jobCapacity)
    {
        float housingVacancy = calculateHousingVacancy(population, capcity);
        float jobVacancy = calculateJobVacancy(employedPopulation, jobCapacity);

        // High when housing vacancy is low and available jobs are high
        // Low when housing vacancy is high
        float rawDemand = (1.0f - housingVacancy) + jobVacancy;
        
        residentialDemand = Mathf.Clamp01(rawDemand);
    }

    

    private float calculateHousingVacancy(int population, int capcity)
    {
        // New city - prevent 0 division
        if(capcity <= 0)
        {
            return 0f;
        }

        float vancancyRate = 1.0f - ((float)population / capcity);
        return vancancyRate;
    }

    // Commercial Demand
    private void updateCommercialDemand(int population)
    {
        if(population <= 0)
        {
            commercialDemand = 0f;
            return;
        }

        float goodsDeficit = 1f - PopulationManager.goodsSatisfaction;
        float rawDemand = (population * 0.05f) + goodsDeficit;

        commercialDemand = Mathf.Clamp01(rawDemand);

    }

    private float calculateJobVacancy(int employedPopulation, int jobCapacity)
    {
        // New city - prevent 0 division
        if (jobCapacity <= 0)
        {
            return 0f;
        }
        float vancancyRate = 1.0f - ((float)employedPopulation / jobCapacity);
        return vancancyRate;
    }

    // Industrial Demand
    private void updateIndustrialDemand(int population, int employedPopulation, int jobCapacity)
    {
        float unemploymentRate = calculateUnemploymentRate(population, employedPopulation);
        float currentDemand = ResourcesManager.dynamicDemand;
        float importReliance = calculateImportReliance(currentDemand);

        float rawDemand = unemploymentRate + importReliance;

        industrialDemand = Mathf.Clamp01(rawDemand);
    }

    private float calculateImportReliance(float currentDemand)
    {
        if (currentDemand <= 0)
        {
            return 0f;
        }

        float importReliance = ResourcesManager.importDemand / currentDemand;
        return importReliance;
    }


    private float calculateUnemploymentRate (int population, int employedPopulation)
    {
        if (population <= 0)
        {
            return 0f;
        }

        float unemployed = (float) (population - employedPopulation) / population;
        return unemployed;
    }
}
