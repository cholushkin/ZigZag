using System.Collections.Generic;
using GameLib;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject PrefabLandCube;
    [Range(0f, 1f)]
    public float SpawnTallBuildingProb;

    public float TallBuildingHeight;
    public float ShortBuildingHeight;


    public List<GameObject> Generate(List<Rect> rects)
    {
        var lands = new List<GameObject>();
        foreach (var rect in rects)
        {
            var obj = Instantiate(PrefabLandCube, rect.center, Quaternion.identity);
            obj.transform.localScale = rect.size.ToVector3(Random.value * (Random.value < SpawnTallBuildingProb ? TallBuildingHeight : ShortBuildingHeight)) * 0.6f;

            lands.Add(obj);
        }

        return lands;
    }
}
