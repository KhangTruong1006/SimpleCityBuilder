using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public MasterSettings masterSettings;
    public Camera camera;
    public GridSettings gridSettings;
    public Threshold threshold;
    public Population population;
    public Economy economy;


    [System.Serializable]
    public class MasterSettings
    {
        // Game speed (second)
        public float tickRateInSeconds = 2.0f;
        public float speed_1;
        public float speed_2;
        public float speed_3 = 1.0f;
    }

    [System.Serializable]
    public class Camera
    {
        public float speed = 5.0f;
    }


    [System.Serializable]
    public class GridSettings
    {
        public int width;
        public int height;
    }

    [System.Serializable]
    public class Population {
        public float seedingPop = 0.5f;
        public float basedGrowthRate = 0.5f;
    }

    [System.Serializable]
    public class Threshold 
    {
        public float workersThreshold = 0.25f;
        public float productionThreshold = 0.5f;
        public float exportThreshold = 0.4f;
    }

    [System.Serializable]
    public class Economy
    {
        public float initialBudget = 1000f;
        public float taxRate = 0.1f;

        public float exportRevenuePerUnit = 10f;
        public float importCostPerUnit = 25f;
        public float productionCostPerUnit = 2f;
        public float salePricePerUnit = 3f;
    }
}
