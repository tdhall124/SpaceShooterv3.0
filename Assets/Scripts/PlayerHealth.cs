using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    // DAMAGE
    [SerializeField] private GameObject _leftEngineFire;
    [SerializeField] private GameObject _rightEngineFire;
    [SerializeField] private AudioClip _audioClipExplosion;

    // LIVES
    private int _lives = 3;
    private int _maxLives;

    [SerializeField] UnityEvent OnPlayerDeath;

    void Start()
    {
        _maxLives = ResourceManager.Instance.MaxPlayerLives;
    }
    public void ProcessDamage()
    {
        // these hardcoded values 0, 2, 1 are unfortunate
        if (_lives != 0)
            _lives--; // avoiding array out-of-bounds exception

        if (_lives < 1)
        {
            OnPlayerDeath.Invoke();
            AudioSource.PlayClipAtPoint(_audioClipExplosion, transform.position);
            Destroy(this.gameObject, 2f);
        }
        else
        {
            if (_lives == 2)
            {
                _rightEngineFire.SetActive(true);
            }
            else if (_lives == 1)
            {
                _leftEngineFire.SetActive(true);
            }
            UIManager.Instance.UpdateCurrentLives(_lives);
        }
    }

    public void HealthBoost()
    {
        if (_lives < _maxLives)
        {
            _lives++;
        
            UIManager.Instance.UpdateCurrentLives(_lives);
            UIManager.Instance.UpdateActiveText("Health");
            ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.Health);

            if (_leftEngineFire.activeSelf)
            {
                _leftEngineFire.SetActive(false);
            }
            else if (_rightEngineFire.activeSelf)
            {
                _rightEngineFire.SetActive(false);
            }
        }
    }
}
