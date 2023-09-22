using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TDH;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Image _livesImage;

    [SerializeField] private TMP_Text _roundOverText;
    [SerializeField] private TMP_Text _restartText;
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private TMP_Text _activeText;
    [SerializeField] private TMP_Text _enemiesSpawnedText;
    [SerializeField] private TMP_Text _enemiesDestroyedText;
    [SerializeField] private TMP_Text _roundText;
    [SerializeField] private TMP_Text _roundSloganText;

    [SerializeField] private TMP_Text[] _powerUpStatsText;
    [SerializeField] private GameObject _uiResourcesPanel;

    // Thruster charge indicator shows 1 thruster image as a normal base,
    // shows 2 thruster images when the Player uses LeftShift to speed up,
    // and shows 3 thrusters when the Speed powerup is active
    private const int _thrusterLevelNormal = 0;
    private const int _thrusterLevelMedium = 1;
    private const int _thrusterLevelHigh = 2;
    [SerializeField] private Image[] _thrusters;

    // Health, Ammo, Shield, TripleShot, Speed, Bomb,
    // Deceptron, Fuel, Homing

    //[SerializeField] GameManager gm;

    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _roundOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _ammoText.text = "Ammo: ";
        _activeText.text = "Active: ";
        _enemiesSpawnedText.text = "Spawned: ";
        _enemiesDestroyedText.text = "Destroyed: ";

        var roundText = ResourceManager.Instance.CurrentRound();

        _roundText.text = "Round: " + roundText.ToString();
        _roundSloganText.text = "Enemies!!";

        _thrusters[_thrusterLevelNormal].gameObject.SetActive(true);
        _thrusters[_thrusterLevelMedium].gameObject.SetActive(false);
        _thrusters[_thrusterLevelHigh].gameObject.SetActive(false);
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateAmmoText(string ammoText)
    {
        _ammoText.text = "Ammo: " + ammoText;
    }

    public void UpdateActiveText(string powerUpText)
    {
        _activeText.text = "Active: " + powerUpText;
    }

    public void UpdateEnemiesDestroyedText(string enemiesDestroyed)
    {
        _enemiesDestroyedText.text = "Destroyed: " + enemiesDestroyed;
    }

    public void UpdateEnemiesSpawnedText(string enemiesSpawned)
    {
        _enemiesSpawnedText.text = "Spawned: " + enemiesSpawned;
    }

    public void UpdateRoundText(string roundText)
    {
        _roundText.text = "Round: " + roundText;
    }

    public void UpdateRoundSloganText(string roundSloganText)
    {
        _roundSloganText.text = roundSloganText;
    }

    public void UpdateCurrentLives(int currentLives)
    {
        if (currentLives == 0)
        {
            _livesImage.sprite = _livesSprites[currentLives];
            RestartRoundSequence();
        }
        else
        {
            if (currentLives < _livesSprites.Length)
            {
                _livesImage.sprite = _livesSprites[currentLives];
            }
        }
    }

    public void UpdateThrusterLevel(int thrusterLevel)
    {
        switch (thrusterLevel)
        {
            case 0:
                _thrusters[_thrusterLevelHigh].gameObject.SetActive(false);
                _thrusters[_thrusterLevelMedium].gameObject.SetActive(false);
                _thrusters[_thrusterLevelNormal].gameObject.SetActive(true);
                break;
            case 1:
                _thrusters[_thrusterLevelHigh].gameObject.SetActive(false);
                _thrusters[_thrusterLevelMedium].gameObject.SetActive(true);
                _thrusters[_thrusterLevelNormal].gameObject.SetActive(true);
                break;
            case 2:
                _thrusters[_thrusterLevelHigh].gameObject.SetActive(true);
                _thrusters[_thrusterLevelMedium].gameObject.SetActive(true);
                _thrusters[_thrusterLevelNormal].gameObject.SetActive(true);
                break;
            default:
                Debug.LogError("Default thruster level. No thruster found.");
                break;
        }
    }

    public void UpdatePowerupStats(ResourceManager.PowerUps p, string newText)
    {
        int index = Array.IndexOf(Enum.GetValues(p.GetType()), p); // tricky call

        _powerUpStatsText[index].SetText(p + ": " + newText);
    }

    public void HideResourcePanel()
    { _uiResourcesPanel.SetActive(false); }

    public void ShowResourcesPanel()
    { _uiResourcesPanel.SetActive(true); }


    void RestartRoundSequence()
    {
        _roundOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(RoundOverFlickerRoutine("ROUND OVER"));

        SpawnManager.Instance.RestartRound();

        GameManager gm = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (gm != null)
            gm.RestartCurrentRound();
    }

    public void StartNewRoundSequence()
    {
        _roundOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true); 

        StartCoroutine(RoundOverFlickerRoutine("START NEXT ROUND"));
        GameManager gm = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (gm != null)
        {
            gm.StartNewRound();
        }
        else
        {
            Debug.Log("UIManager:StartNewRoundSequence:GM is NULL.");
        }
    }

    IEnumerator RoundOverFlickerRoutine(string showText)
    {
        while (true)
        {
            _roundOverText.text = showText;
            yield return new WaitForSeconds(0.5f);
            _roundOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
