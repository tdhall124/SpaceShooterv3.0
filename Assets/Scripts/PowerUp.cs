using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;

    // 0 = TripleShot, 1 = Speed, 2 = Shield
    // 3 = Ammo, 4 = Health, 5 = Bomb, 6 = Deceptron, 7 = Fuel, 8 = Homing
    [SerializeField] private int powerupID; // this uses an ID system

    [SerializeField] private AudioClip _powerupClip;

    private Player _player;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.Log("PowerUp::Start: Player is NULL.");
    }

    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (transform.position.y < -6f)
            Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            Vector3 directionToPlayer = _player.transform.position - transform.position;
            if (directionToPlayer.magnitude < 4f)
            {
                if (Input.GetKey(KeyCode.C))
                    transform.Translate(_speed * Time.deltaTime * directionToPlayer);
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            PowerupHelper powerupHelper = other.gameObject.GetComponent<PowerupHelper>();

            AudioSource.PlayClipAtPoint(_powerupClip, transform.position);

            switch (powerupID)
            {
                case 0:
                    powerupHelper?.TripleShotActive();
                    break;
                case 1:
                    powerupHelper?.SpeedBoostActive();
                    break;
                case 2:
                    powerupHelper?.ShieldActive();
                    break;
                case 3:
                    player?.AmmoReloadActive();
                    break;
                case 4:
                    PlayerHealth health = other.gameObject.GetComponent<PlayerHealth>();
                    health?.HealthBoost();
                    break;
                case 5:
                    powerupHelper?.BombActive();
                    break;
                case 6:
                    player?.DeceptronActive();
                    break;
                case 7:
                    powerupHelper?.FuelActive();
                    break;
                case 8:
                    powerupHelper?.HomingMissileActive();
                    break;
                default:
                    Debug.Log("PowerUp::OnTriggerEnter(): Default powerup ID taken. No powerup found.");
                    break;
            }

            Destroy(this.gameObject, 0.2f);
        }
    }
}
