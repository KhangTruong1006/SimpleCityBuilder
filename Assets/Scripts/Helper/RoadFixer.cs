using System;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEnd, roadStraight, corner, threeWay, fourWay;
    public int rotationDegree = 0; // Default rotation degree for dead end, can be adjusted based on the default orientation of the dead end prefab.
    public void fixRoadAtPosition(PlacementManager placementManager,Vector3Int temporaryPosition)
    {
        //right, up, left , down
        var result = placementManager.getNeighbourTypesFor(temporaryPosition);
        int roadCount = 0;
        roadCount = result.Where(x => x == CellType.Road).Count();
        
        if (roadCount == 0 || roadCount == 1)
        {
            createDeadEnd(placementManager, result, temporaryPosition);
        }

        else if (roadCount == 2)
        {
            if (createStraightRoad(placementManager, result, temporaryPosition))
            {
                return;
            }
            createCorner(placementManager, result, temporaryPosition);
        }

        else if (roadCount == 3)
        {
            createThreeWay(placementManager, result, temporaryPosition);
        }

        else
        {
            createFourWay(placementManager, result, temporaryPosition);
        }

    }

    private void createFourWay(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        placementManager.modifyStructureModel(temporaryPosition, fourWay, Quaternion.identity);
    }

    //[left, up, right, down] - [0, 1, 2, 3]
    private void createThreeWay(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {

        if (checkThreeSurroundingStreets(result, 1,2,3))
        {
            rotationDegree = 0;
        }

        else if (checkThreeSurroundingStreets(result, 2, 3, 0))
        {
            rotationDegree = 90;
        }

        else if (checkThreeSurroundingStreets(result, 3, 0, 1))
        {
            rotationDegree = 180;
        }

        else if (checkThreeSurroundingStreets(result, 0, 1, 2))
        {
            rotationDegree = 270;
        }


        placementManager.modifyStructureModel(position, threeWay, Quaternion.Euler(0, rotationDegree, 0));
    }



    //[left, up, right, down] - [0, 1, 2, 3]
    private void createCorner(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        if (checkTwoSurroundinglStreets(result,1,2))
        {
            rotationDegree = 90;
        }

        else if (checkTwoSurroundinglStreets(result, 2, 3))
        {
            rotationDegree = 180;
        }

        else if (checkTwoSurroundinglStreets(result, 3, 0))
        {
            rotationDegree = 270;
        }

        else if (checkTwoSurroundinglStreets(result, 0, 1))
        {
            rotationDegree = 0;
        }

        placementManager.modifyStructureModel(position, corner, Quaternion.Euler(0, rotationDegree, 0));
    }

    //[left, up, right, down] - [0, 1, 2, 3]
    private bool createStraightRoad(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        if (checkTwoSurroundinglStreets(result, 0, 2))
        {
            placementManager.modifyStructureModel(position, roadStraight, Quaternion.identity);
            return true;
        }

        else if (checkTwoSurroundinglStreets(result, 1, 3))
        {
            placementManager.modifyStructureModel(position, roadStraight, Quaternion.Euler(0,90,0));
            return true;
        }
        return false;
    }


    //[left, up, right, down] - [0, 1, 2, 3]
    private void createDeadEnd(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        
        if (checkSurroundingStreet(result,1))
        {
            rotationDegree = 270;
        }

        else if (checkSurroundingStreet(result, 2))
        {
            rotationDegree = 0;
        }

        else if (checkSurroundingStreet(result, 3))
        {
            rotationDegree = 90;
        }

        else if (checkSurroundingStreet(result, 0))
        {
            rotationDegree = 180;
        }

        placementManager.modifyStructureModel(position, deadEnd, Quaternion.Euler(0, rotationDegree, 0));
    }


    // d -> direction
    //[left, up, right, down] - [0, 1, 2, 3]
    private bool checkThreeSurroundingStreets(CellType[] result, int d1, int d2, int d3)
    {
        return result[d1] == CellType.Road && result[d2] == CellType.Road && result[d3] == CellType.Road;
    }

    private bool checkTwoSurroundinglStreets(CellType[] result, int d1, int d2)
    {
        return result[d1] == CellType.Road && result[d2] == CellType.Road;
    }

    private bool checkSurroundingStreet(CellType[] result, int direction)
    {
        return result[direction] == CellType.Road;
    }
}
