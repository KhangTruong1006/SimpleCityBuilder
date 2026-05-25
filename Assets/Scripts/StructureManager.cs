using JetBrains.Annotations;
using SVS;
using System;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted[] housesPrefabs, commercialPrefabs, industrialPrefabs, specialPrefabs;
    public PlacementManager placementManager;
    public PopulationManager populationManager;

    private float[] houseWeights, commercialWeights, industrialWeights, specialWeights;
    private void Start()
    {
        houseWeights = housesPrefabs.Select(prefabStats=> prefabStats.weight).ToArray();
        commercialWeights = commercialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        industrialWeights = industrialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void placeHouse(Vector3Int position)
    {
        placeStructure(position, housesPrefabs, houseWeights);
    }

    public void placeCommercial(Vector3Int position)
    {
        placeStructure(position, commercialPrefabs, commercialWeights);
    }

    private void placeIndustrial(Vector3Int position)
    {
        placeStructure(position, industrialPrefabs, industrialWeights);
    }
    public void placeSpecialStructure(Vector3Int position)
    {
        placeStructure(position, specialPrefabs, specialWeights);
    }

    private void placeStructure(Vector3Int position, StructurePrefabWeighted[] prefabArray, float[] structureWeights)
    {
        if (checkPositionBeforePlacement(position))
        {
            int randomIndex = getRandomWeightedIndex(structureWeights);
            placementManager.placeObjectOnTheMap(position, prefabArray[randomIndex].prefab, CellType.Structure);
            populationManager.updateCapacityAndJobs(prefabArray[randomIndex].population, prefabArray[randomIndex].job);
            AudioPlayer.instance.PlayPlacementSound();
        };
    }

    private int getRandomWeightedIndex(float[] weights)
    {
        float sum = 0f;
        for(int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0, sum);
        float tempSum = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue >= tempSum && randomValue < tempSum + weights[i])
            {
                return i;
            }
            tempSum += weights[i];
        }
        return 0; // Fallback, should not reach here if weights are valid
    }

    private bool checkPositionBeforePlacement(Vector3Int position)
    {
        // Check if the position is within bounds
        if (placementManager.isPositionInBound(position) ==  false)
        {
            Debug.Log("Position is out of bound, cannot place structure here.");
            return false;
        }

        // Check if the position is free
        if (placementManager.isPositionFree(position) == false)
        {
            Debug.Log("Position is occupied, cannot place structure here.");
            return false;
        }

        // Check if the position is adjacent to a road
        if (placementManager.getNeighbourOfTypesFor(position,CellType.Road).Count <= 0)
        {
            Debug.Log("Position is not adjacent to a road, cannot place structure here.");
            return false;
        }
        return true;
    }
}

[Serializable]
public struct StructurePrefabWeighted
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float weight;
    [Min(0)]
    public int population;
    [Min(0)]
    public int job;

    //economy
    [Min(0)]
    public int cost;
    [Min(0)]
    public int income;
    [Min(0)]
    public int spending;
    [Range(0f, 1f)]
    public int tax;
}
