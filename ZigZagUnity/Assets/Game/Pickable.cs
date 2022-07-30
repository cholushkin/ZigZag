using DG.Tweening;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public Vector3 NextPos;
    public void StartDisappear()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutCubic).OnComplete(() => Destroy(gameObject));
    }
}
