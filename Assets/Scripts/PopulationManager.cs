using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public UIController uiController;


    [Header("Current Statistics")]
    public int population;
    public int populationCapacity;
    public int availableJobs;
    public int employedPopulation;


    [Header("Simulation Settings")]
    public float tickRateInSeconds = 2.0f;
    public float citySatisfaction = 100f; // Scale from 0 to 100

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
    

    public void updatePopulationCapacity(int capacity)
    {
        populationCapacity += capacity;
        Debug.Log($"Population capacity updated: {populationCapacity}");
    }

    public void updateNewJobs(int newJobs)
    {
        availableJobs += newJobs;
        Debug.Log($"Jobs updated: {availableJobs}");
    }

    private void runSimulationTick()
    {
        calculateEmployment();
        calculateSatisfaction();
        calculatePopulationChange();

        uiController.displayPopulation(population);
        uiController.displayJobs(availableJobs);
        uiController.displaySatisfaction(citySatisfaction);
    }

    private void calculateEmployment()
    {
        employedPopulation = Math.Min(population, availableJobs);
    }

    private void calculateSatisfaction()
    {
        float employmentRate = population > 0 ? (float)employedPopulation / population : 1.0f;
        float housingRate = populationCapacity > 0 ? (float)population / populationCapacity : 0.0f;

        float targetSatisfaction = (employmentRate * 0.5f + housingRate * 0.5f) * 100f;
        //float targetSatisfaction = 50.0f;

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
        if(citySatisfaction > 60.0f && population < populationCapacity)
        {
            int growth = Mathf.CeilToInt((populationCapacity - population) * 0.05f);
            growth = Mathf.Clamp(growth, 1, 100);
            population = Math.Min(population + growth, populationCapacity);

        }
        //Population decline if thresholds are not met
        else if (citySatisfaction < 40.0f && population > 0)
        {
            int decline = Mathf.CeilToInt(population * 0.03f);
            decline = Mathf.Clamp(decline, 1, 50);
            population = Math.Max(population - decline, 0);
        }
        Debug.Log($"Population : {population} Jobs: {availableJobs} Workers: {employedPopulation} Satisfaction: {citySatisfaction}");
    }
}
