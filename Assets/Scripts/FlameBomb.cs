using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlameBomb : MonoBehaviour
{
    [SerializeField] private float _speed = 8f; // another parameter to be controlled by Stats
    private bool _isEnemyBomb = false;
    public bool IsEnemyBomb
    {
        get { return _isEnemyBomb; }
        set { _isEnemyBomb = value; }
    }

    void Update()
    {
        if (_isEnemyBomb == false)
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
        if (other.tag == "Player" && _isEnemyBomb == true)
        {
            Player player = other.GetComponent<Player>();
            player?.Damage();
            Destroy(this.gameObject);
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 7.0f)
        {
            // don't need parent check for the bomb -- it has no 
            // parent like laser
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
