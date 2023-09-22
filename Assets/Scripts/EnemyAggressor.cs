using UnityEngine;

public class EnemyAggressor : Enemy
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        MoveAggressively();
    }
    private void MoveAggressively()
    {
        // Aggressively move towards the player
        if (_player != null)
        {
            Vector3 directionToPlayer = _player.transform.position - transform.position;
            if (directionToPlayer.magnitude < 3f)
            {
                transform.Translate(_speed * Time.deltaTime * directionToPlayer);
            }
        }
    }
}
