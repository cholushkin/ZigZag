using Events;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class SimpleMoveStreamingPointer : BaseStreamingPointer, IHandle<BaseStreamingPointer.EventHit>
{
    public float Speed;
    public GameObject VehicleVisual;
    public GeneratorController GeneratorController;
    private Vector3 _direction;
    private Rigidbody _rigidbody;
    private Quaternion _origQuaternion;
    private float _nextChangeDirection;

    void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
        _rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigidbody);
        _origQuaternion = VehicleVisual.transform.rotation;


        _direction = Vector3.up;
        VehicleVisual.transform.rotation = _origQuaternion;
    }

    void Update()
    {
        _nextChangeDirection -= Time.deltaTime;
        if (_nextChangeDirection < 0f)
        {
            _nextChangeDirection = Random.Range(5, 16f);
        }
    }

    void FixedUpdate()
    {

        // Move to current direction
        //transform.Translate(_direction * Time.deltaTime * Speed);
        //transform.position = new Vector3(transform.position.x, transform.position.y, _isJump ? _jumpPos : _landPos);
        _rigidbody.AddForce(_direction * Speed * Time.fixedDeltaTime, ForceMode.Impulse);


        // Process player input
        var isChangedDirectionThisFrame = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _direction = Vector3.left;
            isChangedDirectionThisFrame = true;
            VehicleVisual.transform.rotation = _origQuaternion * Quaternion.AngleAxis(-90, Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _direction = Vector3.right;
            isChangedDirectionThisFrame = true;
            VehicleVisual.transform.rotation = _origQuaternion * Quaternion.AngleAxis(90, Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            _direction = Vector3.up;
            isChangedDirectionThisFrame = true;
            VehicleVisual.transform.rotation = _origQuaternion;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _direction = Vector3.down;
            isChangedDirectionThisFrame = true;
            VehicleVisual.transform.rotation = _origQuaternion * Quaternion.AngleAxis(180, Vector3.forward);
        }

        if (isChangedDirectionThisFrame)
        {
            _rigidbody.velocity = _direction * _rigidbody.velocity.magnitude;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void Handle(EventHit message)
    {
       
    }
}

