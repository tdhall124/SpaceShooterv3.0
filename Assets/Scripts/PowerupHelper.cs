using System.Collections;
using UnityEngine;

public class PowerupHelper : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private int _thrusterLevelNormal = 0;
    private int _thrusterLevelMedium = 1;
    private int _thrusterLevelHigh = 2;

    // TRIPLE SHOT
    private bool _isTripleShotActive = false;
    public bool IsTripleShotActive
    { get { return _isTripleShotActive; } private set { } }

    // MAGIC FLAME BOMB
    private bool _isBombActive = false;
    public bool IsBombActive
    {
        get { return _isBombActive; }
        set { _isBombActive = value; }
    }

    // HOMING MISSILE
    private bool _isHomingMissileActive = false;
    public bool IsHomingMissileActive
    {
        get { return _isHomingMissileActive; }
        set { _isHomingMissileActive = value; }
    }
    
    // SHIELD
    [SerializeField] private GameObject[] _shieldVisualizer;
    private bool _isShieldActive = false;
    public bool IsShieldActive
    {
        get { return _isShieldActive; }
        set { _isShieldActive = value; }
    }
    private int _shieldHitCount = 0;
    public int ShieldHitCount
    {
        get { return _shieldHitCount; }
        set { _shieldHitCount = value; }
    }

    [SerializeField] private int _maxShieldHits = 0; // this should be in ResourceManager
    public int MaxShieldHits
    {
        get { return _maxShieldHits; }
        set { _maxShieldHits = value; }
    }

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();

        if (_shieldVisualizer == null)
            Debug.LogError("PowerupHelper::Start: The Shield Visualizer is NULL.");

        _maxShieldHits = _shieldVisualizer.Length;
    }

    public void UpdateShieldVisual(int shieldHitCount, bool active)
    {
        if(shieldHitCount < _maxShieldHits)
            _shieldVisualizer[shieldHitCount].SetActive(active);
    }

    public void TripleShotActive()
    {
        if (!_isTripleShotActive)
        {
            _isTripleShotActive = true;
            UIManager.Instance.UpdateActiveText("Triple Shot");

            ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.TripleShot);

            StartCoroutine(TripleShotPowerDownRoutine());
        }
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
        UIManager.Instance.UpdateActiveText("");
    }
    
    public void SpeedBoostActive()
    {
        if (!_playerMovement.IsSpeedBoostActive)
        {
            _playerMovement.SetBoost(ResourceManager.Instance.SpeedBoost);
            _playerMovement.IsSpeedBoostActive = true;

            ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.Speed);

            UIManager.Instance.UpdateActiveText("Speed");
            UIManager.Instance.UpdateThrusterLevel(_thrusterLevelHigh);

            StartCoroutine(SpeedBoostPowerDownRoutine());
        }
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _playerMovement.RelieveBoost(ResourceManager.Instance.SpeedBoost);
        
        UIManager.Instance.UpdateActiveText("");
        UIManager.Instance.UpdateThrusterLevel(_thrusterLevelNormal);
    }
    
    public void ShieldActive()
    {
        if (!_isShieldActive)
        {
            _isShieldActive = true;

            if(_shieldHitCount < _maxShieldHits)
                _shieldVisualizer[_shieldHitCount].SetActive(true);
            
            ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.Shield);
            UIManager.Instance.UpdateActiveText("Shield");
        }
    }

    public void BombActive()
    {
        if (!_isBombActive)
        {
            _isBombActive = true;
            UIManager.Instance.UpdateActiveText("Bomb");
            ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.Bomb);
            StartCoroutine(BombPowerDownRoutine());
        }
    }

    IEnumerator BombPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isBombActive = false;
        UIManager.Instance.UpdateActiveText("");
    }

    public void FuelActive()
    {
        Debug.Log("Fuel is active.");
    }

    public void HomingMissileActive()
    {
        if (!_isHomingMissileActive)
        {
            _isHomingMissileActive = true;
            UIManager.Instance.UpdateActiveText("Homing");
            ResourceManager.Instance.UpdatePowerupStats(ResourceManager.PowerUps.Homing);
            StartCoroutine(HomingMissilePowerDownRoutine());
        }
    }

    IEnumerator HomingMissilePowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isHomingMissileActive = false;
        UIManager.Instance.UpdateActiveText("");
    }
}
