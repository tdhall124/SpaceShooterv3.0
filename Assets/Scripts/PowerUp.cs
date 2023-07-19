using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;

    // 0 = TripleShot, 1 = Speed, 2 = Shield
    // 3 = Ammo, 4 = Health, 5 = Bomb
    [SerializeField] private int powerupID;

    [SerializeField] private AudioClip _powerupClip;

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6f)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_powerupClip, transform.position);

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AmmoReload();
                        break;
                    case 4:
                        player.HealthBoost();
                        break;
                    case 5:
                        player.MagicFlameBombActive();
                        break;
                    default:
                        Debug.Log("PowerUp::OnTriggerEnter(): Default powerup ID taken. No powerup found.");
                        break;
                }
                
            }

            Destroy(this.gameObject, 2.0f);
        }
    }
}
