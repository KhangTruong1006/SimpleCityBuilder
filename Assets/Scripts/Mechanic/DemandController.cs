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

    public bool isInitialSeeding = true;

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
            return;
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
        if (PopulationManager.getCurrentPopulation() > 1)
        {
            isInitialSeeding = false;
            return;
        }

        
        residentialDemand = 1.0f;
        commercialDemand = 0.0f;
        industrialDemand = 0.0f;
        
    }

  
    // Demands Methods
    private void updateResidentialDemand(int population, int capcity, int employed, int jobCapacity)
    {
        if(jobCapacity <= 0)
        {
            residentialDemand = 0.5f;
            return;
        }

        int openJobs = Mathf.Max(0, jobCapacity - employed);
        float jobRatio = (float)openJobs / jobCapacity;
        float housingFactor = calculateHousingFactor(population,capcity);  

        float rawDemand = (0.5f * housingFactor) + (0.5f * jobRatio);

        residentialDemand = clamp01Input(rawDemand);
    }
    private void updateCommercialDemand(int population, int employed)
    {
        if(population <= 0)
        {
            commercialDemand = 0f;
            return;
        }

        float goodsDeficit = 1f - PopulationManager.getGoodsSatisfaction();
        float employmentRate = (float)employed / population;
        float basePopulationDemand = clamp01Input((float) population / 50f); 

        float rawDemand = (0.3f * goodsDeficit) + (0.4f * employmentRate) + (0.3f * basePopulationDemand);

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
        float currentDemand = ResourcesManager.getDynamicDemand();
        float importReliance = calculateImportReliance(currentDemand);

        float rawDemand = 0.5f * unemploymentRate + 0.5f * importReliance;

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

        float importReliance = ResourcesManager.getImportDemand() / currentDemand;
        return clamp01Input(importReliance);
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

    public float getResidentialDemand()
    {
        return residentialDemand;
    }
}
