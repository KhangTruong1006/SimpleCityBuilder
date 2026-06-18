using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public Camera camera;
    public GridSettings gridSettings;


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
}
