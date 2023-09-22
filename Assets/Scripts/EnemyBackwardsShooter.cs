using UnityEngine;

public class EnemyBackwardsShooter : Enemy
{
    private bool _hasFired = false;
    protected override void FixedUpdate()
    {
        // offset the raycast starting point so that it begins at the rear of the spacecraft
        Vector3 raycastPosition = new Vector3(transform.position.x, transform.position.y + 1.5f, 0);
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition , Vector2.up); // straight up
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                if (Time.time > _canFire)
                {
                    FireBackwardsLaser();
                }
                else if (!_hasFired)
                {
                    FireBackwardsLaser();
                    _hasFired = true;
                }
            }
        }
    }

    void FireBackwardsLaser()
    {
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);

        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].IsEnemyLaser = true;
            lasers[i].IsBackwardsLaser = true;
        }
        _audioSourceLaser.Play();
    }
}
