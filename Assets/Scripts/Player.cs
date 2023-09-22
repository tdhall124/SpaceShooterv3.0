using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PowerupHelper _powerupHelper;

    // LASER
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private GameObject _laserPrefab;
    private Vector3 _laserOffset;
    [SerializeField] private AudioSource _audioSourceLaser;
    private int _ammoCount;
    private int _maxAmmo;

    // TRIPLE SHOT
    [SerializeField] private GameObject _tripleShotPrefab;

    // MAGIC FLAME BOMB
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private AudioSource _audioSourceBomb;

    // HOMING MISSILE
    [SerializeField] private GameObject _homingMissile;
    [SerializeField] private AudioSource _audioSourceHoming;

    // UI
    [SerializeField] private int score = 0;

    [SerializeField] Camera _camera;
    private CameraShake _cameraShake;

    PlayerHealth _health;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        _powerupHelper = GetComponent<PowerupHelper>();

        if (_powerupHelper == null)
            Debug.LogError("The Powerup Helper is NULL.");

        UIManager.Instance.UpdateAmmoText(_ammoCount + " / " + _maxAmmo);

        _laserOffset = new Vector3(0, 1.05f, 0);

        if (_audioSourceLaser == null)
            Debug.LogError("Player::Start(): Laser Shot audio source is NULL.");
        if (_audioSourceBomb == null)
            Debug.LogError("Player::Start(): Magic Flame Bomb audio source is NULL.");
        if (_audioSourceHoming == null)
            Debug.LogError("Player::Start: Homing Missile audio source is NULL.");

        _maxAmmo = ResourceManager.Instance.MaxPlayerAmmo;
        _ammoCount = _maxAmmo;

        if (_camera == null)
            Debug.Log("Player::Start: Camera reference is NULL.");
        _cameraShake = _camera.GetComponent<CameraShake>();
        if (_cameraShake == null)
            Debug.Log("Player::Start: Camera Shake is NULL.");

        _health = GetComponent<PlayerHealth>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();
    }

    void FireLaser()
    {
        // change name to FireWeapon
        _canFire = Time.time + _fireRate; // this is here to reset the _canFire 

        if (_powerupHelper.IsTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position + _laserOffset, Quaternion.identity);
            _audioSourceLaser.Play();
        }
        else if (_powerupHelper.IsBombActive)
        {
            Instantiate(_bombPrefab, transform.position + _laserOffset, Quaternion.identity);
            _audioSourceBomb.Play();
        }
        else if (_powerupHelper.IsHomingMissileActive)
        {
            Instantiate(_homingMissile, transform.position + _laserOffset, Quaternion.identity);
            _audioSourceHoming.Play();
        }
        else if (_ammoCount > 0)
        {
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
            _audioSourceLaser.Play();
            _ammoCount--;
            UIManager.Instance.UpdateAmmoText(_ammoCount + " / " + _maxAmmo);
        }
    }

    public void Damage()
    {
        if (_powerupHelper.IsShieldActive == true)
        {
            ProcessShieldHit();
            return;
        }

        StartCoroutine(_cameraShake.Shake(.25f, .4f));

        _health.ProcessDamage();
    }

    void ProcessShieldHit()
    {
        if (_powerupHelper.ShieldHitCount < _powerupHelper.MaxShieldHits)
        {
            _powerupHelper.UpdateShieldVisual(_powerupHelper.ShieldHitCount, false);
            _powerupHelper.ShieldHitCount += 1;
            if (_powerupHelper.ShieldHitCount < _powerupHelper.MaxShieldHits)
                _powerupHelper.UpdateShieldVisual(_powerupHelper.ShieldHitCount, true);
        }
        if (_powerupHelper.ShieldHitCount == _powerupHelper.MaxShieldHits)
        {
            _powerupHelper.ShieldHitCount = 0;
            _powerupHelper.IsShieldActive = false;
        }
    }

    public void UpdateScore(int points)
    {
        score += points;
        UIManager.Instance.UpdateScoreText(score);
    }

    public void AmmoReloadActive()
    {
        _ammoCount = ResourceManager.Instance.MaxPlayerAmmo;

        UIManager.Instance.UpdateAmmoText(_ammoCount + " / " + _maxAmmo);
        UIManager.Instance.UpdateActiveText("Ammo");

        ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.Ammo);
    }

    public void DeceptronActive()
    {
        // take half your ammo
        _ammoCount = (int) Mathf.Floor(_ammoCount / 2);
        UIManager.Instance.UpdateAmmoText(_ammoCount + " / " + _maxAmmo);
        ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.Deceptron);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}
