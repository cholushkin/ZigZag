using System;
using System.Collections.Generic;
using GameLib;
using TMPro;
using UnityEngine;

public class DistanceToNextTarget : MonoBehaviour
{
    public List<GameObject> AllTargets;
    public TextMeshProUGUI TextDistance;

    public void RegisterTarget(GameObject target)
    {
        AllTargets.Add(target);
    }

    void Update()
    {
        float min = Single.MaxValue;
        foreach (var trg in AllTargets)
        {
            if (trg != null)
            {
                float lmin = trg.transform.position.DistanceTo(transform.position);
                if (lmin < min)
                    min = lmin;
            }
        }

        if (min == Single.MaxValue)
            TextDistance.text = "";
        else
            TextDistance.text = min < 1f ? "" : $"{min:##.#}";
    }
}
