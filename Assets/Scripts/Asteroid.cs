using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 20.0f;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private SpawnManager _spawnManager;

    private void Start()
    {
        if (_explosionPrefab == null)
            Debug.LogError("Asteroid::Start: The Explosion prefab is NULL.");

        // The Asteroid tells the Spawn_Manager to start spawning
        // and then the game is underway
        if (_spawnManager == null)
            Debug.LogError("Asteroid::Start: The Spawn_Manager is NULL.");
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.25f);
        }
    }
}
