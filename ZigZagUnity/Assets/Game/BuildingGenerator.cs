using System.Collections.Generic;
using GameLib;
using GameLib.Random;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject PrefabLandCube;
    public GameObject[] PrefabRoofs;
    [Range(0f, 1f)]
    public float SpawnTallBuildingProb;

    public float TallBuildingHeight;
    public float ShortBuildingHeight;
    private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();
    private float[] _angles = {-90f, -180f, -270f};


    public List<GameObject> Generate(List<Rect> rects, long seed)
    {
        if (seed == -1)
            return new List<GameObject>();
        var lands = new List<GameObject>();
        _rnd.SetState(new LinearConRng.State(seed));
        const float scale = 0.6f;

        foreach (var rect in rects)
        {
            var obj = Instantiate(PrefabLandCube, rect.center, Quaternion.identity);
            var z = _rnd.ValueFloat() * (_rnd.ValueFloat() < SpawnTallBuildingProb ? TallBuildingHeight : ShortBuildingHeight);
            if (z < 2.0f)
                z = 2.0f;
            obj.transform.localScale = rect.size.ToVector3(z) * scale;


            var roofPrefab = _rnd.FromArray(PrefabRoofs);
            var origRotation = roofPrefab.transform.rotation;
            var roof = Instantiate(roofPrefab, rect.center, origRotation);
            //roof.transform.rotation *= Quaternion.AngleAxis(_rnd.FromArray(_angles), Vector3.forward);
            roof.transform.position = roof.transform.position + Vector3.back * z * scale;
            roof.transform.localScale = rect.size.ToVector3(1f) * scale * roofPrefab.transform.localScale.x;
            roof.transform.SetParent(obj.transform);

            //roof.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y,
            //    roof.transform.localScale.z);



            lands.Add(obj);
        }

        return lands;
    }
}
