using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Image _livesImage;

    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _restartText;
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private TMP_Text _activeText;

    // Thruster charge indicator shows 1 thruster image as a normal base,
    // shows 2 thruster images when the Player uses LeftShift to speed up,
    // and shows 3 thrusters when the Speed powerup is active
    private const int _thrusterLevelNormal = 0;
    private const int _thrusterLevelMedium = 1;
    private const int _thrusterLevelHigh = 2;
    [SerializeField] private Image[] _thrusters;

    [SerializeField] private int _maxShots = 15;

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _ammoText.text = "Ammo: " + _maxShots;
        _activeText.text = "Active: ";

        _thrusters[_thrusterLevelNormal].gameObject.SetActive(true);
        _thrusters[_thrusterLevelMedium].gameObject.SetActive(false);
        _thrusters[_thrusterLevelHigh].gameObject.SetActive(false);
    }
   
    public void UpdateScoreText(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateAmmoText(int shots)
    {
        _ammoText.text = "Ammo: " + shots;
    }

    public void UpdateActiveText(string powerUpText)
    {
        _activeText.text = "Active: " + powerUpText;
    }

    public void UpdateLives(int currentLives)
    {
        // display img sprite
        // give it a new one based on currentLives index
        _livesImage.sprite = _livesSprites[currentLives]; // PROBLEM HERE index out of bounds
        if (currentLives == 0)
        {
            GameOverSequence();
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

    void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());

        GameManager gm = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (gm != null)
        {
            gm.GameOver();
        }
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
