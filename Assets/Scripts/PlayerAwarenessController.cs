using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public  Vector2 DirectionToPlayer { get; private set; }

    [SerializeField] private float _playerAwarenessDistance;

    private Transform _player;

    void Awake()
    {
        _player = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        if (_player != null)
        {
            Vector2 enemyToPlayerVector = _player.position - transform.position;

            // "With a direction, we don't need the magnitude of the vector.
            // We just want the direction on its own. We can get this by 
            // normalizing the vector. This retains the direction, but sets
            // the magnitude to one." (Internet)
            DirectionToPlayer = enemyToPlayerVector.normalized;

            // the magnitude of the vector will give us the distance to the player
            if (enemyToPlayerVector.magnitude > _playerAwarenessDistance)
            {
                AwareOfPlayer = true;
            }
            else
                AwareOfPlayer = false;
        }
    }
}
