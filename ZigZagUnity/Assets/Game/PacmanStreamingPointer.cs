using Events;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class PacmanStreamingPointer : BaseStreamingPointer, IHandle<BaseStreamingPointer.EventHit>
{
    public float Speed;
    public GameObject VehicleVisual;
    public GameObject TrailSegmentPrefab;
    public bool GenerateTrail;
    public GeneratorController GeneratorController;
    private Vector3 _direction;
    private bool _isDead;
    private Vector3 _lastSpawnPos;
    private int _score;
    private bool _isJump;
    private const float _jumpPos = -3f;
    private const float _landPos = 0f;
    private Rigidbody _rigidbody;
    private Quaternion _origQuaternion;

    void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
        _rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigidbody);
        _origQuaternion = VehicleVisual.transform.rotation;
    }

    void FixedUpdate()
    {
        if (_isDead)
            return;

        // Move to current direction
        //transform.Translate(_direction * Time.deltaTime * Speed);
        //transform.position = new Vector3(transform.position.x, transform.position.y, _isJump ? _jumpPos : _landPos);
        _rigidbody.AddForce(_direction * Speed * Time.fixedDeltaTime, ForceMode.Impulse);


        // Spawn segments
        if (GenerateTrail && _lastSpawnPos.DistanceTo(transform.position) > 0.8f)
        {
            var segment = Instantiate(TrailSegmentPrefab, transform.position, Quaternion.identity);
            var chunk = GeneratorController.GetActiveChunk(GetGridCoordinate());
            segment.transform.localScale = Vector3.one;
            segment.transform.SetParent(chunk.GameObject.transform);
            _lastSpawnPos = transform.position;
        }

        // Process player input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isJump = !_isJump;
            print(_isJump);
            transform.position = new Vector3(transform.position.x, transform.position.y, _isJump ? _jumpPos : _landPos);
        }

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

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 200), $"Score: {_score}"))
            SceneManager.LoadScene(0);
    }

    public void Handle(EventHit message)
    {
        var pickable = message.GameObject.GetComponent<Pickable>();
        if (pickable != null)
        {
            pickable.StartDisappear();
            _score++;
            gameObject.GetComponent<Energy>().AddValue(100);
            return;
        }

        var building = message.GameObject.GetComponent<Building>();
        if (building != null)
        {
            Die();
            return;
        }

        var seg = message.GameObject.GetComponent<TrailSegment>();
        if (seg != null)
        {
            Die();
            return;
        }
    }

    public void Die()
    {
        _isDead = true;
    }
}

