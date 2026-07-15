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
    public WaterAndPowerService waterAndPowerService;

    private float[] residentialWeights, commercialWeights, industrialWeights, bigWeights;
    private void Start()
    {
        residentialWeights = structurePrefab.residentialPrefabs.Select(prefabStats=> prefabStats.weight).ToArray();
        commercialWeights = structurePrefab.commercialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        industrialWeights = structurePrefab.industrialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        bigWeights = structurePrefab.bigPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    // Zoning Structures
    public void placeResidential(Vector3Int position)
    {
        IStructurePrefab prefab = placeStructureOnWeight(position, structurePrefab.residentialPrefabs, residentialWeights, CellType.Residential);
        
        updatePopulationCapacity(prefab);
    }

    public void placeCommercial(Vector3Int position)
    {
        IBusinessPrefab prefab = placeStructureOnWeight(position, structurePrefab.commercialPrefabs, commercialWeights, CellType.Commercial);
        
        updateJobAndStorageCapacity(prefab);
    }

    public void placeIndustrial(Vector3Int position)
    {
        IBusinessPrefab prefab = placeStructureOnWeight(position, structurePrefab.industrialPrefabs, industrialWeights, CellType.Industrial);
        
        updateJobAndStorageCapacity(prefab);
        updateProductionRate(prefab);
    }


    // Service Structures
    public void placeWaterPlant(Vector3Int position)
    {
        IServicesPrefab prefab = placeServiceStructure(position, structurePrefab.waterPrefabs);
        waterAndPowerService.updateWaterSupplyCapacity(prefab.GeneratingCapacityPerTick);

    }

    public void placeSewagePlant(Vector3Int position)
    {
        IServicesPrefab prefab = placeServiceStructure(position, structurePrefab.sewagePrefab);
        waterAndPowerService.updateSewageProcessingCapacity(prefab.GeneratingCapacityPerTick);
    }

    public void placePowerPlant(Vector3Int position)
    {
        IServicesPrefab prefab = placeServiceStructure(position, structurePrefab.powerPrefabs);
        waterAndPowerService.updateElectricitySupplyCapacity(prefab.GeneratingCapacityPerTick);
    }



    // ===== General Placement Methods =====
    private T placeStructureOnWeight<T>(Vector3Int position, T[] prefabArray, float[] structureWeights, CellType type) where T : IStructurePrefab
    {
        if (!isPositionPlacable(position))
        {
            return default(T);
        }

        // Get a random prefab based on the weights
        int randomIndex = getRandomWeightedIndex(structureWeights);
        T prefab = prefabArray[randomIndex];

        placementManager.placeObjectOnTheMap(position, prefab.Prefab, type);
        AudioPlayer.instance.PlayPlacementSound();

        updateWaterAndPowerConsumption(prefab.PowerConsumptionPerTick, prefab.WaterConsumptionPerTick);

        return prefab;
    }

    private IServicesPrefab placeServiceStructure(Vector3Int position, IServicesPrefab prefab)
    {
        if (!isPositionPlacable(position))
        {
            return null;
        }

        placementManager.placeObjectOnTheMap(position, prefab.Prefab, CellType.Service);
        AudioPlayer.instance.PlayPlacementSound();


        economyManager.substractConstructionCost(prefab.Cost);
        return prefab;
    }

    public void placeBigStructure(Vector3Int position)
    {
        int width = 2;
        int height = 2;

        if (!isStructureBig(position, width, height))
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
        for (int i = 0; i < weights.Length; i++)
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


    // ===== Update Methods =====
    private void updateJobAndStorageCapacity(IBusinessPrefab prefab)
    {
        populationManager.updateJobCapacity(prefab.Capacity);
        resourcesManager.updateTotalStorageCapacity(prefab.InventoryCapacity);
    }

    private void updateProductionRate(IBusinessPrefab prefab)
    {
        resourcesManager.updateProductionRatePerTimeUnit(prefab.GoodsUnitPerTick);
    }

    private void updatePopulationCapacity(IStructurePrefab prefab)
    {
        populationManager.updatePopulationCapacity(prefab.Capacity);
    }

    private void updateWaterAndPowerConsumption(float power, float water)
    {
        waterAndPowerService.updateConsumptions(power, water);
    }


    // ===== Validation Methods =====
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
        return isPositionInBoundAndFree(position) && isPositionAdjacentToRoad(position);
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
        if (!placementManager.isPositionInBound(position))
        {
            Debug.Log("Position is out of bound, cannot place structure here.");
            return false;
        }

        // Check if the position is free
        if (!placementManager.isPositionFree(position))
        {
            Debug.Log("Position is occupied, cannot place structure here.");
            return false;
        }
        
        return true;
    }
}
