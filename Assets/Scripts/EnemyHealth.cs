using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    // DAMAGE
    [SerializeField] private GameObject _leftEngineFire;
    [SerializeField] private GameObject _rightEngineFire;
    [SerializeField] private AudioClip _audioClipExplosion;

    protected Animator _animExplosion;
    //[SerializeField] protected AudioSource _audioSourceExplosion;

    // LIVES
    private int _lives = 1;
    private int _maxLives = 1;

    //[SerializeField] UnityEvent OnPlayerDeath;

    void Start()
    {
        _animExplosion = GetComponent<Animator>();
        if (_animExplosion == null)
            Debug.LogError("EnemyHealth::Start: Animatin Explosion is NULL.");

        _maxLives = ResourceManager.Instance.MaxEnemyLives;
    }
    public void ProcessDamage()
    {
        // these hardcoded values 0, 2, 1 are unfortunate
        if (_lives > 0)
            _lives--; // avoiding array out-of-bounds exception

        if (_lives == 2)
        {
            _rightEngineFire.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngineFire.SetActive(true);
        }

        if (_lives == 0)
        {
            AudioSource.PlayClipAtPoint(_audioClipExplosion, transform.position);

            Destroy(GetComponent<Collider2D>());

            _animExplosion.SetTrigger("OnEnemyDeath");

            ResourceManager.Instance.UpdateEnemiesDestroyed();

            // this is handled in Health now
            Destroy(this.gameObject, 1f);
        }
    }
}
