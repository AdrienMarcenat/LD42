using System.Collections.Generic;

public class GoalManager
{
    private List<Bin> m_Bins;
    private List<int> m_BinNumberSpawned;
    private Dictionary<TileCoordinates, BinGoal> m_BinGoals;
    private Dictionary<TileCoordinates, bool> m_BinGoalFlags;

    public GoalManager ()
    {
        m_Bins = new List<Bin> ();
        m_BinGoals = new Dictionary<TileCoordinates, BinGoal> ();
        m_BinGoalFlags = new Dictionary<TileCoordinates, bool> ();
        m_BinNumberSpawned = new List<int> ();
    }

    public void RegisterBin (Bin bin)
    {
        m_Bins.Add (bin);
        m_BinNumberSpawned.Add (bin.GetNumber ());
    }

    public void UnegisterBin (Bin bin)
    {
        m_Bins.Remove (bin);
        m_BinNumberSpawned.Remove (bin.GetNumber ());

    }
    public void RegisterBinGoal (BinGoal binGoal)
    {
        if(LevelManagerProxy.Get().GetMode() == ELevelMode.MaxBin)
        {
            binGoal.gameObject.SetActive (false);
        }
        else
        {
            m_BinGoals.Add (binGoal.GetCoordinates (), binGoal);
            m_BinGoalFlags.Add (binGoal.GetCoordinates (), false);
        }
    }

    public bool CheckEndCondition()
    {
        foreach (bool binGoalFlag in m_BinGoalFlags.Values)
        {
            if (!binGoalFlag)
            {
                return false;
            }
        }
        return true;
    }

    public void Reset ()
    {
        m_Bins.Clear ();
        m_BinGoals.Clear ();
        m_BinGoalFlags.Clear ();
        m_BinNumberSpawned.Clear ();
    }

    public void OnBinPlaced (Bin bin)
    {
        if (DoesBinMatchWithGoal (bin))
        {
            m_BinGoalFlags[bin.GetCoordinates ()] = true;
            if (CheckEndCondition ())
            {
                new GameFlowEvent (EGameFlowAction.EndLevel).Push ();
                return;
            }
            int nextBinNumber = bin.GetNumber () + 1;
            if (!m_BinNumberSpawned.Contains(nextBinNumber)) 
            {
                new BinSpawnEvent (true, nextBinNumber).Push ();
            }
        }
    }

    public void OnBinRemoved (Bin bin, bool undo)
    {
        if (DoesBinMatchWithGoal (bin))
        {
            m_BinGoalFlags[bin.GetCoordinates ()] = false;
            int nextBinNumber = bin.GetNumber () + 1;
            foreach (Bin existingBin in m_Bins)
            {
                if(existingBin.GetNumber() == nextBinNumber && existingBin.IsSpawned() && undo)
                {
                    new BinSpawnEvent (false, nextBinNumber).Push ();
                    break;
                }
            }
        }
    }

    public bool DoesBinMatchWithGoal (Bin bin)
    {
        BinGoal binGoal;
        if (m_BinGoals.TryGetValue (bin.GetCoordinates (), out binGoal))
        {
            return (binGoal.GetNumber () == bin.GetNumber ());
        }
        return false;
    }
}

public class GoalManagerProxy : UniqueProxy<GoalManager>
{ }
