using UnityEngine;

public class EnemyTopGun : Enemy
{
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private AudioSource _audioSourceBomb;

    protected override void FireLaser()
    {
        FireBomb();
    }

    void FireBomb()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        GameObject enemyBomb = Instantiate(_bombPrefab, transform.position + _laserOffset, Quaternion.identity);
        
        FlameBomb flameBomb = enemyBomb.GetComponent<FlameBomb>();
        flameBomb.IsEnemyBomb = true;

        _audioSourceBomb.Play();
    }
}
