using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameSettings settings;

    public DemandController demandController;
    public DemographicsManager demographicsManager;
    public UIController uiController;


    [Header("Current Statistics")]
    public int population;
    public int populationCapacity;
    public int jobCapacity;
    public int employedPopulation;
    public int employablePopulation;

    private int commercialEmployedPopulation;
    private int industrialEmployedPopulation;

    [Range(0.0f, 1.0f)]
    public float goodsSatisfaction = 1.0f;
    [Range(0.0f, 1.0f)]
    public float workersThreshold;

    private float growthRate;
    public float precisePopulation = 0f;
    public float globalFactor;

    private void Awake()
    {
        //demandController = GetComponent<DemandController>();
        demographicsManager = GetComponent<DemographicsManager>();
    }

    private void Start()
    {
        workersThreshold = settings.threshold.workersThreshold;

        precisePopulation = population;
        initializeDemographicDistribution(population); // If the city starts with a population, initialize the demographics distribution
    }

    public void runSimulationTick()
    {
        calculateEmployment();
        calculateGlobalFactor();
        calculatePopulationChange();

        uiController.displayPopulation(population);
    }

    private void calculateEmployment()
    {
        employablePopulation = demographicsManager.getEmployablePopulation();

        if (employablePopulation < 1)
        {
            employedPopulation = 0;
            return;
        }
        employedPopulation = Math.Min(employablePopulation, jobCapacity);
    }

    private void calculatePopulationChange()
    {
        // To handle calculation when population is zero (the start of the game) / Seeding
        if (precisePopulation <= 0 && populationCapacity >= 0)
        {
            precisePopulation = settings.population.seedingPop;
            initializeDemographicDistribution((int)precisePopulation);
        }

        // Prevent negative/over population 
        if (precisePopulation > populationCapacity)
        {
            return;
        }

        // This method uses Logistic Growth Model based on various factors (metrics)
        // rate = base * factors point * population * (1 - population / capacity)

        float basedGrowthRate = settings.population.basedGrowthRate;
        float growthRate = basedGrowthRate * globalFactor * precisePopulation * (1f - (precisePopulation / populationCapacity));
        //float growthRate = basedGrowthRate * precisePopulation * (1f - (precisePopulation / populationCapacity));

        int updatedPopulation = demographicsManager.updateDemographics(growthRate, populationCapacity);
        population = Mathf.Min(updatedPopulation, populationCapacity);
        precisePopulation = population;

        //Debug.Log($"Population : {population} Precise Pop: {precisePopulation} GF: {globalFactor} Jobs: {jobCapacity} Workers: {employedPopulation}");
    }

    private void calculateGlobalFactor()
    {
        // This method calculates the global factor based on various city metrics
        // Prevent zero division
        if (population <= 0)
        {
            globalFactor = 1.0f;
            return;
        }
        float employmentRate = (employablePopulation > 0) ? (float)employedPopulation / (float)employablePopulation : 0f;
        float housingRate = (float)population / (float)populationCapacity;

        globalFactor = 0.4f * housingRate + 0.3f * employmentRate + 0.3f * (float)goodsSatisfaction;
    }



    public bool haveWorkers()
    {
        return employedPopulation > 0;
    }


    // Get Functions
    public float getCurrentPopulationRate()
    {
        return population / populationCapacity;
    }

    public int getCurrentPopulation()
    {
        return population;
    }

    public int getPopulationCapacity()
    {
        return populationCapacity;
    }

    public int getCurrentEmployedPopulation()
    {
        return employedPopulation;
    }

    public int getJobCapacity()
    {
        return jobCapacity;
    }

    public float getEmploymentRate()
    {
        return employedPopulation / jobCapacity;
    }

    public int getEmployablePopulation()
    {
        return employablePopulation;
    }


    // Update Functions

    public void updatePopulationCapacity(int capacity)
    {
        populationCapacity += capacity;
        //Debug.Log($"Population capacity updated: {populationCapacity}");
    }

    public void updateJobCapacity(int newJobs)
    {
        jobCapacity += newJobs;
        //Debug.Log($"Jobs updated: {jobCapacity}");
    }

    public void updateGoodsSatisfaction(float change)
    {
        goodsSatisfaction = change;
    }

    private void initializeDemographicDistribution(int pop)
    {
        if(population > 0)
        {
            demographicsManager.initializeDemographics(pop);
        }
    }


}
