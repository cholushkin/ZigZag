using System.Collections.Generic;
using System.Linq;
using GameLib.Random;
using UnityEngine;

public class PickableGenerator : MonoBehaviour
{
    public GameObject PrefabPickable;
    public DistanceToNextTarget DistanceToNextTarget;
    private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();

    private readonly Range[] _pickupsDistributions = { new(1, 3), new(4, 6) };
    private Vector2Int _pickupDistributionPointer = Vector2Int.zero;
    private int _pickupDistributionIteration;
    private HashSet<Vector2Int> _slots = new HashSet<Vector2Int>(64);

    public void Pregenerate()
    {
        _slots.Add(Vector2Int.zero);
        FillQueue();
    }

    public  void Generate(List<Rect> rects, Transform parent, long seed)
    {
        if (seed == -1)
            return;

        Debug.Log($"Generating{_pickupDistributionIteration} Pickable for {_pickupDistributionPointer}");

        _rnd.SetState(new LinearConRng.State(seed));

        var rect = _rnd.FromList(rects);
        rects.Remove(rect);

        var obj = Instantiate(PrefabPickable, rect.center, Quaternion.identity);
        DistanceToNextTarget.RegisterTarget(obj);

        obj.transform.SetParent(parent);
    }

    private void FillQueue()
    {
        while (_slots.Count < 64)
        {
            var next = _rnd.FromRangeIntInclusive(_pickupsDistributions[0]);
            if (_rnd.ValueFloat() < 0.4f)
                next = _rnd.FromRangeIntInclusive(_pickupsDistributions[1]);
            _pickupDistributionPointer += (_pickupDistributionIteration % 2 == 0 ? new Vector2Int(0, next) : new Vector2Int(next, 0));
            ++_pickupDistributionIteration;
            _slots.Add(_pickupDistributionPointer);
        }
    }

    public bool DoNeedGeneratePickable(Vector2Int gridCoordinate)
    {
        return _slots.Contains(gridCoordinate);
    }
}    