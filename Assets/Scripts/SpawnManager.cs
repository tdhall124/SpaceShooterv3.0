using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TDH;

public class SpawnManager : MonoBehaviour
{
    private GameObject[] _enemyPrefabs;
    
    public enum EnemyType
    {
        Enemy,
        EnemyTopGun,
        EnemyAggressor,
        EnemyAvoider,
        EnemyBackwardsShooter,
        EnemyBoss
    };

    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _powerupContainer;

    [SerializeField] private GameObject[] _powerups;

    [SerializeField] private ScenarioManifestSO _scenarioManifest;
    private ScenarioSO _scenarioSO;
    private ScenarioDetailsSO _scenarioDetails;
    private EnemyDetailsSO _enemyDetails;
    private int _currentRound = 1;

    private bool _stopSpawningEnemies = false;
    private bool _stopSpawningPowerups = false;

    [SerializeField] private bool _suppressEnemy;
    [SerializeField] private bool _suppressEnemyTopGun;
    [SerializeField] private bool _suppressEnemyAggressor;
    [SerializeField] private bool _suppressEnemyBackwardsShooter;
    [SerializeField] private bool _suppressEnemyAvoider;
    [SerializeField] private bool _suppressEnemyBoss;

    private bool _isBossSpawned = false;

    private int _currentEnemyWaveSize = 1;

    [SerializeField] UnityEvent OnEnemySpawned;

    public static SpawnManager Instance;

