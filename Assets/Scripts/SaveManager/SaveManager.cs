using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{

    public SaveState state;

    [HideInInspector]
    public bool preventSaving;

    protected override void Awake()
    {
        base.Awake();
        Load();
    }

    // Save the whole state of this saveState script to the player pref
    public void Save()
    {
        if (preventSaving)
        {
            return;
        }

        PlayerPrefs.SetString("save", Helper.Encrypt(Helper.Serialize<SaveState>(state)));
        PlayerPrefs.Save();
    }

   

    // Save the whole state of this saveState script to the player pref
    public void SavePreCreatedData()
    {
        if (preventSaving)
        {
            return;
        }

        state.LoadPreCreatedSaveState();
        PlayerPrefs.SetString("save", Helper.Encrypt(Helper.Serialize<SaveState>(state)));
        PlayerPrefs.Save();
    }



    // Load the previous saved state from the player prefs
    public void Load()
    {
        // Do we already have a save?
        if (PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<SaveState>(Helper.Decrypt(PlayerPrefs.GetString("save")));
        }
        else
        {
            state = new SaveState();
            state.LoadPreCreatedSaveState();
            Save();
            Debug.Log("No save file found, creating a new one!");
        }
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }

    public void SaveSettings()
    {
        Save();
    }

    public void SetLastLevelID(int lastLevelID)
    {
        state.lastLevelID = lastLevelID;
        Save();
    }

    public int GetLastLevelID()
    {
        return state.lastLevelID;
    }

    public void SetPreviousLevelID(int previousLevelID)
    {
        state.previousLevelID = previousLevelID;
        Save();
    }

    public int GetPreviousLevelID()
    {
        return state.previousLevelID;
    }

    public void SetLastRandomLevelID(int lastRandomLevelID)
    {
        state.lastRandomLevelID = lastRandomLevelID;
        Save();
    }

    public int GetLastRandomLevelID()
    {
        return state.lastRandomLevelID;
    }

    public void IncreaseLevelCounter()
    {
        state.levelCounter++;
        Save();
    }

    public int GetLevelCounter()
    {
        return state.levelCounter;
    }

    public void SetIsFirstPlay()
    {
        state.isFirstPlay = false;
    }

    public bool IsFirstPlay()
    {
        return state.isFirstPlay;
    }

    public List<int> GetNewOrOrderChangedLevels()
    {
        return state.newOrOrderChangedLevels;
    }

    public void RemoveLevelFromNewOrOrderChangedLevels(int levelID)
    {
        if (state.newOrOrderChangedLevels.Contains(levelID))
        {
            state.newOrOrderChangedLevels.Remove(levelID);
            Save();
        }
    }

    public void AddLevelsToNewOrOrderChangedLevels(List<int> levelIDs)
    {
        foreach (int levelID in levelIDs)
        {
            if (state.newOrOrderChangedLevels.Contains(levelID))
                continue;

            state.newOrOrderChangedLevels.Add(levelID);
        }

        Save();
    }

    public void ClearNewOrOrderChangedLevels()
    {
        if (state.newOrOrderChangedLevels.Count > 0)
        {
            state.newOrOrderChangedLevels.Clear();
            Save();
        }
    }

    public List<LevelOrderIndex> GetLevelOrder()
    {
        return state.levelOrder;
    }

    public void SetNewLevelOrder(List<LevelOrderIndex> levelOrder)
    {
        state.levelOrder.Clear();
        foreach (LevelOrderIndex levelOrderLevel in levelOrder)
            state.levelOrder.Add(levelOrderLevel);
        Save();
    }

    public bool IsLevelCompleted(int levelID)
    {
        return state.completedLevels.Contains(levelID);
    }

    public void AddLevelToCompletedLevels(int levelID)
    {
        if (!state.completedLevels.Contains(levelID))
        {
            state.completedLevels.Add(levelID);
            Save();
        }
    }

    public bool GetHapticState()
    {
        return state.isHapticActive;
    }

    public void SetHapticState(bool hapticState)
    {
        state.isHapticActive = hapticState;
        Save();
    }

    public int GetGemCount()
    {
        return state.gemCount;
    }

    
    public void SetGemCount(int i)
    {
        state.gemCount = i;
        Save();
    }

    internal int GetOrder()
    {
        return state.order;
    }

    internal void SetOrder(int order)
    {
        state.order = order ;
        Save();
    }

    public int GetTotalSpentAmount() {
        return state.totalSpent;
    }
    public void SetTotalSpentAmount(int i) {
        state.totalSpent = i;
        Save();
    }


    internal int GetPopularityValue()
    {
        return state.popularity;
    }

    public void SetPopularity(int i)
    {
        state.popularity = i;
        Save();
    }

    internal int GetPopularityLevel()
    {
        return state.popularityLevel;
    }

    public void IncreasePopularityLevel()
    {
        state.popularityLevel++;
        Save();
    }

    public void SetActiveRing(int activeRingIndex) {
        state.activeRing = activeRingIndex;
        Save();
    }

    public int GetActiveRing()
    {
       return state.activeRing;
         
    }
}
