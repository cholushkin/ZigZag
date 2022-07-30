
using Events;
using GameLib;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacmanStreamingPointer : BaseStreamingPointer, IHandle<BaseStreamingPointer.EventHit>
{
    public float Speed;
    public GameObject TrailSegmentPrefab;
    public GeneratorController GeneratorController;
    private Vector3 _direction;
    private bool _isDead;
    private Vector3 _lastSpawnPos;
    private int _score;

    void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    void Update()
    {
        if(_isDead)
            return;
        transform.Translate(_direction * Time.deltaTime * Speed);

        if (_lastSpawnPos.DistanceTo(transform.position) > 0.8f)
        {
            var segment = Instantiate(TrailSegmentPrefab, transform.position, Quaternion.identity);
            var chunk = GeneratorController.GetActiveChunk(GetGridCoordinate());
            segment.transform.SetParent(chunk.GameObject.transform);
            _lastSpawnPos = transform.position;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
            _direction = Vector3.left;
        else if (Input.GetKey(KeyCode.RightArrow))
            _direction = Vector3.right;
        else if (Input.GetKey(KeyCode.UpArrow))
            _direction = Vector3.up;
        else if (Input.GetKey(KeyCode.DownArrow))
            _direction = Vector3.down;
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
            building.gameObject.AddComponent<Push>().PunchDirection = _direction;
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

