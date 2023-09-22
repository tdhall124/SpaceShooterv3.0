using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int _numEnemiesPerRound = 12;
    public int NumberOfEnemies
    { get { return _numEnemiesPerRound; } set { _numEnemiesPerRound = value; } }

    [SerializeField] private float _enemySpeed = 3.5f;
    public float EnemySpeed
    { get { return _enemySpeed; } set { _enemySpeed = value; } }

    [SerializeField] protected int _laserHitPoints = 10;
    public int LaserHitPoints
    { get { return _laserHitPoints; } set { _laserHitPoints = value; } }

    [SerializeField] protected int _bombHitPoints = 20;
    public int BombHitPoints
    { get { return _bombHitPoints; } set { _bombHitPoints = value; } }

    [SerializeField] protected int _homingHitPoints = 30;
    public int HomingHitPoints
    { get { return _homingHitPoints; } set { _homingHitPoints = value; } }

    [SerializeField] protected int _maxPlayerAmmo = 15;

    [SerializeField] protected int _maxPlayerLives = 3;
    public int MaxPlayerLives
    { get { return _maxPlayerLives; } set { _maxPlayerLives = value; } }

    [SerializeField] protected int _maxEnemyLives = 1;
    public int MaxEnemyLives { get { return _maxEnemyLives; } set { _maxEnemyLives = value; } }

    // MOVEMENT
    [SerializeField] private float _speedBoost = 2f;
    public float SpeedBoost
    { get { return _speedBoost; } set { _speedBoost = value; } }

    [SerializeField] private float _thrusterBoost = 1.5f;
    public float ThrusterBoost
    { get { return _thrusterBoost; } set { _thrusterBoost = value; } }

    public int MaxPlayerAmmo
    { get { return _maxPlayerAmmo; } set { _maxPlayerAmmo = value; } }

    [Header("--- Balanced Spawning of Powerups Based on Number of Enemies ---", order = 1)]
    [Space(-10, order = 2)]
    [Header("These values express spawn maximums as the ratio of powerups", order = 3)]
    [Space(-10, order = 4)]
    [Header("to be spawned, to the number of enemies.", order = 5)]
    [Space(-10, order = 6)]
    [Header("E.g. Ceiling of 25 enemies * .15f = 4 health powerups spawned.", order = 7)]
    [Space(10, order = 8)]

    [SerializeField][Tooltip("Health Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _healthSpawn = .15f;

    [SerializeField][Tooltip("Ammo Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _ammoSpawn = .5f;

    [SerializeField][Tooltip("Shield Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _shieldSpawn = .25f;

    [SerializeField][Tooltip("TripleShot Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _tripleShotSpawn = .5f;

    [SerializeField][Tooltip("Speed Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _speedSpawn = .5f;

    [SerializeField][Tooltip("Bomb Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _bombSpawn = .1f;

    [SerializeField][Tooltip("Deceptron Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _deceptronSpawn = .2f;

    [SerializeField][Tooltip("Fuel Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _fuelSpawn = .1f;

    [SerializeField][Tooltip("Homing Spawn Rate")]
    [Range(0.0f, 1.0f)] private float _homingSpawn = .2f;

    [SerializeField][Tooltip("Use the Scriptable Objects implementation.")]
    private bool _useScriptableObjects;

    Dictionary<PowerUps, ResourceEntry> _dictionary;

    private int _enemiesSpawned = 0;
    private int _enemiesDestroyed = 0;

    [SerializeField] private int _currentEnemyWaveSize = 1;
    public int CurrentEnemyWaveSize
    {
        get { return _currentEnemyWaveSize; }
        set { _currentEnemyWaveSize = value; }
    }

    private class ResourceEntry
    {
        public int consumed = 0;
        public int maximum = 0;
        public ResourceEntry(int maxAllowed)
        {
            maximum = maxAllowed;
        }
    }

    public enum PowerUps
    {
        Health,
        Ammo,
        Shield,
        TripleShot,
        Speed,
        Bomb,
        Deceptron,
        Fuel,
        Homing
    };

    private int _currentRound;
    private int _maxRounds;
    GameManager gm;

    public static ResourceManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    void Start()
    {
        InitializePowerupResources();
        gm = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _currentRound = gm != null ? gm.CurrentRound : 1;
        _maxRounds = gm != null ? gm.MaxRounds : 1;
    }

    public bool CanSpawn(int index)
    {
        //PowerUps e = PowerUps.Health;
        //int indexx = Array.IndexOf(Enum.GetValues(e.GetType()), e);
        // index is 0
        //PowerUps f = (PowerUps)(Enum.GetValues(e.GetType())).GetValue(index);
        // f is  PowerUps.Health

        PowerUps p = (PowerUps)index;
        if (_dictionary[p].consumed < _dictionary[p].maximum)
            return true;

        return false;
    }

    public void UpdatePowerupStats(PowerUps p)
    {
        if (_dictionary[p].consumed < _dictionary[p].maximum)
            _dictionary[p].consumed++;

        ResourceEntry re = _dictionary[p];
        string ratioText = re.consumed.ToString() + "/" + re.maximum.ToString();
        UIManager.Instance.UpdatePowerupStats(p, ratioText);
    }

    public void ResetPowerupStats()
    {
        InitializePowerupResources();
    }

    public void UpdateEnemiesSpawned()
    {
        if (_enemiesSpawned < _numEnemiesPerRound)
        {
            _enemiesSpawned++;
        }
        if (_enemiesSpawned == _numEnemiesPerRound)
            SpawnManager.Instance.StopSpawningEnemies();

        UIManager.Instance.UpdateEnemiesSpawnedText(_enemiesSpawned.ToString() + "/" + _numEnemiesPerRound.ToString());
    }

    public void UpdateEnemiesDestroyed()
    {
        if (_enemiesDestroyed < _numEnemiesPerRound)
        {
            _enemiesDestroyed++;
        }

        UIManager.Instance.UpdateEnemiesDestroyedText(_enemiesDestroyed.ToString() + "/" + _numEnemiesPerRound .ToString());

        if (_enemiesDestroyed == _numEnemiesPerRound)
        {
            SpawnManager.Instance.EndCurrentRound();
            int currentRound = gm != null ? gm.CurrentRound : 1;
            if (currentRound == _maxRounds)
            {
                gm.GameOver();
            }
            else
            {
                
                UIManager.Instance.StartNewRoundSequence();
            }
        }
    }

    void InitializePowerupResources()
    {
        // THIS COULD BE PUT INTO A Stats.cs class instead

        // Based on number of enemies, take the ceiling to establish
        // the total number of spawn types to spawn during a given round.

        // health = enemies * .15
        // ammo = enemies * .50
        // shield = enemies * .25
        // triple shot = enemies * .50
        // speed = enemies * .50
        // bomb = enemies * .10
        // deceptron = enemies * .20
        // fuel = enemies * .10
        // homing = enemies * .20

        _dictionary = new Dictionary<PowerUps, ResourceEntry>
        {
            { PowerUps.Health, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _healthSpawn)) },

            { PowerUps.Ammo, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _ammoSpawn)) },

            { PowerUps.Shield, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _shieldSpawn)) },

            { PowerUps.TripleShot, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _tripleShotSpawn)) },

            { PowerUps.Speed, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _speedSpawn)) },

            { PowerUps.Bomb, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _bombSpawn)) },

            { PowerUps.Deceptron, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _deceptronSpawn)) },

            { PowerUps.Fuel, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _fuelSpawn)) },

            { PowerUps.Homing, new ResourceEntry((int)Mathf.Ceil(_numEnemiesPerRound * _homingSpawn)) }
        };

        foreach (KeyValuePair<PowerUps, ResourceEntry> entry in _dictionary)
            UIManager.Instance.UpdatePowerupStats(entry.Key, entry.Value.consumed + "/" + entry.Value.maximum);

        UIManager.Instance.UpdateEnemiesSpawnedText(_enemiesSpawned.ToString() + "/" + _numEnemiesPerRound.ToString());
        UIManager.Instance.UpdateEnemiesDestroyedText(_enemiesDestroyed.ToString() + "/" + _numEnemiesPerRound.ToString());
    }

    public int CurrentRound()
    {
        return _currentRound;
    }
}
