using UnityEngine;

public class EnemyAvoider : Enemy
{
    [SerializeField] private float _circleCastRadius = 2.0f;
    [SerializeField] private float _dodgeDistance = 10f;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _circleCastRadius, Vector2.down);

        if (hit.collider != null)
        {
            if (hit.transform.gameObject.tag == "Laser")
            {
                Laser laser = hit.transform.GetComponent<Laser>();
                if (!laser.IsEnemyLaser)
                    transform.Translate(_speed * _dodgeDistance * Time.deltaTime * Vector3.right); // Avoid!!
            
            }
        }
    }
}
