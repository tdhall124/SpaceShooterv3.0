using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    protected float _speed = 1f;

    protected Player _player;

    // EXPLOSION
    protected Animator _animExplosion;
    [SerializeField] protected AudioSource _audioSourceExplosion;

    // LASER
    [SerializeField] protected GameObject _laserPrefab;
    [SerializeField] protected AudioSource _audioSourceLaser;
    protected Vector3 _laserOffset;
    
    // FIRE RATE
    [SerializeField] protected float _fireRate = 3.0f;
    protected float _canFire = -1;

    // SHIELD
    protected bool _isShieldActive = false;
    public bool IsShieldActive
    {
        get { return _isShieldActive; }
        set { _isShieldActive = value; }
    }

    [SerializeField] protected GameObject[] _shieldVisualizer;
    private int _shieldHitCount = 0;
    private int _maxShieldHitCount;

    // HEALTH
    EnemyHealth _enemyHealth;

    protected virtual void Start()
    {
        GameObject player = GameObject.Find("Player");
        _player = player?.GetComponent<Player>();   

        _laserOffset = new Vector3(0f, -.2f, 0f);

        _animExplosion = GetComponent<Animator>();
        if (_animExplosion == null)
            Debug.LogError("Enemy::Start(): Animator is NULL.");

        if (_audioSourceLaser == null)
            Debug.LogError("Enemy::Start(): Audio Source Laser is NULL.");

        _maxShieldHitCount = _shieldVisualizer.Length;

        _enemyHealth = GetComponent<EnemyHealth>();
        if (_enemyHealth == null)
            Debug.Log("Enemy::Start: Enemy Health is NULL.");

        if(_isShieldActive)
            _shieldVisualizer[_shieldHitCount].SetActive(true);

        _speed = ResourceManager.Instance.EnemySpeed;

    }
    
    protected virtual void Update()
    {
        if (Time.time > _canFire)
            FireLaser();
    }

    protected virtual void FixedUpdate()
    {
        LookForPowerup();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            HandlePlayer(player);
        }
        else if (other.tag == "Laser")
        {
            if (!other.gameObject.GetComponent<Laser>().IsEnemyLaser)
            {
                HandleArmament(other.gameObject, ResourceManager.Instance.LaserHitPoints);
            }
        }
        else if (other.tag == "FlameBomb")
        {
            if (other.gameObject.GetComponent<FlameBomb>().IsEnemyBomb == false)
            {
                HandleArmament(other.gameObject, ResourceManager.Instance.BombHitPoints);
            }
        }
        else if (other.tag == "Homing")
        {
            HandleArmament(other.gameObject, ResourceManager.Instance.HomingHitPoints);
        }
    }

    protected virtual void Damage()
    {
        if (_isShieldActive == true)
        {
            ProcessShieldHit();
            return;
        }

        _enemyHealth.ProcessDamage(); // default 1 life
        // could pass a life-draining value/percentage instead of a whole 1 
    }

    void ProcessShieldHit()
    {
        if (_shieldHitCount < _maxShieldHitCount)
        {
            UpdateShieldVisual(_shieldHitCount, false);
            _shieldHitCount += 1;
            if (_shieldHitCount < _maxShieldHitCount)
                UpdateShieldVisual(_shieldHitCount, true);
        }
        if (_shieldHitCount == _maxShieldHitCount)
        {
            _shieldHitCount = 0;
            _isShieldActive = false;
        }
    }

    protected virtual void LookForPowerup()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up); // straight down
        // set limit on Raycast?

        if (hit.collider != null)
        {
            if (hit.transform.gameObject.tag == "Powerup")
            {
                if (Time.time > _canFire)
                    FireLaser();
            }
        }
    }

    protected virtual void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);

        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].IsEnemyLaser = true;
        }
        _audioSourceLaser.Play();
    }

    protected virtual void ShieldActive()
    {
        if (!_isShieldActive)
        {
            _isShieldActive = true;

            if (_shieldHitCount < _maxShieldHitCount)
                _shieldVisualizer[_shieldHitCount].SetActive(true);
        }
    }

    public void UpdateShieldVisual(int shieldHitCount, bool active)
    {
        if (shieldHitCount < _maxShieldHitCount)
            _shieldVisualizer[shieldHitCount].SetActive(active);
    }

    private void HandleArmament(GameObject armament, int score)
    {
        Destroy(armament);

        _player?.UpdateScore(10); // aramament needs to have its score value attached or lookup-able

        Damage();
    }

    private void HandlePlayer(Player player)
    {
        _speed = 0;
        _canFire = -1;
        Destroy(GetComponent<Collider2D>());

        player?.Damage();
        _player?.UpdateScore(10);

        _animExplosion.SetTrigger("OnEnemyDeath");
        _audioSourceExplosion.Play();

        ResourceManager.Instance.UpdateEnemiesDestroyed();

        Destroy(this.gameObject, 1f);

        // if player rams enemy, then enemy is destroyed
        // but player suffers only damage but is not destroyed
        // so users will learn that ramming is an option 
    }
}
