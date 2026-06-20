using JetBrains.Annotations;
using SVS;
using System;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public ResidentialPrefab[] residentialPrefabs;
    public CommericalPrefab[] commercialPrefabs;
    public IndustrialPrefab[] industrialPrefabs;
    public BigPrefab[] bigPrefabs;

    public PlacementManager placementManager;
    public PopulationManager populationManager;
    public EconomyManager economyManager;
    public ResourcesManager resourcesManager;

    private float[] residentialWeights, commercialWeights, industrialWeights, bigWeights;
    private void Start()
    {
        residentialWeights = residentialPrefabs.Select(prefabStats=> prefabStats.weight).ToArray();
        commercialWeights = commercialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        industrialWeights = industrialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        bigWeights = bigPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void placeResidential(Vector3Int position)
    {
        placeStructure(position, residentialPrefabs, residentialWeights, CellType.Residential, capacity => populationManager.updatePopulationCapacity(capacity));
    }

    public void placeCommercial(Vector3Int position)
    {
        placeStructure(position, commercialPrefabs, commercialWeights, CellType.Commercial, capacity => populationManager.updateJobCapacity(capacity));
    }

    public void placeIndustrial(Vector3Int position)
    {
        placeStructure(position, industrialPrefabs, industrialWeights, CellType.Industrial, capacity => populationManager.updateJobCapacity(capacity));
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

        if (prefab is IBusinessPrefab businessPrefab)
        {
            resourcesManager.updateTotalStorageCapacity(businessPrefab.InventoryCapacity);
           
            if(type == CellType.Commercial)
            {
                resourcesManager.updateSalesRatePerTimeUnit(businessPrefab.GoodsUnitPerTick);
            }
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
        placementManager.placeObjectOnTheMap(position, bigPrefabs[randomIndex].Prefab, CellType.Structure, width, height);
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

public interface IStructurePrefab
{
    public GameObject Prefab { get; }
    public float Weight { get; }
    public int Capacity { get; }
}

public interface IBusinessPrefab : IStructurePrefab
{
    public float InventoryCapacity { get; } // Unit: Tons
    public float GoodsUnitPerTick { get; } // Commerical: Sales per tick, Industrial: Produced Freight per tick
}

[Serializable]
public struct ResidentialPrefab : IStructurePrefab
{
    public GameObject prefab;
    
    [Range(0f, 1f)] 
    public float weight;
    public int populationCapacity;
    
    [Range(0f, 1f)]
    public GameObject Prefab => prefab;
    public float Weight => weight;
    public int Capacity => populationCapacity;
}

[Serializable]
public struct IndustrialPrefab : IBusinessPrefab
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float weight;
    public int workerCapacity;
    
    [Min(0f)]
    public float freightPerTick;
    public float inventory; // Unit: Tons
   
    public GameObject Prefab => prefab;
    public float Weight => weight;
    public int Capacity => workerCapacity;
    public float InventoryCapacity => inventory;
    public float GoodsUnitPerTick => freightPerTick;
}

[Serializable]
public struct CommericalPrefab : IBusinessPrefab
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float weight;
    public int workerCapacity;
    
    [Min(0f)]
    public float salesPerTick;
    public float inventory; // Unit: Tons

    public GameObject Prefab => prefab;
    public float Weight => weight;
    public int Capacity => workerCapacity;
    public float InventoryCapacity => inventory;
    public float GoodsUnitPerTick => salesPerTick;
}

[Serializable]
public struct BigPrefab: IStructurePrefab
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
