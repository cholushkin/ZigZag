using System.Collections.Generic;
using System.Linq;
using GameLib.Random;
using UnityEngine;

public class GeneratorController : MonoBehaviour
{
    public interface IStreamingPointer
    {
        Vector2Int GetGridCoordinate();
        Vector2 GetGridCellSize();
    }

    public class Chunk
    {
        public int GenerationID;
        public List<GameObject> Buildings;
        public List<Rect> Rects;
        public List<GameObject> Decorations;
        public GameObject ChunkParent;
    }

    public class ChunkState
    {
        public bool IsDestroyed;
    }


    [Range(0f, 1f)]
    [Tooltip("Chance to offset entire chunk to close neighbor passages")]
    public float ChanceOffsetChunk;

    [Range(0f, 1f)]
    [Tooltip("Chance to generate second layer of buildings inside same chunk")]
    public float ChanceGenerateSecondLayer;

    public long CoreSeed;
    public Vector2 ChunkSize;
    public BaseStreamingPointer StreamingPointer; // pointer which tells where to generate current zone
    public ZoneGenerator ZoneGenerator;
    public ZoneGenerator ZoneGeneratorLayer2;
    public DecorationGenerator DecorationGenerator;
    public BuildingGenerator BuildingGenerator;

    private IPseudoRandomNumberGenerator _rnd;
    private Vector2Int _prevPos; // prev position of pointer to determine when streaming pointer changes
    private Dictionary<Vector2Int, Chunk> _activeChunks = new(32);
    readonly Vector2Int[] _directions = { new(1, 0), new(0, -1), new(-1, 0), new(0, 1) };

    void Awake()
    {
        _rnd = RandomHelper.CreateRandomNumberGenerator(CoreSeed);
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

    private void GenerateAround(IStreamingPointer pointer, int radius)
    {
        var gridCoordinate = pointer.GetGridCoordinate();

        Debug.Log($"Generate for grid coordinate: {gridCoordinate}");

        if (!_activeChunks.ContainsKey(gridCoordinate))
            _activeChunks.Add(gridCoordinate, GenerateChunk(gridCoordinate * pointer.GetGridCellSize(), GetSeedForCoord(gridCoordinate)));
        _activeChunks[gridCoordinate].GenerationID = Time.frameCount;

        Vector2Int curPointer = Vector2Int.zero;
        for (int r = 1; r <= radius; ++r)
        {
            curPointer.Set(-r, r);
            for (int dirIndex = 0; dirIndex < 4; ++dirIndex)
            {
                var dir = _directions[dirIndex];
                for (int step = 0; step < r * 2; ++step)
                {
                    var pos = gridCoordinate + curPointer;

                    var seed = GetSeedForCoord(pos);

                    if (!_activeChunks.ContainsKey(pos))
                        _activeChunks.Add(pos, GenerateChunk(pos * pointer.GetGridCellSize(), seed));
                    _activeChunks[pos].GenerationID = Time.frameCount;
                    curPointer += dir;
                }
            }
        }

        // clean around
        var toRemove = _activeChunks.Where(kv => kv.Value.GenerationID != Time.frameCount).ToArray();
        foreach (var keyValuePair in toRemove)
        {
            _activeChunks.Remove(keyValuePair.Key);
            DestroyChunk(keyValuePair.Value);
        }
    }

    private Chunk GenerateChunk(Vector2 absCoord, long seed)
    {
        var rects = ZoneGenerator.Generate(ChunkSize, absCoord, seed);
        _rnd.SetState(new LinearConRng.State(seed));
        if (ZoneGeneratorLayer2 != null && _rnd.ValueFloat() < ChanceGenerateSecondLayer)
            rects.AddRange(ZoneGeneratorLayer2.Generate(ChunkSize, absCoord, seed));

        var chunkParent = new GameObject($"Chunk{absCoord}");
        chunkParent.transform.position = absCoord;
        chunkParent.transform.SetParent(transform);

        var buildings = BuildingGenerator.Generate(rects, seed);
        foreach (var building in buildings)
        {
            building.transform.SetParent(chunkParent.transform);
        }

        // Randomly offset entire chunk
        if (_rnd.ValueFloat() < ChanceOffsetChunk)
        {
            // get random offset
            var offset = _rnd.FromArray(_directions);
            chunkParent.transform.Translate(new Vector3(offset.x, offset.y, 0) * 2); // distance enough to close the passage
        }

        var chunk = new Chunk
        {
            Rects = rects,
            //Decorations = DecorationGenerator.Generate(ChunkSize.x, ChunkSize.y, new Vector3(cellCoord.x, cellCoord.y, 0)),
            Buildings = buildings,
            ChunkParent = chunkParent
        };
        return chunk;
    }

    private void DestroyChunk(Chunk chunk)
    {
        Destroy(chunk.ChunkParent);
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

    private static long GetSeedForCoord(Vector2Int gridCoord, long coreSeed = 0)
    {
        const int subrange = 100000;
        if (gridCoord.x < 0)
            gridCoord.x += subrange;
        if (gridCoord.y < 0)
            gridCoord.y += subrange;

        if (gridCoord.x < 0 || gridCoord.y < 0)
            return -1;

        long result = (gridCoord.x % subrange) * subrange + (gridCoord.y % subrange);
        return result;
    }
}
