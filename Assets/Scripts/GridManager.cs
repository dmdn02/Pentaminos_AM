using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 8;
    public int height = 8;

    [SerializeField]
    public float cellSize = 1.05f;

    public static float CellSize => Instance.cellSize;

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
