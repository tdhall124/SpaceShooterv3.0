using UnityEngine;

public class EnemyMovement : MonoBehaviour, IMovement
{
    // MOVEMENT
    [SerializeField] private float _speed = 3.5f;
    // will getter/setter work for _speed instead of the dedicated method call at the bottom

    SpawnManager.EnemyType _enemyType;

    private Vector3 _atAngle;

    void Start()
    {
        _atAngle = AtRandomAngle();
    }

    void Update()
    {
        CalculateMovement();
    }

    public void CalculateMovement()
    {
        transform.Translate(_speed * Time.deltaTime * _atAngle);

        if (transform.position.y < -7f)
        {
            transform.position = new Vector3(Random.Range(-9f, 9f), 7f, 0);
        }
    }

    Vector3 AtRandomAngle()
    {
        Vector3 atAngle;
        float xvalue = Random.Range(-0.93f, 0.93f);
        float yvalue = Random.Range(-0.6f, -0.8f);
        atAngle = new Vector3(xvalue, yvalue, 0);

        return atAngle;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetEnemyType(SpawnManager.EnemyType enemyType)
    {
        _enemyType = enemyType;
    }
}
