using System;
using System.Collections;
using System.Collections.Generic;
using AI.Combat;
using DG.Tweening;
using udoEventSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private int _moneyAmount;
    private int _moneyIndex;
    private int _collectedMoneyCount;
    public Transform spawnPoint;
    public Transform collectedMoneys;

    public GameObject trailMain;
    public GameObject trail2;
    public GameObject paint;
   // public GameObject paint;
  
    [HideInInspector]public bool isEndGame = false;
  
    
    private int _moneySpentAmountAtCurrentLevel = 0;
    public int MoneySpentAmountAtCurrentLevel => _moneySpentAmountAtCurrentLevel;


    [HideInInspector]public int endingBridgeLevel = 1;
    
    public void SetMoneySpentAmountAtCurrentLevel(int price)
    {
        _moneySpentAmountAtCurrentLevel += price;
        GetComponent<GangLeaderProgress>().CheckProgressAndChangeVisual();
    }
    // private void OnEnable()
    // {
    //     EventManager.Get<LevelCompleted>().AddListener(LevelComplete);
    //       
    // }
    //
    // private void OnDisable()
    // {
    //     EventManager.Get<LevelCompleted>().RemoveListener(LevelComplete);
    //        
    // }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
    }

    private void Start()
    {
        _collectedMoneyCount = collectedMoneys.childCount - 1;
        //paint.transform.DOMoveZ(spawnPoint.position.z, 1f);
        trailMain.transform.DOLocalMoveZ(12, 0.2f);
        trail2.transform.DOLocalMoveZ(12, 0.2f);

    }

    public int GetMoneyAmount()
    {
        return _moneyAmount;
    }

    public void SpendMoney(int spendAmount , bool isSpend)
    {
        bool x = true;
        _moneyAmount -= spendAmount;
        if(isSpend)
        {
            SetMoneySpentAmountAtCurrentLevel(spendAmount);
             UIManager.Instance.gamePanel.UpdateMoneyText(_moneySpentAmountAtCurrentLevel);

        }
        
        
        for (int i = 0; i < spendAmount/10; i++)
        {
            if (_moneyIndex == _collectedMoneyCount && x)
            {
                x = false;
                _moneyIndex++;
            }
            _moneyIndex--;
            _moneyIndex = Mathf.Clamp(_moneyIndex, 0, _collectedMoneyCount);
            collectedMoneys.GetChild(_moneyIndex).gameObject.SetActive(false);
        }
        EventManager.Get<PriceColorChanged>().Execute();
    }
   
    public void CollectMoney(int collectAmount)
    {
        _moneyAmount += collectAmount;
        for (int i = 0; i < collectAmount/10; i++)
        {
            collectedMoneys.GetChild(_moneyIndex).gameObject.SetActive(true);
            _moneyIndex++;
            _moneyIndex = Mathf.Clamp(_moneyIndex, 0, _collectedMoneyCount);
        }
        EventManager.Get<PriceColorChanged>().Execute();
    }

    public void DestroyAllMoneys()
    {
        collectedMoneys.transform.DOScale(0.1f, 0.1f).OnComplete((() =>
        {
            Destroy(collectedMoneys.gameObject);
            //puf particle
        }));
    }
    
    
}