    [SerializeField] private bool _useScriptableObjects = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;       
        else
            Destroy(Instance);
    }

    private void Start()
    {
        GameManager gm = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _currentRound = gm.CurrentRound;

        _scenarioSO = _scenarioManifest.GetScenario(_currentRound);
        if (_scenarioSO != null)
        {
            _scenarioDetails = _scenarioSO.GetScenarioDetails();
        }
        else
            Debug.LogError("SpawnManager:Start:Scenario is NULL.");

        _enemyDetails = _scenarioManifest.GetEnemyDetails();
        if (_enemyDetails == null)
        {
            Debug.LogError("SpawnManager:Start:_enemyDetails is NULL.");
        }

        if (_useScriptableObjects)
        {
            ProcessScenarioDetails();
            ProcessEnemyDetails();
        }
    }

        void ProcessScenarioDetails()
    {
        SuppressAll();

        UIManager.Instance.UpdateRoundSloganText(_scenarioSO._roundSlogan);

        List<EnemyType> _enemies = _scenarioDetails._enemyTypes;
        foreach (EnemyType enemy in _enemies)
        {
            switch (enemy)
            {
                case EnemyType.Enemy:
                    _suppressEnemy = false; break;
                case EnemyType.EnemyTopGun:
                    _suppressEnemyTopGun = false; break;
                case EnemyType.EnemyAvoider:
                    _suppressEnemyAvoider = false; break;
                case EnemyType.EnemyAggressor:
                    _suppressEnemyAggressor = false; break;
                case EnemyType.EnemyBackwardsShooter:
                    _suppressEnemyBackwardsShooter = false; break;
                case EnemyType.EnemyBoss:
                    _suppressEnemyBoss = false; break;
                default:
                    break;
            }
        }
    }

    void ProcessEnemyDetails()
    {
        _enemyPrefabs = _enemyDetails.EnemyPrefabs;
        if(_enemyPrefabs == null )
            Debug.LogError("SpawnManager:ProcessEnemyDetails:_enemyPrefabs is NULL.");
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPrimaryPowerUpRoutine());
        StartCoroutine(SpawnSecondaryPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        int counter = 0;
        int waveCount = 0;

        _currentEnemyWaveSize = ResourceManager.Instance.CurrentEnemyWaveSize;

        while (_stopSpawningEnemies == false)
        {
            // this spawns at the top of the window 
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            
            if (counter == _enemyPrefabs.Length)
                counter = 0; // reset to start of array

            EnemyType enemyType = (EnemyType)counter;

            if(CheckSuppression(enemyType))
            {
                Debug.Log("SpawnManager:SpawnEnemy:enemyType = " + enemyType.ToString());
                
                if (waveCount < _currentEnemyWaveSize)
                {
                    posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                    GameObject nextEnemy = Instantiate(_enemyPrefabs[counter], posToSpawn, Quaternion.identity);
                    waveCount++;

                    if (nextEnemy != null)
                    {
                        Enemy enemy = nextEnemy.GetComponent<Enemy>();
                        if (!enemyType.Equals(EnemyType.EnemyBoss))
                        {
                            if (CoinToss())
                            {
                                enemy.IsShieldActive = true;
                            }
                        }
                        else
                        {
                            enemy.IsShieldActive = true;
                        }
                        nextEnemy.transform.parent = _enemyContainer.transform;
                        OnEnemySpawned?.Invoke();
                    }
                }
                else
                {
                    waveCount = -0;
                    yield return new WaitForSeconds(30f); // 2,0f seems to create time gaps (waiting)
                }
            }
            counter++;
        }
    }

    private bool CheckSuppression(EnemyType enemyType)
    {
        if (IsSuppressed(enemyType))
        {
            return false;
        }
        if (enemyType.ToString() == "EnemyBoss")
        {
            if (_isBossSpawned)
            {
                return false;
            }
            else
            {
                _isBossSpawned = true;
                return true;
            }
        }
        return true;
    }

    private bool IsSuppressed(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Enemy: if (_suppressEnemy) return true; break;
            case EnemyType.EnemyTopGun: if (_suppressEnemyTopGun) return true; break;
            case EnemyType.EnemyAvoider: if (_suppressEnemyAvoider) return true; break;
            case EnemyType.EnemyAggressor: if (_suppressEnemyAggressor) return true; break;
            case EnemyType.EnemyBackwardsShooter: if (_suppressEnemyBackwardsShooter) return true; break;
            case EnemyType.EnemyBoss: if (_suppressEnemyBoss) return true; break;
            default: return false;
        }
        return false;
    }

    void SuppressAll()
    {
        _suppressEnemy = true;
        _suppressEnemyTopGun = true;
        _suppressEnemyAggressor = true;
        _suppressEnemyBackwardsShooter = true;
        _suppressEnemyAvoider = true;
        _suppressEnemyBoss = true;
    }

    IEnumerator SpawnPrimaryPowerUpRoutine()
    {
        yield return new WaitForSeconds(_scenarioDetails._initialWaitPrimaryPowerup);
        
        while (_stopSpawningPowerups == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

            // 0 = TripleShot, 1 = Speed, 2 = Shield, 3 = Ammo
            int randomPowerUp = Random.Range(0, 4); 

            if (ResourceManager.Instance.CanSpawn(randomPowerUp))
            {
                GameObject powerup = Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
                powerup.transform.parent = _powerupContainer.transform;
                yield return new WaitForSeconds(_scenarioDetails._spawnWaitPrimaryPowerup);
            }
        }
    }

    IEnumerator SpawnSecondaryPowerUpRoutine()
    {
        yield return new WaitForSeconds(_scenarioDetails._initialWaitSecondaryPowerup);
        while (_stopSpawningPowerups == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomPowerUp = Random.Range(4, 9); // 4 = Health, 5 = Bomb, 6 = Deceptron, 7 = Fuel, 8 = Homing

            if (ResourceManager.Instance.CanSpawn(randomPowerUp))
            {
                GameObject powerup = Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
                powerup.transform.parent = _powerupContainer.transform;
                yield return new WaitForSeconds(_scenarioDetails._spawnWaitSecondaryPowerup);
            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerups = true;
    }

    public void RestartRound()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerups = true;
        ResourceManager.Instance.ResetPowerupStats();
        _enemyContainer.SetActive(false);
        _powerupContainer.SetActive(false);
    }

    public void EndCurrentRound()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerups = true;
        ResourceManager.Instance.ResetPowerupStats(); // ?? maybe not
        _enemyContainer.SetActive(false);
        _powerupContainer.SetActive(false);
    }

    public void StopSpawningEnemies()
    {
        _stopSpawningEnemies = true;
    }

    private bool CoinToss()
    {
        if (Random.Range(0, 10) >= 5)
            return true;
        else return false;
    }
}
