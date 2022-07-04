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


    public List<Rect> Generate(int width, int height, Vector3 pos)
    {
        Bounds = new Rect(
            new Vector2(pos.x, pos.y),
            new Vector2(width, height));

        var rootNode = new BspTree.Node { Rect = Bounds };

        var treeGeneratorParams = new BspTreeHelper.BspTreeGeneratorParams { MinNodeHeight = MinRectHeight, MinNodeWidth = MinRectWidth };
        _tree = BspTreeHelper.GenerateBspTree(rootNode, treeGeneratorParams);
        Zones = _tree.GetTopNodes();
        return Zones;
    }
}