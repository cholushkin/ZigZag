using System.Collections.Generic;
using GameLib;
using GameLib.Random;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject PrefabLandCube;
    [Range(0f, 1f)]
    public float SpawnTallBuildingProb;

    public float TallBuildingHeight;
    public float ShortBuildingHeight;
    private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();


    public List<GameObject> Generate(List<Rect> rects, long seed)
    {
        if (seed == -1)
            return new List<GameObject>();
        var lands = new List<GameObject>();
        _rnd.SetState(new LinearConRng.State(seed));

        foreach (var rect in rects)
        {
            var obj = Instantiate(PrefabLandCube, rect.center, Quaternion.identity);
            var z = _rnd.ValueFloat() * (_rnd.ValueFloat() < SpawnTallBuildingProb ? TallBuildingHeight : ShortBuildingHeight);
            if (z < 0.1f)
                z = 0.1f;
            obj.transform.localScale = rect.size.ToVector3(z) * 0.6f;
            lands.Add(obj);
        }
        
        return lands;
    }
}
