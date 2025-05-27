using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x * cellSize, 0, z * cellSize);
    }

    public bool IsInsideGrid(int x, int z)
    {
        return x >= 0 && z >= 0 && x < width && z < height;
    }
}
