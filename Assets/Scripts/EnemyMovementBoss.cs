using UnityEngine;

public class EnemyMovementBoss : MonoBehaviour, IMovement
{
    // MOVEMENT
    [SerializeField] private float _speed = 3.5f;
    // will getter/setter work for _speed instead of the dedicated method call at the bottom

    bool _isFixed = false;

    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private float _rotationSpeed;

    GameObject _player;

    void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
            Debug.LogError("EnemyMovementBoss:Start:Player ref is NULL.");
    }

    void Update()
    {
        if (!_isFixed)
            CalculateMovement();
    }

    public void CalculateMovement()
    {
        Vector3 targetPosition = target.TransformPoint(new Vector3(0, 1, 0));

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        if(transform.position == targetPosition)
            _isFixed = true;
    }
    
    void FixedUpdate()
    {
        if (_player != null)
            transform.up = (_player.transform.position - transform.position) * -1;
    }
}
