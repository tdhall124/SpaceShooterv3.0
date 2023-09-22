using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    private bool _isEnemyLaser = false;
    public bool IsEnemyLaser
    {
        get { return _isEnemyLaser; }
        set { _isEnemyLaser = value; }
    }
    private bool _isBackwardsLaser = false;
    public bool IsBackwardsLaser
    {
        get { return _isBackwardsLaser; }
        set { _isBackwardsLaser = value; }
    }

    void Update()
    {
        if (_isEnemyLaser == false || _isBackwardsLaser == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();
            player?.Damage();
            Destroy(this.gameObject);
        }
        else if (other.tag == "Powerup" && _isEnemyLaser == true)
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    void MoveUp()
    {
        // MoveUp is for the Player's laser moving upward (+Y)
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
