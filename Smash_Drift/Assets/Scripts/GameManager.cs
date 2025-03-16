using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int levelsPassed;

    private const string LevelsPassedKey = "LevelsPassed"; // Key for PlayerPrefs

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress(); // Load saved levelsPassed
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        LoadProgress();
    }

    public void PassLevel()
    {
        levelsPassed++;
        SaveProgress(); // Save progress after passing a level
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(LevelsPassedKey, levelsPassed);
        PlayerPrefs.Save(); // Ensure the data is written to disk
    }

    public void LoadProgress()
    {
        levelsPassed = PlayerPrefs.GetInt(LevelsPassedKey, 0); // Default to 0 if no save data exists
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LevelsPassedKey);
        levelsPassed = 0;
    }
}
