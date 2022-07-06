using UnityEngine;

public class BaseStreamingPointer : MonoBehaviour, GeneratorController.IStreamingPointer
{
    public Vector2 GridCellSize;
    public float Speed;

    public Vector2Int GetGridCoordinate()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x / GridCellSize.x), Mathf.FloorToInt(transform.position.y / GridCellSize.y));
    }

    public Vector2 GetGridCellSize()
    {
        return GridCellSize;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
            transform.Translate(Vector3.left*Time.deltaTime* Speed);
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Translate(Vector3.right * Time.deltaTime * Speed);
        if (Input.GetKey(KeyCode.UpArrow))
            transform.Translate(Vector3.up * Time.deltaTime * Speed);
        if (Input.GetKey(KeyCode.DownArrow))
            transform.Translate(Vector3.down * Time.deltaTime * Speed);
    }
}
