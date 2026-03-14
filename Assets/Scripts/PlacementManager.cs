using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementManager : MonoBehaviour
{
    public int width, height;
    Grid placementGrid;

    private Dictionary<Vector3Int,StructureModel> temporaryRoadObjects =new Dictionary<Vector3Int, StructureModel>();
    private Dictionary<Vector3Int, StructureModel> structureDictionary = new Dictionary<Vector3Int, StructureModel>();

    private void Start()
    {
        placementGrid = new Grid(width, height);
    }

    // check if structure's position is in bound of the map
    internal bool isPositionInBound(Vector3Int position)
    {
        if(position.x >= 0 && position.x < width && position.z >= 0 && position.z < height)
        {
            return true;
        }
        return false;
    }

    // check if the position is empty and can be built on
    internal bool isPositionFree(Vector3Int position)
    {
        return isPositionOfType(position, CellType.Empty);
    }

    private bool isPositionOfType(Vector3Int position, CellType type)
    {
        return placementGrid[position.x, position.z] == type;
    }

    internal void placeTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x, position.z] = type;
        StructureModel structure = createNewStructureModel(position, structurePrefab, type);
        temporaryRoadObjects.Add(position, structure);
    }

    private StructureModel createNewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        GameObject structure = new GameObject(type.ToString());

        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;

        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    public void modifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (temporaryRoadObjects.ContainsKey(position))
        {
            temporaryRoadObjects[position].SwapModel(newModel, rotation);
        }
        else if (structureDictionary.ContainsKey(position))
        {
            structureDictionary[position].SwapModel(newModel, rotation);
        }
    }

    internal CellType[] getNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
    }

    internal List<Vector3Int> getNeighbourOfTypesFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var vertex in neighbourVertices)
        {
            neighbours.Add(new Vector3Int(vertex.X, 0, vertex.Y));
        }
        return neighbours;
    }

    internal void removeAllTemporaryStructures()
    {
        foreach(var structure in temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        temporaryRoadObjects.Clear();
    }

    internal List<Vector3Int> getPathBetween(Vector3Int startPosition, Vector3Int endPosition)
    {
        var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x,startPosition.z), new Point(endPosition.x,endPosition.z));

        List<Vector3Int> path = new List<Vector3Int>();
        foreach (Point point in resultPath)
        {
            path.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return path;
    }

    internal void addTemporaryStructuresToStructureDictionary()
    {
        foreach(var structure in temporaryRoadObjects)
        {
            structureDictionary.Add(structure.Key, structure.Value);
            destroyNature(structure.Key);
        }
        temporaryRoadObjects.Clear();
    }

    internal void placeObjectOnTheMap(Vector3Int position, GameObject prefab, CellType type)
    {
        placementGrid[position.x, position.z] = type;
        StructureModel structure = createNewStructureModel(position, prefab, type);
        structureDictionary.Add(position, structure);

        destroyNature(position);
    }

    private void destroyNature(Vector3Int position)
    {
        RaycastHit[] hits = Physics.BoxCastAll(position + new Vector3(0,0.5f,0), new Vector3(0.5f, 0.5f, 0.5f), 
            transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Nature"));
        foreach (var hit in hits)
        {
            Destroy(hit.collider.gameObject);
        }
    }
}
