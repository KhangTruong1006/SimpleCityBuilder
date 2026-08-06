using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    public StatsPanelController statsPanelController;

    public DemandController demandController;
    public DemographicsManager demographicsManager;
    public UIController uiController;


    [Header("Current Statistics")]
    public int population;
    public int populationCapacity;
    public int jobCapacity;
    public int employedPopulation;
    public int employablePopulation;

    [Range(0.0f, 1.0f)]
    public float goodsSatisfaction = 1.0f;

    private float baseGrowthRate;
    public float precisePopulation = 0f;
    public float globalFactor;

    public float hiringSpeed;
    public float naturalUnemploymentRate;

    private void Awake()
    {
        demographicsManager = GetComponent<DemographicsManager>();
    }

    private void Start()
    {
        hiringSpeed = settings.population.hiringSpeed;
        naturalUnemploymentRate = settings.population.naturalUnemploymentRate;
        baseGrowthRate = settings.population.basedGrowthRate;

        precisePopulation = population;
        initializeDemographicDistribution(population); // If the city starts with a population, initialize the demographics distribution
    }

    public void runSimulationTick()
    {
        calculateEmployment();
        calculateGlobalFactor();
        calculatePopulationChange();

        uiController.displayPopulation(population);
        statsPanelController.displayEmploymentStats(getEmploymentRate(),employedPopulation, employablePopulation, jobCapacity);
    }

    private void calculatePopulationChange()
    {
        // To handle calculation when population is zero (the start of the game) / Seeding
        if (precisePopulation <= 0 && populationCapacity >= 0)
        {
            precisePopulation = settings.population.seedingPop;
            initializeDemographicDistribution((int)precisePopulation);
        }
        if (populationCapacity <= 0){
            return;
        }

        float capacityRatio = Mathf.Max(0f, 1.0f - (precisePopulation / populationCapacity));
        float growthRate = baseGrowthRate * globalFactor * precisePopulation * capacityRatio;

        // Force negative growth if overpopulation
        if (precisePopulation > populationCapacity)
        {
            growthRate = -baseGrowthRate * (precisePopulation - populationCapacity);
        }

        // This method uses Logistic Growth Model based on various factors (metrics)
        // rate = base * factors point * population * (1 - population / capacity)

       
        int updatedPopulation = demographicsManager.updateDemographics(growthRate, populationCapacity);
        population = Mathf.Clamp(updatedPopulation, 0, populationCapacity);
        precisePopulation = population;
    }
    private void calculateEmployment()
    {
        employablePopulation = demographicsManager.getEmployablePopulation();

        if (employablePopulation < 1)
        {
            employedPopulation = 0;
            return;
        }

        int targetWorkforce = Mathf.FloorToInt(employablePopulation * (1f - naturalUnemploymentRate));
        int targetEmployed = Mathf.Min(targetWorkforce, jobCapacity);

        float lerpedEmployment = Mathf.Lerp(employedPopulation, targetEmployed, hiringSpeed);
        employedPopulation = Mathf.RoundToInt(lerpedEmployment);
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
;       float openJobsRatio = calculateJobsRatio();
        float lifeSatisfaction = goodsSatisfaction; // Add tax

        float jobAttractionFactor =  1.0f + openJobsRatio * 1.5f;
        float satisfactionFactor = 0.5f + lifeSatisfaction * 0.5f;

        globalFactor = Mathf.Clamp01(jobAttractionFactor + satisfactionFactor);
    }

    public float calculateJobsRatio()
    {
        if(jobCapacity <= 0)
        {
            return 0f;
        }
        int openJobs = Mathf.Max(0, jobCapacity - employedPopulation);
        float jobRatio = (float)openJobs / jobCapacity;
        return jobRatio;
    }

    public bool haveWorkers()
    {
        return employedPopulation > 0;
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
        if (population > 0)
        {
            demographicsManager.initializeDemographics(pop);
        }
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

    public float getEmploymentAndJobRatio()
    {
        return (float)employedPopulation / jobCapacity;
    }
    public float getEmploymentRate()
    {
        if (employablePopulation <= 0)
        {
            return 0f;
        }
        return (float)employedPopulation / population;
    } 

    public int getEmployablePopulation()
    {
        return employablePopulation;
    }

    public float getGoodsSatisfaction()
    {
        return goodsSatisfaction;
    }
}
