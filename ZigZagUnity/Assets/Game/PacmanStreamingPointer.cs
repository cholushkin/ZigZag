
using UnityEngine;

public class PacmanStreamingPointer : BaseStreamingPointer
{
    public float Speed;
    private Vector3 _direction;

    void Update()
    {
        transform.Translate(_direction * Time.deltaTime * Speed);
        if (Input.GetKey(KeyCode.LeftArrow))
            _direction = Vector3.left;
        else if (Input.GetKey(KeyCode.RightArrow))
            _direction = Vector3.right;
        else if (Input.GetKey(KeyCode.UpArrow))
            _direction = Vector3.up;
        else if (Input.GetKey(KeyCode.DownArrow))
            _direction = Vector3.down;
    }
}
