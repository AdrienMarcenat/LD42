
using System;
using System.Collections.Generic;
using System.IO;
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
    private static string ms_LevelFilename = "Datas/LevelNames.txt";
    private TileCoordinates m_LevelDimension;
    private ELevelMode m_LevelMode = ELevelMode.Normal;

    public LevelManager ()
    {
        m_LevelDimension = new TileCoordinates (0, 0);
        m_LevelIdToName = new Dictionary<int, string> ();
        FillLevelNames (ms_LevelFilename);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    ~LevelManager ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public ELevelMode GetMode()
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

    public void NextLevel ()
    {
        m_CurrentLevel++;
    }

    public string GetActiveSceneName ()
    {
        return SceneManager.GetActiveScene ().name;
    }

    public string GetCurrentLevelName ()
    {
        return m_LevelIdToName[m_CurrentLevel];
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
            m_LevelDimension = LevelParser.GenLevel ("Datas/Level" + m_CurrentLevel + ".txt");
            new LevelEvent (m_CurrentLevel, true).Push ();
        }
    }

    public TileCoordinates GetLevelDimension ()
    {
        return m_LevelDimension;
    }

    private void FillLevelNames (string filename)
    {
        char[] separators = { ':' };
#if UNITY_EDITOR
        filename = "Assets/" + filename;
#endif

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
}

public class LevelManagerProxy : UniqueProxy<LevelManager>
{ }
