using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private UserControl carObject;
    [SerializeField] private Transform carSpawnPos;
    [SerializeField] private GameObject gameOverCollider;

    [Header("Road Settings")]
    public List<GameObject> roadPrefabs; 
    public int minRoadLength = 5; // Minimum road length
    public int maxRoadLength = 15; // Maximum road length
    private int roadLength; // Road length is now random
    public float roadSpacing = 5f; 

    [Header("Turn Roads")]
    public GameObject roadTurnRight;
    public GameObject roadTurnLeft;
    public Vector3 turnRightOffset = new Vector3(5f, 0f, 10f);
    public Vector3 turnLeftOffset = new Vector3(-5f, 0f, 10f);

    [Header("Obstacle Settings")]
    public GameObject obstaclePrefab;
    public int minObstaclesPerSegment = 1;
    public int maxObstaclesPerSegment = 3;

    [Header("Obstacle Position Tweaks")]
    public float obstacleHeightOffset = 0.2f; 
    public float horizontalOffset = 0f; 

    [Header("Finish Settings")]
    public GameObject finishPrefab;
    public float finishHeightOffset = 1f; 
    public float finishZOffset = 0f; 

    private Vector3 spawnPosition = Vector3.zero;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Transform tempParent; 
    private float roadLengthSize; 

    [Header("UI")]
    public GameObject startMenuUI;
    public GameObject gameOverUI;
    public GameObject levelEndUI;
    public GameObject gamePlayUI;
    public GameObject pauseUI;
    public ProgressSlider progressSlider;
    public TextMeshProUGUI currentLevel;

    void Start()
    {
        GameObject tempObj = GameObject.Find("Temp");
        if (tempObj == null)
        {
            tempObj = new GameObject("Temp");
        }
        tempParent = tempObj.transform;

        if (roadPrefabs.Count > 0)
        {
            GameObject tempRoad = Instantiate(roadPrefabs[0]);
            Road roadComponent = tempRoad.GetComponent<Road>();
            if (roadComponent != null)
            {
                roadLengthSize = roadComponent.RoadSize.z;
            }
            Destroy(tempRoad);
        }
        else
        {
            Debug.LogError("No road prefabs assigned to the list!");
            return;
        }

        GenerateLevel();
        SetCurrentLevelText();
        progressSlider.SetActive(false);
    }

    void GenerateLevel()
    {
        ClearLevel();
        spawnPosition = Vector3.zero;

        roadLength = Random.Range(minRoadLength, maxRoadLength + 1); // Random road length

        GameObject tempParent = new GameObject("Temp"); 
        GameObject lastRoad = null; 

        GameObject firstRoadPrefab = roadPrefabs[0];
        GameObject firstRoad = Instantiate(firstRoadPrefab, spawnPosition, Quaternion.identity, tempParent.transform);
        spawnedObjects.Add(firstRoad);
        lastRoad = firstRoad; 

        Road firstRoadScript = firstRoad.GetComponent<Road>();
        float actualRoadLength = firstRoadScript != null ? firstRoadScript.RoadSize.z : 10f;
        spawnPosition.z += actualRoadLength + roadSpacing;

        for (int i = 1; i < roadLength; i++)
        {
            GameObject selectedRoadPrefab;
            Vector3 offset = Vector3.zero;

            int roadType = Random.Range(0, 10);
            if (roadType < 2 && roadTurnRight != null)  
            {
                selectedRoadPrefab = roadTurnRight;
                offset = turnRightOffset;
            }
            else if (roadType < 4 && roadTurnLeft != null)  
            {
                selectedRoadPrefab = roadTurnLeft;
                offset = turnLeftOffset;
            }
            else 
            {
                selectedRoadPrefab = roadPrefabs[Random.Range(0, roadPrefabs.Count)];
            }

            Vector3 roadPos = spawnPosition;
            GameObject road = Instantiate(selectedRoadPrefab, roadPos, Quaternion.identity, tempParent.transform);
            spawnedObjects.Add(road);
            lastRoad = road; 

            Road roadScript = road.GetComponent<Road>();
            actualRoadLength = roadScript != null ? roadScript.RoadSize.z : 10f;

            int obstacleCount = Random.Range(minObstaclesPerSegment, maxObstaclesPerSegment + 1);
            for (int j = 0; j < obstacleCount; j++)
            {
                Vector3 obstaclePos = new Vector3(
                    roadPos.x + horizontalOffset,
                    roadPos.y + (roadScript.RoadSize.y / 2) + obstacleHeightOffset,
                    roadPos.z
                );

                GameObject obstacle = Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity, tempParent.transform);
                spawnedObjects.Add(obstacle);
            }

            spawnPosition += offset;
            spawnPosition.z += actualRoadLength + roadSpacing;
        }

        if (finishPrefab != null)
        {
            Vector3 finishPosition = spawnPosition + new Vector3(0, finishHeightOffset, finishZOffset);
            GameObject finish = Instantiate(finishPrefab, finishPosition, Quaternion.identity, tempParent.transform);
            spawnedObjects.Add(finish);
    
            Finish finishScript = finish.GetComponent<Finish>();
            CarController car = FindObjectOfType<CarController>();
            progressSlider.SetActive(true);
            finishScript.Initialize(car.transform, progressSlider);
            finishScript.ResetProgress();
            progressSlider.SetActive(false);
        }
    }

    void ClearLevel()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    public void StartLevel()
    {
        startMenuUI.SetActive(false);
        gamePlayUI.SetActive(true);
        carObject.canMove = true;
        carObject.tutorialUI.SetActive(true);
        progressSlider.SetActive(true);
    }
    
    public void MenuFinish()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void NextLevel()
    {
        RestartCarPos();
        GenerateLevel();
        levelEndUI.SetActive(false);
        gamePlayUI.SetActive(true);
        gameOverCollider.SetActive(true);
        carObject.tutorialUI.SetActive(true);
        progressSlider.SetActive(true);
    }
    
    public void LevelFinish()
    {
        levelEndUI.SetActive(true);
        carObject.canMove = false;
        gamePlayUI.SetActive(false);
        gameOverCollider.SetActive(false);
    }
    
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        carObject.canMove = false;
        gamePlayUI.SetActive(false);
    }
    
    public void RestartCarPos()
    {
        UnpauseGame();
        carObject.transform.position = carSpawnPos.position;
        carObject.transform.rotation = carSpawnPos.rotation;
        carObject.canMove = true;
        carObject.RestartCarSpeed();
        gameOverUI.SetActive(false);
        pauseUI.SetActive(false);
        gamePlayUI.SetActive(true);
        progressSlider.SetActive(true);
        carObject.tutorialUI.SetActive(true);
        FindObjectOfType<Finish>().ResetProgress();
        EndWall[] endWalls = FindObjectsOfType<EndWall>(); // Find all EndWall objects

        foreach (EndWall wall in endWalls)
        {
            wall.RestartColliders();
        }
    }
    
    public void MenuGameOver()
    {
        FindObjectOfType<Finish>().ResetProgress();
        carObject.transform.position = carSpawnPos.position;
        carObject.transform.rotation = carSpawnPos.rotation;
        startMenuUI.SetActive(true);
        gameOverUI.SetActive(false);
        gamePlayUI.SetActive(false);
        UnpauseGame();
        carObject.canMove = false;
        carObject.RestartCarSpeed();
        progressSlider.SetActive(false);
    }

    public void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
    }

    public void SetCurrentLevelText()
    {
        currentLevel.text = "Level " + GameManager.instance.levelsPassed;
    }
}
