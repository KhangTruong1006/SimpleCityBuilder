using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public StructureManager structureManager;


    [Header("Current Statistics")]
    public int population;
    public int capacity;
    public int jobs;
    public int workers;


    [Header("Simulation Settings")]
    public float tickRateInSeconds = 2.0f;
    public float citySatisfaction = 50.0f; // Scale from 0 to 100

    public float tickTimer = 0.0f;


    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRateInSeconds)
        {
            runSimulationTick();
            tickTimer = 0.0f;
        }
    }
    

    public void updateCapacityAndJobs(int capacityAmount, int jobsAmount)
    {
        capacity += capacityAmount;
        jobs += jobsAmount;

        Debug.Log($"Population capacity updated: {capacity}");
        Debug.Log($"Jobs updated: {jobs}");
    }

   private void runSimulationTick()
    {
        calculateEmployment();
        calculateSatisfaction();
        calculatePopulationChange();
    }

    private void calculateEmployment()
    {
        workers = Math.Min(population, jobs);
    }

    private void calculateSatisfaction()
    {
        float employmentRate = population > 0 ? (float)workers / population : 1.0f;
        float housingRate = capacity > 0 ? (float)population / capacity : 0.0f;

        //float targetSatisfaction = (employmentRate * 0.5f + housingRate * 0.5f) * 100f;
        float targetSatisfaction = 50.0f;

        // If employmen rate is low, satisfaction drops
        if (employmentRate < 0.7f) 
        {
            targetSatisfaction -= 20f;
        }

        // If housing rate is high (No empty space), satisfaction drops
        if (housingRate > 0.9f) 
        {
            targetSatisfaction -= 15f;
        }
        
        // If employment rate is high and there is still some space for population growth, satisfaction increases
        if (employmentRate > 0.9f && housingRate < 0.8f) 
        {
            targetSatisfaction += 20f;
        }

        citySatisfaction = Mathf.Lerp(citySatisfaction, targetSatisfaction, 0.1f);
    }
    private void calculatePopulationChange()
    {
        //Population increase if thresholds are met
        if(citySatisfaction > 60.0f && population < capacity)
        {
            int growth = Mathf.CeilToInt((capacity - population) * 0.05f);
            growth = Mathf.Clamp(growth, 1, 100);
            population = Math.Min(population + growth, capacity);

        }
        //Population decline if thresholds are not met
        else if (citySatisfaction < 40.0f && population > 0)
        {
            int decline = Mathf.CeilToInt(population * 0.03f);
            decline = Mathf.Clamp(decline, 1, 50);
            population = Math.Max(population - decline, 0);
        }

        Debug.Log($"Population : {population} Jobs: {jobs} Workers: {workers} Satisfaction: {citySatisfaction}");
        //Update UI
    }
}
