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
        public GameObject GameObject;
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
    public PickableGenerator PickableGenerator;
    public BuildingGenerator BuildingGenerator;

    private IPseudoRandomNumberGenerator _rnd;
    private Vector2Int _prevPos; // prev position of pointer to determine when streaming pointer changes
    private Dictionary<Vector2Int, Chunk> _activeChunks = new(32);
    readonly Vector2Int[] _directions = { new(1, 0), new(0, -1), new(-1, 0), new(0, 1) };
    

    void Awake()
    {
        _rnd = RandomHelper.CreateRandomNumberGenerator(CoreSeed);
        CoreSeed = _rnd.GetState().AsNumber();
        PickableGenerator?.Pregenerate();
        GenerateAround(StreamingPointer, 5);
    }

    void Update()
    {
        var gridCoord = StreamingPointer.GetGridCoordinate();

        if (gridCoord != _prevPos)
        {
            _prevPos = gridCoord;
            GenerateAround(StreamingPointer, 5);
        }
    }

    private void GenerateAround(IStreamingPointer pointer, int radius)
    {
        TryGenerateChunk(pointer, Vector2Int.zero);

        Vector2Int offset = Vector2Int.zero;
        for (int r = 1; r <= radius; ++r)
        {
            offset.Set(-r, r);
            for (int dirIndex = 0; dirIndex < 4; ++dirIndex)
            {
                var dir = _directions[dirIndex];
                for (int step = 0; step < r * 2; ++step)
                {
                    TryGenerateChunk(pointer, offset);
                    offset += dir;
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

    public Chunk GetActiveChunk(Vector2Int gridCoord)
    {
        Chunk chunk = null;
        _activeChunks.TryGetValue(gridCoord, out chunk);
        return chunk;
    }

    private Chunk TryGenerateChunk(IStreamingPointer pointer, Vector2Int gridOffset)
    {
        var gridCoordinate = pointer.GetGridCoordinate() + gridOffset;
        if (_activeChunks.ContainsKey(gridCoordinate))
        {
            _activeChunks[gridCoordinate].GenerationID = Time.frameCount;
            return null;
        }

        var absCoord = gridCoordinate * pointer.GetGridCellSize();
        var seed = GetSeedForCoord(gridCoordinate, CoreSeed); // seed for this grid coordinate

        // Generate rects for main layer
        var rects = ZoneGenerator.Generate(ChunkSize, absCoord, seed);

        // Generate rects for second layer
        _rnd.SetState(new LinearConRng.State(seed));
        if (ZoneGeneratorLayer2 != null && !PickableGenerator.DoNeedGeneratePickable(gridCoordinate) && _rnd.ValueFloat() < ChanceGenerateSecondLayer)
            rects.AddRange(ZoneGeneratorLayer2.Generate(ChunkSize, absCoord, seed));

        var gObj = new GameObject($"Chunk:{seed}:{gridCoordinate}");
        gObj.transform.position = absCoord;
        gObj.transform.SetParent(transform);

        // Generate pickable
        if(PickableGenerator.DoNeedGeneratePickable(gridCoordinate))
            PickableGenerator.Generate(rects, gObj.transform, seed);

        // Generate buildings
        var buildings = BuildingGenerator.Generate(rects, seed);
        foreach (var building in buildings)
        {
            building.transform.SetParent(gObj.transform);
        }

        // Randomly offset entire chunk
        if (_rnd.ValueFloat() < ChanceOffsetChunk)
        {
            // get random offset
            var offset = _rnd.FromArray(_directions);
            gObj.transform.Translate(new Vector3(offset.x, offset.y, 0) * 2); // distance enough to close the passage
        }

        var chunk = new Chunk
        {
            Rects = rects,
            //Decorations = DecorationGenerator.Generate(ChunkSize.x, ChunkSize.y, new Vector3(cellCoord.x, cellCoord.y, 0)),
            Buildings = buildings,
            GameObject = gObj
        };

        _activeChunks.Add(gridCoordinate, chunk);
        _activeChunks[gridCoordinate].GenerationID = Time.frameCount;
        return chunk;
    }

    private void DestroyChunk(Chunk chunk)
    {
        Destroy(chunk.GameObject);
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
        return gridCoord.GetHashCode() + coreSeed % subrange;
    }
}
