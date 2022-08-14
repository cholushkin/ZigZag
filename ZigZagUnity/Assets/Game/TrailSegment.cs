using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrailSegment : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(transform.localScale * 1.5f, 0.5f).OnComplete(()=>gameObject.GetComponent<SphereCollider>().enabled = true);
    }
}
