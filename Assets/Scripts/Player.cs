using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // MOVEMENT
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _speedBoost = 2f;
    [SerializeField] private float _thrusterBoost = 1.5f;
    private bool _isSpeedBoostActive = false;
    private int _thrusterLevelNormal = 0;
    private int _thrusterLevelMedium = 1;
    private int _thrusterLevelHigh = 2;

    // LASER
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private GameObject _laserPrefab;
    private Vector3 _laserOffset;
    [SerializeField] private AudioSource _audioSourceLaser;
    private int _ammoCount = 15;

    [SerializeField] private int _lives = 3;
    private int _maxLives;

    private SpawnManager _spawnManager;

    // TRIPLE SHOT
    [SerializeField] private GameObject _tripleShotPrefab;
    private bool _isTripleShotActive = false;

    // MAGIC FLAME BOMB
    [SerializeField] private GameObject _magicFlameBomb;
    private bool _isMagicFlameBombActive = false;
    [SerializeField] private AudioSource _audioSourceBomb;

    // SHIELD
    private bool _isShieldActive = false;
    [SerializeField] private GameObject[] _shieldVisualizer;
    private int _shieldHitCount = 0;

    // UI
    [SerializeField] private int score = 0;
    private UIManager _uiManager;

    // DAMAGE
    [SerializeField] private GameObject _leftEngineFire;
    [SerializeField] private GameObject _rightEngineFire;
    [SerializeField] private AudioClip _audioClipExplosion;

    [SerializeField] private CameraShake _cameraShake;
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null) 
            Debug.LogError("Player::Start(): The SpawnManager is NULL.");

        if (_shieldVisualizer == null) 
            Debug.LogError("Player::Start(): The Shield Visualizer is NULL.");

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null) 
            Debug.LogError("Player::Start(): UI_Manager is NULL.");

        _laserOffset = new Vector3(0, 1.05f, 0);

        if (_leftEngineFire == null)
            Debug.LogError("Player::Start(): The Left_Engine_Fire is NULL.");
        if (_rightEngineFire == null)
            Debug.LogError("Player::Start(): The Right_Engine_Fire is NULL.");

        if (_audioSourceLaser == null)
            Debug.LogError("Player::Start(): Laser Shot audio source is NULL.");
        if (_audioClipExplosion == null)
            Debug.LogError("Player::Start(): Explosion audio clip is NULL.");
        if (_audioSourceBomb == null)
            Debug.LogError("Player::Start(): Magic Flame Bomb audio source is NULL.");

        _maxLives = _lives;

        if (_cameraShake == null)
            Debug.LogError("Player:Start: Camera is NULL.");
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed *= _thrusterBoost;
            _uiManager.UpdateThrusterLevel(_thrusterLevelMedium);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed /= _thrusterBoost;
            _uiManager.UpdateThrusterLevel(_thrusterLevelNormal);
        }

        transform.Translate(direction * _speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.66f, 0), 0);

        if (transform.position.x > 9f)
        {
            transform.position = new Vector3(-9f, transform.position.y, 0);
        }
        else if (transform.position.x < -9f)
        {
            transform.position = new Vector3(9f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            _audioSourceLaser.Play();
        }
        else if (_isMagicFlameBombActive)
        {
            Instantiate(_magicFlameBomb, transform.position + _laserOffset, Quaternion.identity);
            _audioSourceBomb.Play();
        }
        else if(_ammoCount > 0)
        {
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
            _audioSourceLaser.Play();
            _ammoCount--;
            _uiManager.UpdateAmmoText(_ammoCount);
        }
        _audioSourceLaser.Play();
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            switch (_shieldHitCount)
            {
                case 0:
                case 1:
                    _shieldVisualizer[_shieldHitCount].SetActive(false);
                    _shieldHitCount++;
                    _shieldVisualizer[_shieldHitCount].SetActive(true);
                    break;
                case 2:
                    _shieldVisualizer[_shieldHitCount].SetActive(false);
                    _isShieldActive = false;
                    _shieldHitCount = 0;
                    break;
                default:
                    Debug.Log("Player::Damage(): Default shield hit count found.");
                    break;
            }
            return;
        }

        StartCoroutine(_cameraShake.Shake(.25f, .4f));

        if(_lives != 0) 
            _lives--; // avoiding array out-of-bounds exception
        
        if (_lives == 2)
        {
            _rightEngineFire.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngineFire.SetActive(true);
        }
        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            AudioSource.PlayClipAtPoint(_audioClipExplosion, transform.position);
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        if (!_isTripleShotActive)
        {
            _isTripleShotActive = true;
            _uiManager.UpdateActiveText("Triple Shot");
            StartCoroutine(TripleShotPowerDownRoutine());
        }
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
        _uiManager.UpdateActiveText("");
    }

    public void SpeedBoostActive()
    {
        if (!_isSpeedBoostActive)
        {
            _speed *= _speedBoost;
            _isSpeedBoostActive = true;
            _uiManager.UpdateActiveText("Speed");
            _uiManager.UpdateThrusterLevel(_thrusterLevelHigh);
            StartCoroutine(SpeedBoostPowerDownRoutine());
        }
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _speed /= _speedBoost;
        _isSpeedBoostActive = false;
        _uiManager.UpdateActiveText("");
        _uiManager.UpdateThrusterLevel(_thrusterLevelNormal);
    }

    public void ShieldActive()
    {
        if (!_isShieldActive)
        {
            _isShieldActive = true;
            _shieldVisualizer[_shieldHitCount].SetActive(true);
            _uiManager.UpdateActiveText("Shield");
        }
    }

    public void MagicFlameBombActive()
    {
        if (!_isMagicFlameBombActive)
        {
            _isMagicFlameBombActive = true;
            _uiManager.UpdateActiveText("Bomb");
            StartCoroutine(MagicFlameBombPowerDownRoutine());
        }
    }

    IEnumerator MagicFlameBombPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isMagicFlameBombActive = false;
        _uiManager.UpdateActiveText("");
    }

    public void UpdateScore(int points)
    {
        score += points;
        _uiManager.UpdateScoreText(score);
    }

    public void AmmoReload()
    {
        _ammoCount = 15;
        _uiManager.UpdateAmmoText(_ammoCount);
        _uiManager.UpdateActiveText("Ammo");
    }

    public void HealthBoost()
    {
        if (_lives < _maxLives)
        {
            _lives++;

            _uiManager.UpdateLives(_lives);
            _uiManager.UpdateActiveText("Health");

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

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}
