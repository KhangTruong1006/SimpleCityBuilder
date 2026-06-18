using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public GridSettings gridSettings;

    
    [System.Serializable]
    public class GridSettings
    {
        public int width;
        public int height;
    }
}
