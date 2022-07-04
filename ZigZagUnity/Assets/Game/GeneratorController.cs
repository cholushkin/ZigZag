using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : MonoBehaviour
{
    public interface IStreamingPointer
    {
        Vector2Int GetGridCoordinate();
        Vector2Int GetGridCellSize();
    }

    public class Chunk
    {
        public List<Rect> Rects;
        // monsters
        // decoration
    }

    public Vector2Int ChunkSize;
    public BaseStreamingPointer StreamingPointer; // pointer which tells where to generate current zone
    public ZoneGenerator ZoneGenerator;

    private Vector2Int _prevPos; // prev position of pointer to determine when streaming pointer changes
    private Dictionary<Vector2Int, Chunk> _activeChunks = new(32);

    void Awake()
    {
        GenerateAround(StreamingPointer);
    }

    void Update()
    {
        var gridCoord = StreamingPointer.GetGridCoordinate();
        print(gridCoord);

        if (gridCoord != _prevPos)
        {
            _prevPos = gridCoord;
            GenerateAround(StreamingPointer);
        }
    }

    private void GenerateAround(IStreamingPointer pointer)
    {
        var gridCoordinate = pointer.GetGridCoordinate();
        if (_activeChunks.ContainsKey(gridCoordinate))
            return;

        Debug.Log($"Generate for grid coordinate: {gridCoordinate}");

        var chunk = new Chunk();
        chunk.Rects = ZoneGenerator.Generate(ChunkSize.x, ChunkSize.y, new Vector3(gridCoordinate.x * pointer.GetGridCellSize().x, gridCoordinate.y * pointer.GetGridCellSize().y, 0));
        _activeChunks.Add(gridCoordinate, chunk);
    }

    private void OnDrawGizmos()
    {
        void DrawRect(Rect rect, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(rect.center, new Vector3(rect.width, rect.height, 2));
        }

        foreach (var activeChunk in _activeChunks)
        {
            foreach (var rect in activeChunk.Value.Rects)
            {
                DrawRect(rect, Color.white);
            }
        }
    }
}
