using NUnit.Framework;
using SVS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;
    public List<Vector3Int> temporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;
    
    public RoadFixer roadFixer;

    private void Start()
    {
       roadFixer = GetComponent<RoadFixer>();
    }
    
    public void PlaceRoad(Vector3Int position)
    {

        if (!isPositionInBoundOrFree(position))
        {
            return;
        }

        if (!placementMode)
        {
            temporaryPlacementPositions.Clear();
            roadPositionsToRecheck.Clear();

            placementMode = true;
            startPosition = position;

            temporaryPlacementPositions.Add(position);
            placementManager.placeTemporaryStructure(position, roadFixer.deadEnd, CellType.Road);
        }

        else
        {
            placementManager.removeAllTemporaryStructures();
            temporaryPlacementPositions.Clear();


            fixSurrondingPositions();

            roadPositionsToRecheck.Clear();

            temporaryPlacementPositions = placementManager.getPathBetween(startPosition, position);

            foreach (var temporaryPosition in temporaryPlacementPositions)
            {
                if (!placementManager.isPositionFree(temporaryPosition))
                {
                    continue;
                }
                placementManager.placeTemporaryStructure(temporaryPosition, roadFixer.deadEnd, CellType.Road);
            }
        }
        
        fixRoadPrefabs();
    }

    private void fixRoadPrefabs()
    {
        foreach (var temporaryPosition in temporaryPlacementPositions)
        {
            roadFixer.fixRoadAtPosition(placementManager, temporaryPosition);
            var neighbours = placementManager.getNeighbourOfTypesFor(temporaryPosition, CellType.Road);
            
            foreach (var roadPosition in neighbours)
            {
                if(!roadPositionsToRecheck.Contains(roadPosition))
                {
                    roadPositionsToRecheck.Add(roadPosition);
                }
            }
        }

        fixSurrondingPositions();
    }

    private void fixSurrondingPositions()
    {
        foreach (var position in roadPositionsToRecheck)
        {
            roadFixer.fixRoadAtPosition(placementManager, position);
        }
    }

    private bool isPositionInBoundOrFree(Vector3Int position)
    {
        return placementManager.isPositionInBound(position) && placementManager.isPositionFree(position);
    }

    public void finishPlacingRoad()
    {
        placementMode = false;
        placementManager.addTemporaryStructuresToStructureDictionary();
        
        if(temporaryPlacementPositions.Count > 0)
        {
            AudioPlayer.instance.PlayPlacementSound();
        }
        
        temporaryPlacementPositions.Clear();
        startPosition = Vector3Int.zero;
    }
}
