using UnityEngine;

public class BaseStreamingPointer : MonoBehaviour, GeneratorController.IStreamingPointer
{
    public class EventHit
    {
    }
    
    public Vector2 GridCellSize;

    public Vector2Int GetGridCoordinate()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x / GridCellSize.x), Mathf.FloorToInt(transform.position.y / GridCellSize.y));
    }

    public Vector2 GetGridCellSize()
    {
        return GridCellSize;
    }

    void OnCollisionEnter(Collision collision)
    {
        GlobalEventAggregator.EventAggregator.Publish(new EventHit());
    }
}
