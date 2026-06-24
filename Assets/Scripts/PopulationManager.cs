using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public UIController uiController;


    [Header("Current Statistics")]
    public int population;
    public int populationCapacity;
    public int jobCapacity;
    public int employedPopulation;

    [Range(0.0f, 1.0f)]
    public float goodsSatisfaction = 1.0f;
    [Range(0.0f, 1.0f)]
    public float workersThreshold = 0.25f;

    private float growthRate;
    public float precisePopulation = 0f;
    private float globalFactor;


    private void Start()
    {
        precisePopulation = population;
        //DemandManager demandManager = GetComponent<DemandManager>(); //For future expansion
    }

    public void runSimulationTick()
    {
        // Change Employment to Demand
        calculateEmployment();
        calculateGlobalFactor();
        calculatePopulationChange();

        uiController.displayPopulation(population);
        uiController.displayJobs(jobCapacity);
    }

    private void calculateEmployment()
    {
        if(population <1)
        {
            employedPopulation = 0;
            return;
        }
        employedPopulation = Math.Min(population, jobCapacity);
    }

    private void calculatePopulationChange()
    {
        // To handle calculation when population is zero (the start of the game) / Seeding
        if(precisePopulation <= 0 && populationCapacity >= 0)
        {
            precisePopulation = 0.5f;
        }

        // Prevent negative population 
        if(precisePopulation > populationCapacity)
        {
            return;
        }

        // This method uses Logistic Growth Model based on various factors (metrics)
        // rate = base * factors point * population * (1 - population / capacity)

        float basedGrowthRate = 0.1f;
        float growthRate = basedGrowthRate * globalFactor * precisePopulation * (1f - (precisePopulation / populationCapacity));
        //float growthRate = basedGrowthRate * precisePopulation * (1f - (precisePopulation / populationCapacity));

        precisePopulation += growthRate;
        population = Mathf.FloorToInt(precisePopulation);

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
        float employmentRate = (float)employedPopulation / (float)population;
        float housingRate = (float)population / (float)populationCapacity;
        
        globalFactor = 0.3f * housingRate + 0.4f * employmentRate + 0.3f * (float)goodsSatisfaction;
    }
    
    public void updateGoodsSatisfaction(float change)
    {
        goodsSatisfaction = change;
    }

    public bool haveWorkers()
    {
        return employedPopulation > 0;
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
}
