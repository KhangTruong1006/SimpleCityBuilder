using System;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEnd, roadStraight, corner, threeWay, fourWay;
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
            if(createStraightRoad(placementManager, result, temporaryPosition))
            {
                return;
            }
            createCorner(placementManager, result, temporaryPosition);
        }

        else if(roadCount == 3)
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
    private void createThreeWay(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, threeWay, Quaternion.identity);
        }
        
        else if (result[2] == CellType.Road && result[3] == CellType.Road && result[0] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0,90,0));
        }

        else if (result[3] == CellType.Road && result[0] == CellType.Road && result[1] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0, 180, 0));
        }

        else if (result[0] == CellType.Road && result[1] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0, 270, 0));
        }
    }


    //[left, up, right, down] - [0, 1, 2, 3]
    private void createCorner(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road )
        {
            placementManager.modifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0, 90, 0));
        }

        else if (result[2] == CellType.Road && result[3] == CellType.Road )
        {
            placementManager.modifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0, 180, 0));
        }

        else if (result[3] == CellType.Road && result[0] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0, 270, 0));
        }

        else if (result[0] == CellType.Road && result[1] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, corner, Quaternion.identity);
        }
    }

    //[left, up, right, down] - [0, 1, 2, 3]
    private bool createStraightRoad(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        if (result[0] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, roadStraight, Quaternion.identity);
            return true;
        }

        else if (result[1] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, roadStraight, Quaternion.Euler(0,90,0));
            return true;
        }
        return false;
    }


    //[left, up, right, down] - [0, 1, 2, 3]
    private void createDeadEnd(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        if (result[1] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0, 270, 0));
        }

        else if (result[2] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, deadEnd, Quaternion.identity);
        }

        else if (result[3] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0, 90, 0));
        }

        else if (result[0] == CellType.Road)
        {
            placementManager.modifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0, 180, 0));
        }
    }
}
