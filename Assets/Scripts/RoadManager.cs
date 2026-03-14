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
        if(placementManager.isPositionInBound(position) == false)
        {
            return;
        }
        
        if(placementManager.isPositionFree(position) == false) {
            return;
        }

        if (placementMode == false)
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

            foreach (var positionToFix in roadPositionsToRecheck)
            {
                roadFixer.fixRoadAtPosition(placementManager, positionToFix);
            }

            roadPositionsToRecheck.Clear();

            temporaryPlacementPositions = placementManager.getPathBetween(startPosition, position);

            foreach (var temporaryPosition in temporaryPlacementPositions)
            {
                if (placementManager.isPositionFree(temporaryPosition) == false)
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
                if(roadPositionsToRecheck.Contains(roadPosition) == false)
                {
                    roadPositionsToRecheck.Add(roadPosition);
                }
            }
        }

        foreach (var positionToFix in roadPositionsToRecheck)
        {
            roadFixer.fixRoadAtPosition(placementManager, positionToFix);
        }
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
