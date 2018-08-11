
using UnityEngine.SceneManagement;

public class LevelEvent : GameEvent
{
    public LevelEvent(int levelIndex, bool enter) : base("Game", EProtocol.Instant)
    {
        m_LevelIndex = levelIndex;
        m_Enter = enter;
    }

    public int GetLevelIndex()
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

public class LevelManager
{
    private int m_CurrentLevel = 1;
    
    public LevelManager ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    ~LevelManager ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene ().name;
    }

    public void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        if (GetActiveSceneName() == "Level")
        {
            TileManagerProxy.Get ().Reset ();
            LevelParser.GenLevel ("Datas/Level" + m_CurrentLevel + ".txt");
            new LevelEvent (m_CurrentLevel, true).Push ();
        }
    }
}

public class LevelManagerProxy : UniqueProxy<LevelManager>
{ }
