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
        // Change Employment to Demand
        calculateEmployment();

        if (demandController != null) {
            demandController.updateDemand();
        }

        calculateGlobalFactor();
        calculatePopulationChange();

        uiController.displayPopulation(population);
        uiController.displayJobs(jobCapacity);
    }

    private void calculateEmployment()
    {
        int availableWorkers = demographicsManager.getWorkForce();
        
        if (availableWorkers < 1)
        {
            employedPopulation = 0;
            return;
        }
        employedPopulation = Math.Min(availableWorkers, jobCapacity);
    }

    private void calculatePopulationChange()
    {
        // To handle calculation when population is zero (the start of the game) / Seeding
        if(precisePopulation <= 0 && populationCapacity >= 0)
        {
            precisePopulation = settings.population.seedingPop;
            initializeDemographicDistribution((int)precisePopulation);
        }

        // Prevent negative/over population 
        if(precisePopulation > populationCapacity)
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
        if(population <= 0)
        {
            globalFactor = 1.0f;
            return;
        }
        float availableWorkers = demographicsManager.getWorkForce();
        float employmentRate = (availableWorkers > 0) ? (float)employedPopulation / (float)availableWorkers : 0f;
        float housingRate = (float)population / (float)populationCapacity;
        
        globalFactor = 0.4f * housingRate + 0.3f * employmentRate + 0.3f * (float)goodsSatisfaction;
    }
    
    public void updateGoodsSatisfaction(float change)
    {
        goodsSatisfaction = change;
    }

    public bool haveWorkers()
    {
        return employedPopulation > 0;
    }

    public float getEmploymentRate()
    {
        return employedPopulation / jobCapacity;
    }

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

    private void initializeDemographicDistribution(int pop)
    {
        if(population > 0)
        {
            demographicsManager.initializeDemographics(pop);
        }
    }
}
