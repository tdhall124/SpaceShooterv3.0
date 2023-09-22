using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isGameOver = false;

    // Round management
    private int _maxRounds = 7;
    public int MaxRounds { get { return _maxRounds; } }

    private int _currentRound;
    public int CurrentRound { get { return _currentRound; } }

    private static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        _currentRound = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartNewRound()
    {
        ResourceManager.Instance.ResetPowerupStats(); // ?? are you sure?

        if (_currentRound < _maxRounds)
        {
            _currentRound++;
        }

        _isGameOver = true;
    }

    public void RestartCurrentRound()
    {
        _isGameOver = true;
    }

    public void GameOver()
    {
        _isGameOver = true;
        _currentRound = 1;
    }
}
