using System.Collections;
using System.Collections.Generic;
using GameLib;
using UnityEngine;

public class SnakeStreamingPointer : BaseStreamingPointer
{
    public GameObject SnakeSegmentPrefab;
    public float Speed;
    public float RotationSpeed;
    public float Acceleration;
    private Vector3 _lastSpawnPos;
    private bool _rotatingLeft;


    void Update()
    {
        if (_lastSpawnPos.DistanceTo(transform.position) > 0.5f)
        {
            Instantiate(SnakeSegmentPrefab, transform.position, Quaternion.identity);
            _lastSpawnPos = transform.position;
        }

        transform.Translate(Vector3.up * Time.deltaTime * Speed);


        var hasMove = false;


        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    hasMove = true;
        //    transform.Translate(Vector3.left * Time.deltaTime * Speed);
        //}

        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    hasMove = true;
        //    transform.Translate(Vector3.up* Time.deltaTime * Speed);
        //}

        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    hasMove = true;
        //    transform.Translate(Vector3.right * Time.deltaTime * Speed);
        //}

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    hasMove = true;
        //    transform.Translate(Vector3.down * Time.deltaTime * Speed);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rotatingLeft = !_rotatingLeft;
        }

        //    if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    Acceleration += Time.deltaTime;
        //    _rotatingLeft = true;
        //    //transform.Rotate(Vector3.forward * Time.deltaTime * RotationSpeed);
        //    hasMove = true;
        //}

        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    _rotatingLeft = false;
        //    //transform.Rotate(-Vector3.forward * Time.deltaTime * RotationSpeed);
        //    hasMove = true;
        //}

        transform.Rotate(_rotatingLeft ? Vector3.forward * Time.deltaTime * RotationSpeed : -Vector3.forward * Time.deltaTime * RotationSpeed);


        //if (hasMove)
        //    Time.timeScale = 1.0f;
        //else
        //{
        //    Time.timeScale = 0.0f;

        //}

        //if (Input.GetKey(KeyCode.LeftArrow))
        //    transform.Translate(Vector3.left*Time.deltaTime* Speed);
        //if (Input.GetKey(KeyCode.RightArrow))
        //    transform.Translate(Vector3.right * Time.deltaTime * Speed);
        //if (Input.GetKey(KeyCode.UpArrow))
        //    transform.Translate(Vector3.up * Time.deltaTime * Speed);
        //if (Input.GetKey(KeyCode.DownArrow))
        //    transform.Translate(Vector3.down * Time.deltaTime * Speed);
    }
}
