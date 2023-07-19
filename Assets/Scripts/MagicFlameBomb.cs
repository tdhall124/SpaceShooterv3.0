using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFlameBomb : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    
    void Update()
    {
        MoveUp();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 7.0f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            else
                Destroy(this.gameObject);
        }
    }
}
