using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public StructureManager structureManager;

    public int population;
    public int maxPopulation;
    [Range(0f,1f)]
    public float growthRate = 0.02f;
    public float timeBetweenUpdates = 5f;

    public float timer;


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBetweenUpdates)
        {
            timer = 0f;
            UpdatePopulation();
        }
    }

    private void UpdatePopulation()
    {
        float growth;
        if (population < 100) {
            growth = 1;
        }
        else
        {
            growth = population * growthRate * (1 - population/maxPopulation);
        }

        population += Mathf.RoundToInt(growth);
        population = Mathf.Clamp(population, 0, maxPopulation);
        //population = Mathf.Min(population, maxPopulation);

        Debug.Log($"Population updated: {population}");
        //Update UI
    }

    public void UpdateCapacity(int amount)
    {
        maxPopulation += amount;
        Debug.Log($"Population capacity updated: {maxPopulation}");
    }
}
