using System.Collections.Generic;
using BSPTreeRect;
using GameLib.Random;
using UnityEngine;

public class ZoneGenerator : MonoBehaviour
{
    public Range MinRectHeight;
    public Range MinRectWidth;

    public List<Rect> Zones { get; private set; }
    public Rect Bounds { get; private set; }

    private BspTree _tree;

    // Generate zones based on seed passed. Special cases:
    // seed == -1 generate empty list
    public List<Rect> Generate(Vector2 chunkSize, Vector2 pos, long seed)
    {
        if (seed == -1)
            return new List<Rect>();

        Bounds = new Rect(pos, chunkSize);
        var rootNode = new BspTree.Node { Rect = Bounds };

        var treeGeneratorParams = new BspTreeHelper.BspTreeGeneratorParams { MinNodeHeight = MinRectHeight, MinNodeWidth = MinRectWidth };
        _tree = BspTreeHelper.GenerateBspTree(rootNode, treeGeneratorParams, seed);
        Zones = _tree.GetTopNodes();
        return Zones;
    }
}