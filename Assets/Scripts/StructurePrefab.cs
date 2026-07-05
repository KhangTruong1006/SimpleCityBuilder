using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StructurePrefab", menuName = "Structure Prefab")]
public class StructurePrefab : ScriptableObject
{
    public ResidentialPrefab[] residentialPrefabs;
    public CommericalPrefab[] commercialPrefabs;
    public IndustrialPrefab[] industrialPrefabs;
    public BigPrefab[] bigPrefabs;
    public ServicePrefab[] servicePrefabs;
}

// === Interface ===
public interface IStructurePrefab
{
    public GameObject Prefab { get; }
    public float Weight { get; }
    public int Capacity { get; }

    public int Cost { get; }
}

public interface IBusinessPrefab : IStructurePrefab
{
    public float InventoryCapacity { get; } // Unit: Tons
    public float GoodsUnitPerTick { get; } // Commerical: Sales per tick, Industrial: Produced Freight per tick
}

public interface IServicesPrefab
{
    public GameObject Prefab { get; }
    public int Cost { get; }
    public float ExpensePerTick { get; } // Unit: Money per tick
}

// === Struct ===

[Serializable]
public struct ResidentialPrefab : IStructurePrefab
{
    public GameObject prefab;
    public int cost;

    [Range(0f, 1f)]
    public float weight;
    public int populationCapacity;


    [Range(0f, 1f)]
    public GameObject Prefab => prefab;
    public float Weight => weight;
    public int Capacity => populationCapacity;
    public int Cost => cost;
}

[Serializable]
public struct IndustrialPrefab : IBusinessPrefab
{
    public GameObject prefab;
    public int cost;
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
    public int Cost => cost;
}

[Serializable]
public struct CommericalPrefab : IBusinessPrefab
{
    public GameObject prefab;
    public int cost;
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
    public int Cost => cost;   
}

[Serializable]
public struct BigPrefab : IStructurePrefab
{
    public GameObject prefab;
    public int cost;
    [Range(0f, 1f)]
    public float weight;
    public int workerCapacity;

    [Min(0)]
    public int worker;

    public GameObject Prefab => prefab;
    public float Weight => weight;
    public int Capacity => workerCapacity;
    public int Cost => cost;
}

[Serializable]
public struct ServicePrefab : IServicesPrefab
{
    public GameObject prefab;
    public int cost;
    [Min(0f)]
    public float expensePerTick;


    public GameObject Prefab => prefab;
    public float ExpensePerTick => expensePerTick;
    public int Cost => cost;
}