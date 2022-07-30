using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrailSegment : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(Vector3.one * 8, 3).OnComplete(()=>gameObject.GetComponent<SphereCollider>().enabled = true);
    }
}
