using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    private bool _isEnemyBomb = false;
    public bool IsEnemyBomb
    {
        get { return _isEnemyBomb; }
        set { _isEnemyBomb = value; }
    }

    private GameObject[] _allEnemyObjects;
    private GameObject _nearestObject;

    readonly float _rotationSpeed = 5f;

    private void Start()
    {
        LocateNearestObject();
    }

    void Update()
    {
        if (_isEnemyBomb == false)
        {
            if (_nearestObject != null)
            {
                Vector3 directionToTarget = _nearestObject.transform.position - transform.position; 
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                            Quaternion.LookRotation(directionToTarget),  
                                        Time.deltaTime * _rotationSpeed);
                transform.Translate(_speed * Time.deltaTime * directionToTarget);
            }
            else
            {
                MoveUp();
                LocateNearestObject();
            }
        }
        else
        {
            MoveDown();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyBomb == true)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player?.Damage();
            }
        }
    }

    private void LocateNearestObject()
    {
        _allEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        float distance;
        float nearestDistance = 25f;

        for (int i = 0; i < _allEnemyObjects.Length; i++)
        {
            distance = Vector3.Distance(this.transform.position, _allEnemyObjects[i].transform.position);

            if (distance < nearestDistance)
            {
                _nearestObject = _allEnemyObjects[i];
                nearestDistance = distance;
            }
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 7.0f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            else
                Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        // MoveDown is for the enemy's laser moving downward (-Y)
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7.0f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            else
                Destroy(this.gameObject);
        }
    }
}
