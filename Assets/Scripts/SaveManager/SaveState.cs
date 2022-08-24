using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
public class SaveState
{
    // Level Manager Related Properties
    public bool isFirstPlay;
    public int lastLevelID;
    public int previousLevelID;
    public int lastRandomLevelID;
    public int levelCounter;
    public List<int> newOrOrderChangedLevels = new List<int>();
    public List<LevelOrderIndex> levelOrder = new List<LevelOrderIndex>();
    public List<int> completedLevels = new List<int>();
    public string levelOrderText;
   
    public bool isTutorialShown;
    public int currentCurrencyAmount;
    public int gemCount;
    public int totalSpent;
    public int order;
    public int popularity;
    public int popularityLevel;
    public bool isHapticActive;
    public int activeRing;
    public void LoadPreCreatedSaveState()
    {
        isFirstPlay = true;
        lastRandomLevelID = -1;
        levelCounter = 1;
        gemCount = 0;
        totalSpent = 0;
        isTutorialShown = false;
        popularityLevel = 0;
        popularity = 0;
        isHapticActive = true;
        activeRing = -1;
        order = UnityEngine.Random.Range(990, 1900);
    }
}