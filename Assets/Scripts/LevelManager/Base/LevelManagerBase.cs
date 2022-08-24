using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public abstract class LevelManagerBase : MonoBehaviour
{
    [HideInInspector] [SerializeField] protected List<LevelOrderIndex> levelOrder = new List<LevelOrderIndex>();
    [SerializeField] protected Transform levelsContainer;
    [SerializeField] protected int startLevelID;
    [SerializeField] private bool startWithStartLevel;
    [SerializeField] private bool useWithScenes;
    [SerializeField] private bool returnToStartWhenGameFinished;
    [SerializeField] private List<int> excludedLevelIDsAfterGameFinished = new List<int>();

    protected List<Level> _levels = new List<Level>();
    protected int _currentLevelID;
    protected List<int> _tempLevelOrderForNewlyAddedOrNotPlayedLevels = new List<int>();
    protected List<int> _tempLevelOrderForSelectingRandomLevel = new List<int>();
    protected string _levelOrderText;

    public int CurrentLevelID => _currentLevelID;
    public string LevelOrderText => _levelOrderText;
    public List<Level> Levels => _levels;
    public List<LevelOrderIndex> LevelOrder => levelOrder;

    protected virtual void Awake()
    {
        if(levelsContainer == null)
            levelsContainer = this.transform;

        SetLevels(levelsContainer.GetComponentsInChildren<Level>(true).ToList());

        // Create Level Order Text
        CreateLevelOrderText();

        if(useWithScenes) DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        
        if (IsFirstPlay())
        {
            SetIsFirstPlay();
            SetLastLevelID(levelOrder[0].LevelID);
            _currentLevelID = levelOrder[0].LevelID;

        }
    }

    // Replaces the level order text with provided text.
    public void UpdateLevelOrderText(string newLevelOrderText)
    {
        _levelOrderText = newLevelOrderText;
    }

    // Updates the level order list with using level order text.
    public virtual void UpdateLevelOrderFromLevelOrderText()
    {
        if (string.IsNullOrEmpty(_levelOrderText))
            return;

        string[] splittedLevelOrder = _levelOrderText.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        levelOrder.Clear();
        foreach (string splittedLevel in splittedLevelOrder)
        {
            string[] splittedLevelInfo = splittedLevel.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);
            int levelID = int.Parse(splittedLevelInfo[0]);

            if (_levels != null && _levels.Count > 0)
            {
                if (_levels.Find(level => level.LevelID == levelID) != null)
                    levelOrder.Add(new LevelOrderIndex(levelID, splittedLevelInfo.Length <= 1 ? null : splittedLevelInfo[1]));
            }
            else
                levelOrder.Add(new LevelOrderIndex(levelID, splittedLevelInfo.Length <= 1 ? null : splittedLevelInfo[1]));
        }
    }

    // Creates level order text from level order list.
    public virtual string CreateLevelOrderText()
    {
        _levelOrderText = "";
        levelOrder = levelOrder.Distinct().ToList();
        foreach (LevelOrderIndex levelOrderLevel in levelOrder)
        {
            if (string.IsNullOrEmpty(levelOrderLevel.LevelName))
                _levelOrderText += levelOrderLevel.LevelID;
            else
                _levelOrderText += levelOrderLevel.LevelID + "_" + levelOrderLevel.LevelName;

            if (levelOrderLevel.LevelID != levelOrder.Last().LevelID)
                _levelOrderText += ",";
        }

        return _levelOrderText;
    }

    // Exports the current level order list as text file.
    [ContextMenu("Create Level Order Text")]
    private void ExportLevelOrderAsTextFile()
    {
        if (levelOrder.Count <= 0)
            return;

        int textFileCountInAssets = Directory.GetFiles("Assets/", "LevelOrderText*.txt").Length;
        string savePath = Application.dataPath + "/LevelOrderText" + (textFileCountInAssets + 1).ToString() + ".txt";
        var streamWriter = File.CreateText(savePath);
        streamWriter.Write(CreateLevelOrderText());
        streamWriter.Close();
    }

    public virtual void OpenCurrentLevel()
    {
        if (_levels != null && _levels.Count > 0)
        {
            DeactivateAllLevels();
            //_currentLevel.gameObject.SetActive(true);
            _levels.Find(l => l.LevelID == _currentLevelID).gameObject.SetActive(true);
        }

        if (useWithScenes)
            OpenCurrentLevelScene();
    }

    protected virtual void OpenCurrentLevelScene()
    {
        if (/*_currentLevel.LevelID*/_currentLevelID < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(/*_currentLevel.LevelID*/_currentLevelID, LoadSceneMode.Single);
        else
            Debug.Log("Current Level Is Not In The Build Settings! " + "LevelID => " + /*_currentLevel.LevelID*/_currentLevelID);
    }

    public void DeactivateAllLevels()
    {
        foreach (Level level in _levels)
            level.gameObject.SetActive(false);
    }

    public void CheckLevelChangesAndChangeLevel(bool openLevelImmediately = true)
    {
        // If start with start level ticked it starts from that level.
        if (startWithStartLevel)
        {
            SetLastLevelID(startLevelID);
            SetPreviousLevelID(startLevelID);
            SetLevelOrder(levelOrder);
            SaveData();

            //_currentLevel = _levels.Find(level => level.LevelID == startLevelID);
            _currentLevelID = startLevelID;
            OpenCurrentLevel();

            return;
        }

        // If the player opens the game first time, it shows the first level in level order and sets the necessary save data.
        if (IsFirstPlay())
        {
            SetIsFirstPlay();
            SetLastLevelID(levelOrder[0].LevelID);
            SetPreviousLevelID(levelOrder[0].LevelID);
            SetLevelOrder(levelOrder);
            SaveData();

            //_currentLevel = _levels.Find(level => level.LevelID == levelOrder[0].LevelID);
            _currentLevelID = levelOrder[0].LevelID;
            if(openLevelImmediately) OpenCurrentLevel();
        }
        else
        {
            List<LevelOrderIndex> previousLevelOrder = GetLevelOrderFromSaveData();
            ClearNewOrOrderChangedLevelsFromSaveData();

            // If saved level order is different than current level order, check the changes and open the level according to this changes.
            // If saved level order is not different than the current level order, open the last level.
            if (!levelOrder.Equals(previousLevelOrder))
            {
                int lastRandomLevelID = GetLastRandomLevelID();

                // If player is finished the game and plays random levels.
                if (lastRandomLevelID >= 0)
                {
                    if (!returnToStartWhenGameFinished)
                    {
                        CheckNewlyAddedOrNotPlayedLevelsAndSave();

                        if (_tempLevelOrderForNewlyAddedOrNotPlayedLevels.Count > 0)
                        {
                            //_currentLevel = _levels.Find(level => level.LevelID == _tempLevelOrderForNewlyAddedOrNotPlayedLevels[0]);
                            _currentLevelID = _tempLevelOrderForNewlyAddedOrNotPlayedLevels[0];
                        }
                        else if (levelOrder.Exists(lo => lo.LevelID == lastRandomLevelID))
                        {
                            //_currentLevel = _levels.Find(level => level.LevelID == lastRandomLevelID);
                            _currentLevelID = lastRandomLevelID;
                        }
                        else
                        {
                            int selectedLevelID = levelOrder[Random.Range(0, levelOrder.Count)].LevelID;
                            if (excludedLevelIDsAfterGameFinished.Count > 0)
                                selectedLevelID = GetRandomLevelID();
                            //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                            _currentLevelID = selectedLevelID;
                        }

                        //SetLastRandomLevelID(_currentLevel.LevelID);
                        SetLastRandomLevelID(_currentLevelID);
                        if (openLevelImmediately) OpenCurrentLevel();
                    }
                    else
                    {
                        int selectedID = levelOrder.First().LevelID;
                        if (excludedLevelIDsAfterGameFinished.Count > 0)
                            selectedID = GetFirstNonExcludedLevelID();

                        //_currentLevel = _levels.Find(level => level.LevelID == selectedID);
                        _currentLevelID = selectedID;
                        //SetLastRandomLevelID(_currentLevel.LevelID);
                        SetLastRandomLevelID(_currentLevelID);
                        if (openLevelImmediately) OpenCurrentLevel();
                    }
                }
                else // If player not finished the game.
                {
                    int lastLevelID = GetLastLevelID();

                    if (levelOrder.Exists(lo => lo.LevelID == lastLevelID))
                    {
                        if(!returnToStartWhenGameFinished)
                            CheckNewlyAddedOrNotPlayedLevelsAndSave(levelOrder.FindIndex(lo => lo.LevelID == lastLevelID));

                        //_currentLevel = _levels.Find(level => level.LevelID == lastLevelID);
                        _currentLevelID = lastLevelID;
                        if(openLevelImmediately) OpenCurrentLevel();
                    }
                    else
                    {
                        if (!returnToStartWhenGameFinished)
                        {
                            CheckNewlyAddedOrNotPlayedLevelsAndSave();
                            if (_tempLevelOrderForNewlyAddedOrNotPlayedLevels.Count > 0)
                            {
                                //_currentLevel = _levels.Find(level => level.LevelID == _tempLevelOrderForNewlyAddedOrNotPlayedLevels[0]);
                                //SetLastLevelID(_currentLevel.LevelID);

                                _currentLevelID = _tempLevelOrderForNewlyAddedOrNotPlayedLevels[0];
                                SetLastLevelID(_currentLevelID);
                            }
                            else
                            {
                                int selectedLevelID = GetRandomLevelID();
                                //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                                //lastRandomLevelID = _currentLevel.LevelID;
                                //SetLastRandomLevelID(_currentLevel.LevelID);
                                _currentLevelID = selectedLevelID;
                                lastRandomLevelID = _currentLevelID;
                                SetLastRandomLevelID(_currentLevelID);
                            }

                            if (openLevelImmediately) OpenCurrentLevel();
                        }
                        else
                        {
                            int selectedLevelID = levelOrder.First().LevelID;
                            if (excludedLevelIDsAfterGameFinished.Count > 0)
                                selectedLevelID = GetFirstNonExcludedLevelID();
                            //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                            //SetLastLevelID(_currentLevel.LevelID);
                            _currentLevelID = selectedLevelID;
                            SetLastLevelID(_currentLevelID);
                            if (openLevelImmediately) OpenCurrentLevel();
                        }
                    }
                }

                SetLevelOrder(levelOrder);
                SaveData();
            }
            else
            {
                //_currentLevel = _levels.Find(level => level.LevelID == GetLastLevelID());
                _currentLevelID = GetLastLevelID();
                SaveData();
                OpenCurrentLevel();
            }
        }
    }

    public void ChangeToNextLevel(bool changeLevelImmediately)
    {
        IncreaseLevelCounter();
        //AddLevelToCompletedLevels(_currentLevel.LevelID);
        //RemoveLevelFromNewOrOrderChangedLevels(_currentLevel.LevelID);
        AddLevelToCompletedLevels(_currentLevelID);
        RemoveLevelFromNewOrOrderChangedLevels(_currentLevelID);
        SaveData();

        int lastRandomLevelID = GetLastRandomLevelID();
        int lastLevelID = GetLastLevelID();
        int previousLevelID = GetPreviousLevelID();

        List<int> newOrOrderChangedLevels = GetNewOrOrderChangedLevels();

        if (lastRandomLevelID >= 0)
        {
            if (!returnToStartWhenGameFinished)
            {
                //SetPreviousLevelID(_currentLevel.LevelID);
                //previousLevelID = _currentLevel.LevelID;
                SetPreviousLevelID(_currentLevelID);
                previousLevelID = _currentLevelID;

                if (newOrOrderChangedLevels.Count > 0)
                {
                    //_currentLevel = _levels.Find(level => level.LevelID == newOrOrderChangedLevels[0]);
                    _currentLevelID = newOrOrderChangedLevels[0];
                }
                else
                {
                    int selectedLevelID = GetRandomLevelID(previousLevelID);
                    //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                    _currentLevelID = selectedLevelID;
                }

                //SetLastRandomLevelID(_currentLevel.LevelID);
                SetLastRandomLevelID(_currentLevelID);
                SaveData();

                if (changeLevelImmediately) OpenCurrentLevel();
            }
            else
            {
                SetPreviousLevelID(_currentLevelID);

                if (/*_currentLevel.LevelID*/_currentLevelID == levelOrder.Last().LevelID)
                {
                    int selectedLevelID = levelOrder.First().LevelID;
                    if (excludedLevelIDsAfterGameFinished.Count > 0)
                        selectedLevelID = GetFirstNonExcludedLevelID();
                    //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                    _currentLevelID = selectedLevelID;
                }
                else
                {
                    int currentLevelIndex = levelOrder.FindIndex(lo => lo.LevelID == /*_currentLevel.LevelID*/_currentLevelID);
                    if (excludedLevelIDsAfterGameFinished.Count <= 0)
                    {
                        //_currentLevel = _levels.Find(level => level.LevelID == levelOrder[currentLevelIndex + 1].LevelID);
                        _currentLevelID = levelOrder[currentLevelIndex + 1].LevelID;
                    }
                    else
                    {
                        int selectedLevelID = GetFirstNonExcludedLevelID(currentLevelIndex + 1);
                        //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                        _currentLevelID = selectedLevelID;
                    }
                }

                //SetLastRandomLevelID(_currentLevel.LevelID);
                //SetLastLevelID(_currentLevel.LevelID);
                SetLastRandomLevelID(_currentLevelID);
                SetLastLevelID(_currentLevelID);

                SaveData();

                if (changeLevelImmediately) OpenCurrentLevel();
            }
        }
        else
        {
            if (newOrOrderChangedLevels.Count > 0)
            {
                //SetPreviousLevelID(_currentLevel.LevelID);
                //_currentLevel = _levels.Find(level => level.LevelID == newOrOrderChangedLevels[0]);
                //SetLastLevelID(_currentLevel.LevelID);
                SetPreviousLevelID(_currentLevelID);
                _currentLevelID = newOrOrderChangedLevels[0];
                SetLastLevelID(_currentLevelID);
                SaveData();

                if (changeLevelImmediately) OpenCurrentLevel();
            }
            else
            {
                if (levelOrder.Last().LevelID ==/* _currentLevel.LevelID*/_currentLevelID)
                {
                    //SetPreviousLevelID(_currentLevel.LevelID);
                    SetPreviousLevelID(_currentLevelID);

                    if (!returnToStartWhenGameFinished)
                    {
                        int selectedLevelID = GetRandomLevelID(previousLevelID);
                        //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                        _currentLevelID = selectedLevelID;
                    }
                    else
                    {
                        int selectedLevelID = levelOrder.First().LevelID;
                        if (excludedLevelIDsAfterGameFinished.Count > 0)
                            selectedLevelID = GetFirstNonExcludedLevelID();
                        //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                        //SetLastLevelID(_currentLevel.LevelID);
                        _currentLevelID = selectedLevelID;
                        SetLastLevelID(_currentLevelID);
                    }

                    //SetLastRandomLevelID(_currentLevel.LevelID);
                    SetLastRandomLevelID(_currentLevelID);
                    SaveData();

                    if (changeLevelImmediately) OpenCurrentLevel();
                }
                else
                {
                    int nextLevelIndex = levelOrder.FindIndex(l => l.LevelID == /*_currentLevel.LevelID*/_currentLevelID) + 1;
                    for (int i = nextLevelIndex; i < levelOrder.Count; i++)
                    {
                        // if (excludedLevelIDsAfterGameFinished.Count > 0 && excludedLevelIDsAfterGameFinished.Contains(levelOrder[i].LevelID)) continue;

                        if (!IsLevelCompleted(levelOrder[i].LevelID))
                        {
                            //SetPreviousLevelID(_currentLevel.LevelID);
                            //_currentLevel = _levels.Find(level => level.LevelID == levelOrder[i].LevelID);
                            //SetLastLevelID(_currentLevel.LevelID);
                            SetPreviousLevelID(_currentLevelID);
                            _currentLevelID = levelOrder[i].LevelID;
                            SetLastLevelID(_currentLevelID);
                            SaveData();

                            if (changeLevelImmediately) OpenCurrentLevel();

                            return;
                        }
                    }

                    //SetPreviousLevelID(_currentLevel.LevelID);
                    SetPreviousLevelID(_currentLevelID);

                    if (!returnToStartWhenGameFinished)
                    {
                        int selectedLevelID = GetRandomLevelID(previousLevelID);
                        //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                        _currentLevelID = selectedLevelID;
                    }
                    else
                    {
                        int selectedLevelID = levelOrder.First().LevelID;
                        if (excludedLevelIDsAfterGameFinished.Count > 0)
                            selectedLevelID = GetFirstNonExcludedLevelID();
                        //_currentLevel = _levels.Find(level => level.LevelID == selectedLevelID);
                        //SetLastLevelID(_currentLevel.LevelID);
                        _currentLevelID = selectedLevelID;
                        SetLastLevelID(_currentLevelID);
                    }

                    //SetLastRandomLevelID(_currentLevel.LevelID);
                    SetLastRandomLevelID(_currentLevelID);
                    SaveData();

                    if (changeLevelImmediately) OpenCurrentLevel();
                }
            }
        }
    }

    #region Helper Methods

    private void CheckNewlyAddedOrNotPlayedLevelsAndSave(int toIndex = -1)
    {
        _tempLevelOrderForNewlyAddedOrNotPlayedLevels.Clear();
        for (int i = 0; i < (toIndex < 0 ? levelOrder.Count : toIndex); i++)
        {
            if (excludedLevelIDsAfterGameFinished.Count > 0 && excludedLevelIDsAfterGameFinished.Contains(levelOrder[i].LevelID)) continue;

            if (!IsLevelCompleted(levelOrder[i].LevelID))
                _tempLevelOrderForNewlyAddedOrNotPlayedLevels.Add(levelOrder[i].LevelID);
        }
        AddNewOrOrderChangedLevels(_tempLevelOrderForNewlyAddedOrNotPlayedLevels);
    }

    private int GetRandomLevelID(int previousLevelID)
    {
        _tempLevelOrderForSelectingRandomLevel.Clear();
        for (int i = 0; i < levelOrder.Count; i++)
        {
            if (levelOrder[i].LevelID == previousLevelID || levelOrder[i].LevelID == /*_currentLevel.LevelID*/_currentLevelID
                || (excludedLevelIDsAfterGameFinished.Count > 0 && excludedLevelIDsAfterGameFinished.Contains(levelOrder[i].LevelID)))
            {
                continue;
            }

            _tempLevelOrderForSelectingRandomLevel.Add(levelOrder[i].LevelID);
        }

        int selectedLevelID = 0;
        if (_tempLevelOrderForSelectingRandomLevel.Count <= 0)
        {
            if (excludedLevelIDsAfterGameFinished.Count > 0)
            {
                selectedLevelID = previousLevelID;
            }
            else
                selectedLevelID = levelOrder[Random.Range(0, levelOrder.Count)].LevelID;
        }
        else
            selectedLevelID = _tempLevelOrderForSelectingRandomLevel[Random.Range(0, _tempLevelOrderForSelectingRandomLevel.Count)];

        return selectedLevelID;
    }

    private int GetRandomLevelID()
    {
        _tempLevelOrderForSelectingRandomLevel.Clear();
        for (int i = 0; i < levelOrder.Count; i++)
        {
            if (excludedLevelIDsAfterGameFinished.Contains(levelOrder[i].LevelID))
                continue;

            _tempLevelOrderForSelectingRandomLevel.Add(levelOrder[i].LevelID);
        }

        int selectedLevelID = 0;
        if (_tempLevelOrderForSelectingRandomLevel.Count <= 0)
        {
            selectedLevelID = levelOrder[Random.Range(0, levelOrder.Count)].LevelID;
        }
        else
            selectedLevelID = _tempLevelOrderForSelectingRandomLevel[Random.Range(0, _tempLevelOrderForSelectingRandomLevel.Count)];

        return selectedLevelID;
    }

    public int GetFirstNonExcludedLevelID(int startIndex = 0)
    {
        for (int i = startIndex; i < levelOrder.Count; i++)
        {
            if (!excludedLevelIDsAfterGameFinished.Contains(levelOrder[i].LevelID))
            {
                return levelOrder[i].LevelID;
            }
        }

        if(startIndex > 0)
        {
            for (int i = 0; i < startIndex; i++)
            {
                if (!excludedLevelIDsAfterGameFinished.Contains(levelOrder[i].LevelID))
                {
                    return levelOrder[i].LevelID;
                }
            }
        }

        return levelOrder.First().LevelID;
    }

    #endregion

    #region Setter Methods
    public void SetLevels(List<Level> levels)
    {
        _levels.Clear();
        _levels.AddRange(levels);
    }

    public void SetStartWithStartLevel(bool value, int startLevelID)
    {
        this.startLevelID = startLevelID;
        startWithStartLevel = value;
    }

    public void SetReturnToStartWhenGameFinished(bool value)
    {
        returnToStartWhenGameFinished = value;
    }

    public void SetExcludedLevelIDs(List<int> excludedLevelIds)
    {
        excludedLevelIDsAfterGameFinished.Clear();
        excludedLevelIDsAfterGameFinished.AddRange(excludedLevelIds);
    }

    #endregion

    #region Methods For Saving Level Data And Getting Level Data From Save

    protected abstract void AddNewOrOrderChangedLevels(List<int> newOrOrderChangedLevelIDs);
    protected abstract void SetLevelOrder(List<LevelOrderIndex> levelOrder);
    protected abstract void SetLastLevelID(int lastLevelID);
    protected abstract void SetPreviousLevelID(int previousLevelID);
    protected abstract void SetLastRandomLevelID(int lastRandomLevelID);

    protected abstract bool IsFirstPlay();
    protected abstract void SetIsFirstPlay();

    protected abstract List<LevelOrderIndex> GetLevelOrderFromSaveData();
    protected abstract int GetLastRandomLevelID();
    protected abstract int GetLastLevelID();
    protected abstract int GetPreviousLevelID();
    protected abstract List<int> GetNewOrOrderChangedLevels();

    protected abstract void IncreaseLevelCounter();

    protected abstract bool IsLevelCompleted(int levelID);
    protected abstract void AddLevelToCompletedLevels(int levelID);

    protected abstract void ClearNewOrOrderChangedLevelsFromSaveData();
    protected abstract void RemoveLevelFromNewOrOrderChangedLevels(int levelID);

    protected virtual void SaveData()
    {
        // You can save all the data in once here
    }

    #endregion
}
