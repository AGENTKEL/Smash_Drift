using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObstGenerator : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnPos;
    public GameObject[] prefabsToSpawn; // Array of prefabs to spawn randomly
    public int spawnCount = 10;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public float spawnHeight = 1f;

    [Header("UI Settings")]
    public TextMeshProUGUI currentLevel;
    public GameObject startUI;
    public GameObject gamePlayUI;
    public GameObject gameOverUI;
    public GameObject pauseUI;
    public GameObject completionUI;
    public Slider progressSlider;
    public GameObject arrowPointer;
    
    [Header("Timer Settings")]
    public TextMeshProUGUI timerText;
    public float levelTime = 60f;
    private float timeRemaining;
    private bool isTimerRunning = false;

    private int destroyedObstacles = 0;

    [SerializeField] private UserControl _userControl;

    void Start()
    {
        SetCurrentLevelText();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                GameOver();
            }
        }
    }

    void SpawnPrefabs()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogError("Prefabs array is empty! Assign at least one prefab.");
            return;
        }

        destroyedObstacles = 0;
        timeRemaining = levelTime;
        isTimerRunning = true;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                spawnHeight,
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
            );

            // Pick a random prefab from the array
            GameObject randomPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

            GameObject spawnedObstacle = Instantiate(randomPrefab, randomPosition, Quaternion.identity);
            spawnedObstacle.GetComponent<Obstacle>().SetGenerator(this);
        }

        if (completionUI != null)
            completionUI.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = spawnCount;
            progressSlider.value = 0;
        }

        UpdateTimerUI();
    }

    public void ObstacleDestroyed()
    {
        destroyedObstacles++;

        if (progressSlider != null)
        {
            progressSlider.value = destroyedObstacles;
        }

        if (destroyedObstacles >= spawnCount)
        {
            isTimerRunning = false;
            LevelFinish();
        }
    }
    
    public void SetCurrentLevelText()
    {
        currentLevel.text = "LEVEL " + GameManager.instance.levelsPassed;
    }

    public void StartGame()
    {
        startUI.SetActive(false);
        gamePlayUI.SetActive(true);
        SpawnPrefabs();
        _userControl.canMove = true;
        arrowPointer.SetActive(true);
    }

    public void LevelFinish()
    {
        GameManager.instance.PassLevel();
        SetCurrentLevelText();
        completionUI.SetActive(true);
        gamePlayUI.SetActive(false);
        _userControl.canMove = false;
        isTimerRunning = false;
    }
    
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        gamePlayUI.SetActive(false);
        _userControl.canMove = false;
        isTimerRunning = false;
    }
    
    public void Pause()
    {
        pauseUI.SetActive(true);
        _userControl.canMove = false;
    }
    
    public void UnPause()
    {
        pauseUI.SetActive(false);
        _userControl.canMove = true;
    }
    
    public void Respawn()
    {
        _userControl.transform.position = spawnPos.transform.position;
        _userControl.transform.rotation = spawnPos.transform.rotation;
        UnPause();
    }
    
    public void ToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
