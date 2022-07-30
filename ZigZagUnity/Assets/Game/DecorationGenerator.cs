using System.Collections;
using System.Collections.Generic;
using GameLib.Random;
using ResourcesHelper;
using UnityEngine;

public class DecorationGenerator : MonoBehaviour
{
    private static IPseudoRandomNumberGenerator Rnd = RandomHelper.CreateRandomNumberGenerator();
    public GroupHolder<GameObject> PrefabDecorations;

    public List<GameObject> Generate(int width, int height, Vector3 pos)
    {
        var result = new List<GameObject>();
        result.Add(Instantiate(PrefabDecorations.GetRandom(Rnd), pos, Quaternion.identity, transform));
        return result;
        // create dots
        //for (int i = 0; i < Progression.Instance.FieldWidth / 2; ++i)
        //    CreateDot(new Vector3(Random.Range(bounds.xMin, bounds.xMax), Random.Range(bounds.yMin, bounds.yMax), 4), transform);
    }

    private void CreateDot(Vector3 pos, Transform parent)
    {
        //var go = Instantiate(PrefabDecorationDot, pos, Quaternion.identity);
        //go.transform.localScale = Vector3.one * Random.Range(0.5f, 1f);
        //go.transform.parent = parent;
    }
}
