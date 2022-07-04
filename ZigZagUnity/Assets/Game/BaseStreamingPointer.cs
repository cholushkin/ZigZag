using UnityEngine;

public class BaseStreamingPointer : MonoBehaviour, GeneratorController.IStreamingPointer
{
    public Vector2Int GridCellSize;

    public Vector2Int GetGridCoordinate()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x / GridCellSize.x), Mathf.FloorToInt(transform.position.y / GridCellSize.y));
    }

    public Vector2Int GetGridCellSize()
    {
        return GridCellSize;
    }
}
