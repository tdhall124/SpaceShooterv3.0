using System.Collections;
using UnityEngine;

public class EnemyBoss : Enemy
{
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private AudioSource _audioSourceBomb;

    protected override void Start()
    {
        base.Start();
        StartBombing();
    }

    void StartBombing()
    {
        // oscillate between shooting laser and shotting fire bomb
        StartCoroutine(FireBombRoutine());
        StartCoroutine(TurnAndShoot());
    }
    
    IEnumerator FireBombRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        while (true)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            GameObject enemyBomb = Instantiate(_bombPrefab, transform.position + _laserOffset, Quaternion.identity);

            FlameBomb flameBomb = enemyBomb.GetComponent<FlameBomb>();
            flameBomb.IsEnemyBomb = true;

            _audioSourceBomb.Play();

            yield return new WaitForSeconds(Random.Range(5, 10));
        }
    }

    IEnumerator TurnAndShoot()
    {
        yield return new WaitForSeconds(2.0f);
        float _rotationSpeed = 5f;
        while (true)
        {
            if (_player != null)
            {
                Vector3 directionToTarget = _player.transform.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                            Quaternion.LookRotation(directionToTarget),
                                        Time.deltaTime * _rotationSpeed);
                transform.Translate(_speed * Time.deltaTime * directionToTarget);
            }
            yield return new WaitForSeconds(3.0f);
        }
    }
}
