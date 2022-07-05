using System.Collections.Generic;
using System.Linq;
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
        public int GenerationID;
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
        GenerateAround(StreamingPointer, 4);
    }

    void Update()
    {
        var gridCoord = StreamingPointer.GetGridCoordinate();

        if (gridCoord != _prevPos)
        {
            _prevPos = gridCoord;
            GenerateAround(StreamingPointer, 4);
        }
    }

    readonly Vector2Int[] _directions = { new(1, 0), new(0, -1), new(-1, 0), new(0, 1) };
    private void GenerateAround(IStreamingPointer pointer, int radius)
    {
        var gridCoordinate = pointer.GetGridCoordinate();

        Debug.Log($"Generate for grid coordinate: {gridCoordinate}");

        if(!_activeChunks.ContainsKey(gridCoordinate))
            _activeChunks.Add(gridCoordinate, GenerateChunk(gridCoordinate * pointer.GetGridCellSize()));
        _activeChunks[gridCoordinate].GenerationID = Time.frameCount;

        Vector2Int curPointer = Vector2Int.zero;
        for (int r = 1; r <= radius; ++r)
        {
            curPointer.Set(-r,r);
            for (int dirIndex = 0; dirIndex < 4; ++dirIndex)
            {
                var dir = _directions[dirIndex];
                for (int step = 0; step < r*2; ++step)
                {
                    var pos = gridCoordinate + curPointer;
                    if (!_activeChunks.ContainsKey(pos))
                        _activeChunks.Add(pos, GenerateChunk(pos * pointer.GetGridCellSize()));
                    _activeChunks[pos].GenerationID = Time.frameCount;
                    curPointer += dir;
                }
            }
        }

        // clean around
        var toRemove = _activeChunks.Where(kv => kv.Value.GenerationID != Time.frameCount).ToArray();
        foreach (var keyValuePair in toRemove)
            _activeChunks.Remove(keyValuePair.Key);
    }

    private Chunk GenerateChunk(Vector2Int cellCoord)
    {
        var chunk = new Chunk
        {
            Rects = ZoneGenerator.Generate(ChunkSize.x, ChunkSize.y, new Vector3(cellCoord.x, cellCoord.y, 0))
        };
        return chunk;
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
