using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public MasterSettings masterSettings;
    public Camera camera;
    public GridSettings gridSettings;
    public Population population;


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
    }
}
