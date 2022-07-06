using UnityEngine;

public class DecorationRandomizer : MonoBehaviour
{
    public Transform[] chunks;

    void Awake()
    {
        var scaleMode = Random.Range(0, 3);
        print(scaleMode);
        if (scaleMode == 0)
            transform.localScale *= 0.75f;
        else if (scaleMode == 1)
            transform.localScale *= 1.25f;


        foreach (var chunk in chunks)
        {
            chunk.gameObject.SetActive(Random.value > 0.3f);
        }
    }
}