﻿
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEvent : GameEvent
{
    public LevelEvent (int levelIndex, bool enter) : base ("Game", EProtocol.Instant)
    {
        m_LevelIndex = levelIndex;
        m_Enter = enter;
    }

    public int GetLevelIndex ()
    {
        return m_LevelIndex;
    }

    public bool IsEntered ()
    {
        return m_Enter;
    }

    private int m_LevelIndex;
    private bool m_Enter;
}

public enum ELevelMode
{
    Normal,
    MaxBin,
}

public class LevelManager
{
    private int m_CurrentLevel = 0;
    private Dictionary<int, string> m_LevelIdToName;
    private Dictionary<int, int> m_LevelIdToScore;
    private static string ms_LevelFilename = "/LevelNames.txt";
    private TileCoordinates m_LevelDimension;
    private ELevelMode m_LevelMode = ELevelMode.Normal;
    private bool m_SkipDialogue = false;
    private int m_NumberOfMove = 0;

    public LevelManager ()
    {
        m_LevelDimension = new TileCoordinates (0, 0);
        m_LevelIdToName = new Dictionary<int, string> ();
        m_LevelIdToScore = new Dictionary<int, int> ();
        FillLevelNames (ms_LevelFilename);
        FillLevelScores ();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    ~LevelManager ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public ELevelMode GetMode ()
    {
        return m_LevelMode;
    }

    public void LoadScene (int sceneIndex)
    {
        new LevelEvent (m_CurrentLevel, false).Push ();
        SceneManager.LoadScene (sceneIndex);
    }

    public void LoadScene (string sceneName)
    {
        new LevelEvent (m_CurrentLevel, false).Push ();
        SceneManager.LoadScene (sceneName);
    }

    public void LoadLevel ()
    {
        LoadScene ("Scenes/Level");
    }

    public void SetLevelIndex (int levelIndex)
    {
        m_CurrentLevel = levelIndex;
    }

    public bool IsLastLevel ()
    {
        return m_CurrentLevel == m_LevelIdToName.Count - 1;
    }

    public void NextLevel ()
    {
        if (!IsLastLevel ())
        {
            m_CurrentLevel++;
        }
    }

    public string GetActiveSceneName ()
    {
        return SceneManager.GetActiveScene ().name;
    }

    public string GetCurrentLevelName ()
    {
        return m_LevelIdToName[m_CurrentLevel];
    }

    public int GetCurrentLevelID ()
    {
        return m_CurrentLevel;
    }

    public Dictionary<int, string> GetLevelNames ()
    {
        return m_LevelIdToName;
    }

    public void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        if (GetActiveSceneName () == "Level")
        {
            TileManagerProxy.Get ().Reset ();
            GoalManagerProxy.Get ().Reset ();
            CommandStackProxy.Get ().Reset ();
            m_NumberOfMove = 0;
            string levelName = GetCurrentLevelName ();
            m_LevelDimension = LevelParser.GenLevel ("/" + levelName + ".txt");
            new LevelEvent (m_CurrentLevel, true).Push ();
            new BinSpawnEvent (true, 0).Push ();
            new DialogueEvent (levelName + "-start").Push ();
        }
    }

    public TileCoordinates GetLevelDimension ()
    {
        return m_LevelDimension;
    }

    private void FillLevelNames (string filename)
    {
        char[] separators = { ':' };
        filename = Application.streamingAssetsPath + filename;

        string[] lines = File.ReadAllLines (filename);

        for (int i = 0; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split (separators);

            // If there is an error in print a debug message
            if (datas.Length != 2)
            {
                this.DebugLog ("Invalid number of data line " + i + " expecting 2, got " + datas.Length);
                return;
            }

            int levelIndex = Int32.Parse ((String)datas.GetValue (0)); ;
            string levelName = datas[1];
            m_LevelIdToName.Add (levelIndex, levelName);
        }
    }

    private void FillLevelScores ()
    {
        foreach (int levelID in m_LevelIdToName.Keys)
        {
            int score = PlayerPrefs.GetInt (m_LevelIdToName[levelID], -1);
            m_LevelIdToScore.Add (levelID, score);
        }
    }

    public int GetCurrentLevelScore ()
    {
        return m_LevelIdToScore[m_CurrentLevel];
    }

    public void IncreaseNumberOfMove()
    {
        m_NumberOfMove++;
    }

    public void DecreaseNumberOfMove ()
    {
        m_NumberOfMove--;
    }

    public int GetNumberOfMove ()
    {
       return  m_NumberOfMove;
    }

    public void OnLevelEnd ()
    {
        int score = m_NumberOfMove;
        int prevScore = m_LevelIdToScore[m_CurrentLevel];
        if (score < prevScore || prevScore == -1)
        {
            PlayerPrefs.SetInt (GetCurrentLevelName (), score);
            m_LevelIdToScore[m_CurrentLevel] = score;
        }
        new DialogueEvent (GetCurrentLevelName () + "-end").Push ();
    }
}

public class LevelManagerProxy : UniqueProxy<LevelManager>
{ }
