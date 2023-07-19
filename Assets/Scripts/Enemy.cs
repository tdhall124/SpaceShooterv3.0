using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Player _player;

    private Animator _animExplosion;
    [SerializeField] private AudioSource _audioSourceExplosion;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private AudioSource _audioSourceLaser;
    private Vector3 _laserOffset;

    [SerializeField] private float _fireRate = 3.0f;
    private float _canFire = -1;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _laserOffset = new Vector3(0f, -.05f, 0f);
        if (_player == null)
            Debug.LogError("Enemy::Start(): Player is NULL.");
        _animExplosion = GetComponent<Animator>();
        if (_animExplosion == null)
            Debug.LogError("Enemy::Start(): Animator is NULL.");
        if (_audioSourceExplosion == null)
            Debug.LogError("Enemy::Start(): Audio Source Explosion clip is NULL.");
        if (_audioSourceLaser == null)
            Debug.LogError("Enemy::Start(): Audio Source Laser is NULL.");
    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
            FireLaser();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 7f, 0);
        }
    }

    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        _audioSourceLaser.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            _speed = 0;
            Destroy(GetComponent<Collider2D>());

            if (player != null)
                player.Damage();
            
            _animExplosion.SetTrigger("OnEnemyDeath");
            _audioSourceExplosion.Play();

            Destroy(this.gameObject, 1.5f);
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            _speed = 0;
            Destroy(GetComponent<Collider2D>());

            if (_player != null)
            {
                _player.UpdateScore(10);
            }
            _animExplosion.SetTrigger("OnEnemyDeath");
            _audioSourceExplosion.Play();
            
            Destroy(this.gameObject, 1.5f);
        }
        else if (other.tag == "MagicFlameBomb")
        {
            Destroy(other.gameObject);

            _speed = 0;
            Destroy(GetComponent<Collider2D>());

            if (_player != null)
            {
                _player.UpdateScore(20);
            }
            _animExplosion.SetTrigger("OnEnemyDeath");
            _audioSourceExplosion.Play();

            Destroy(this.gameObject, 1.5f);
        }
    }
}
