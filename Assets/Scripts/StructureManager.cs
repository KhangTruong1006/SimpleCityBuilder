using JetBrains.Annotations;
using SVS;
using System;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public ResidentialPrefab[] residentialPrefabs;
    public BusinessPrefab[] commercialPrefabs, industrialPrefabs;

    public PlacementManager placementManager;
    public PopulationManager populationManager;

    private float[] residentialWeights, commercialWeights, industrialWeights, specialWeights;
    private void Start()
    {
        residentialWeights = residentialPrefabs.Select(prefabStats=> prefabStats.weight).ToArray();
        commercialWeights = commercialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        industrialWeights = industrialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        //specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void placeResidential(Vector3Int position)
    {
        placeStructure(position, residentialPrefabs, residentialWeights, CellType.Residential, capacity => populationManager.updatePopulationCapacity(capacity));
    }

    public void placeCommercial(Vector3Int position)
    {
        placeStructure(position, commercialPrefabs, commercialWeights, CellType.Commercial, capacity => populationManager.updateNewJobs(capacity));
    }

    private void placeIndustrial(Vector3Int position)
    {
        placeStructure(position, industrialPrefabs, industrialWeights, CellType.Industrial, capacity => populationManager.updateNewJobs(capacity));
    }

    //public void placeSpecialStructure(Vector3Int position)
    //{
    //    placeStructure(position, specialPrefabs, specialWeights);
    //}

    private void placeStructure<T>(Vector3Int position, T[] prefabArray, float[] structureWeights, CellType type,Action<int> action) where T : IStructurePrefab
    {
        if (!checkPositionBeforePlacement(position))
        {
           return;
        }
        int randomIndex = getRandomWeightedIndex(structureWeights);
        T prefab = prefabArray[randomIndex];
        
        placementManager.placeObjectOnTheMap(position, prefab.Prefab, type);
        AudioPlayer.instance.PlayPlacementSound();
        
        action?.Invoke(prefab.Capacity);
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

public interface IStructurePrefab
{
    public GameObject Prefab { get; }
    public float Weight { get; }
    public int Capacity { get; }
}

[Serializable]
public struct ResidentialPrefab : IStructurePrefab
{
    public GameObject prefab;
    
    [Range(0f, 1f)] 
    public float weight;
    public int populationCapacity;
    
    [Min(0)] 
    public int population;

    public GameObject Prefab => prefab;
    public float Weight => weight;
    public int Capacity => populationCapacity;
}

[Serializable]
public struct BusinessPrefab : IStructurePrefab
{
    public GameObject prefab;
    [Range(0f, 1f)] 
    public float weight;
    public int workerCapacity;
    
    [Min(0)] 
    public int worker;
    
    public GameObject Prefab => prefab;
    public float Weight => weight;
    public int Capacity => workerCapacity;
    
}
