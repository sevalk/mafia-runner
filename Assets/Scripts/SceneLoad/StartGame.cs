using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
   
    void Start()
    {
        LevelManager.Instance.CheckLevelChangesAndChangeLevel();
    }

   
}
