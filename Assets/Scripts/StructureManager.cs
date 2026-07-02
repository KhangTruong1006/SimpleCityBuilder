using JetBrains.Annotations;
using SVS;
using System;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private StructurePrefab structurePrefab;

    public PlacementManager placementManager;
    public PopulationManager populationManager;
    public EconomyManager economyManager;
    public ResourcesManager resourcesManager;

    private float[] residentialWeights, commercialWeights, industrialWeights, bigWeights;
    private void Start()
    {
        residentialWeights = structurePrefab.residentialPrefabs.Select(prefabStats=> prefabStats.weight).ToArray();
        commercialWeights = structurePrefab.commercialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        industrialWeights = structurePrefab.industrialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        bigWeights = structurePrefab.bigPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void placeResidential(Vector3Int position)
    {
        placeStructure(position, structurePrefab.residentialPrefabs, residentialWeights, CellType.Residential, capacity => populationManager.updatePopulationCapacity(capacity));
    }

    public void placeCommercial(Vector3Int position)
    {
        placeStructure(position, structurePrefab.commercialPrefabs, commercialWeights, CellType.Commercial, capacity => populationManager.updateJobCapacity(capacity));
    }

    public void placeIndustrial(Vector3Int position)
    {
        placeStructure(position, structurePrefab.industrialPrefabs, industrialWeights, CellType.Industrial, capacity => populationManager.updateJobCapacity(capacity));
    }

    private void placeStructure<T>(Vector3Int position, T[] prefabArray, float[] structureWeights, CellType type, Action<int> action) where T : IStructurePrefab
    {
        if (!isPositionPlacable(position))
        {
            return;
        }

        int randomIndex = getRandomWeightedIndex(structureWeights);
        T prefab = prefabArray[randomIndex];

        placementManager.placeObjectOnTheMap(position, prefab.Prefab, type);
        AudioPlayer.instance.PlayPlacementSound();

        action?.Invoke(prefab.Capacity);

        checkBusinessPrefab((IBusinessPrefab)prefab, type);

    }

    private void checkBusinessPrefab(IBusinessPrefab prefab, CellType type)
    {
        if (prefab is IBusinessPrefab businessPrefab)
        {
            resourcesManager.updateTotalStorageCapacity(businessPrefab.InventoryCapacity);

            if (type == CellType.Industrial)
            {
                resourcesManager.updateProductionRatePerTimeUnit(businessPrefab.GoodsUnitPerTick);
            }
        }
    }

    public void placeBigStructure(Vector3Int position)
    {
        int width = 2;
        int height = 2;

        if(!isStructureBig(position, width, height))
        {
            return;
        }

        int randomIndex = getRandomWeightedIndex(bigWeights);
        placementManager.placeObjectOnTheMap(position, structurePrefab.bigPrefabs[randomIndex].Prefab, CellType.Structure, width, height);
        AudioPlayer.instance.PlayPlacementSound();
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

    private bool isStructureBig(Vector3Int position, int width, int height)
    {
        bool nearbyRoad = false;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var newPosition = position + new Vector3Int(i, 0, j);

                if (!isPositionInBoundAndFree(newPosition))
                {
                    return false;
                }

                if (!nearbyRoad)
                {
                    nearbyRoad = isPositionAdjacentToRoad(newPosition);
                }
            }
        }
        return nearbyRoad;
    }
    private bool isPositionPlacable(Vector3Int position)
    {
        if (!isPositionInBoundAndFree(position))
        {
            return false;
        }
        // Check if the position is adjacent to a road
        if (!isPositionAdjacentToRoad(position))
        {
            return false;
        }
        return true;
    }

    private bool isPositionAdjacentToRoad(Vector3Int position)
    {
        if (placementManager.getNeighbourOfTypesFor(position, CellType.Road).Count <= 0)
        {
            Debug.Log("Position is not adjacent to a road, cannot place structure here.");
            return false;
        }
        return true;
    } 

    private bool isPositionInBoundAndFree(Vector3Int position)
    {
        // Check if the position is within bounds
        if (placementManager.isPositionInBound(position) == false)
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
        
        return true;
    }
}


