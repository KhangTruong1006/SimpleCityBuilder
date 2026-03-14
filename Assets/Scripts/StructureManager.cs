using SVS;
using System;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted[] housesPrefabs, specialPrefabs;
    public PlacementManager placementManager;

    private float[] houseWeights, specialWeights;

    private void Start()
    {
        houseWeights = housesPrefabs.Select(prefabStates => prefabStates.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStates => prefabStates.weight).ToArray();
    }

    public void placeHouse(Vector3Int position)
    {
        if (checkPositionBeforePlacement(position))
        {
            int randomIndex = getRandomWeightedIndex(houseWeights);
            placementManager.placeObjectOnTheMap(position, housesPrefabs[randomIndex].prefab, CellType.Structure);
            AudioPlayer.instance.PlayPlacementSound();
        };
    }

    public void placeSpecialStructure(Vector3Int position)
    {
        if (checkPositionBeforePlacement(position))
        {
            int randomIndex = getRandomWeightedIndex(specialWeights);
            placementManager.placeObjectOnTheMap(position, specialPrefabs[randomIndex].prefab, CellType.Structure);
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

        float randomValue = UnityEngine.Random.Range(0f, sum);
        float tempSum = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            tempSum += weights[i];
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
        if (placementManager.CheckIfPositionInBound(position) ==  false)
        {
            Debug.Log("Position is out of bound, cannot place structure here.");
            return false;
        }

        // Check if the position is free
        if (placementManager.CheckifPositionIsFree(position) == false)
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
}
