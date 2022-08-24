using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardController : Panel
{
   const int MAX_MONEY = 200000;
    public List<UserElementCotroller> userElements;

    [SerializeField]private GameObject tapToContinueButton;
    private Tween _tapToContinueTween;
    
    
    public Action OnTapToContinue;
    
    private void Awake()
    {
        tapToContinueButton.GetComponent<Button>().onClick.AddListener(OnTapToContinueButtonClicked);
    }
    
    public void OnEnable()
    {
        SaveManager.Instance.SetTotalSpentAmount(SaveManager.Instance.GetTotalSpentAmount() +
                                                 (Player.Instance.MoneySpentAmountAtCurrentLevel * Player.Instance.endingBridgeLevel));
       
        
        _tapToContinueTween = tapToContinueButton.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        
        ShowData(SaveManager.Instance.GetTotalSpentAmount() , i =>
        {
            if (i == 0) i = 1;
            SaveManager.Instance.SetOrder(i);
        });
    }
    

    
    
    public void OnTapToContinueButtonClicked()
    {
        _tapToContinueTween?.Kill();
        tapToContinueButton.GetComponent<Button>().interactable = false;
        //OnTapToContinue?.Invoke();
       LevelManager.Instance.ChangeToNextLevel(true);
      // LevelManager.Instance.OpenCurrentLevel();
       // UIManager.Instance.failPanel.OnTapToRetry?.Invoke();
    }
    
    
    
    public void ShowData(int totalMoney, Action<int> endEvent)
    {
        
        // SetMainPlayerOrder(SaveManager.Instance.GetOrder());
        
        int diff =Mathf.Max(0, MAX_MONEY - totalMoney);
        
        int targetOrder = Mathf.CeilToInt(diff/ 220f);
        StartCoroutine(IncreaseOrder(targetOrder , endEvent));
       
    }

    IEnumerator IncreaseOrder(int targetOrder, Action<int> endEvent)
    {
        float delayTime = 0.02f;
        int playerOldOrder = SaveManager.Instance.GetOrder();
        int increment =   playerOldOrder-targetOrder;
        int localIncrement = 1;
        

        
        while (increment > 0)
        {
            if (increment > 100)
            {
                localIncrement = 100;
                delayTime = 0.02f;
            }
            else if (increment > 10)
            {
                localIncrement = 10;
                delayTime = 0.04f;
            }
            else
            {
                localIncrement = 1;
                delayTime = 0.06f;
            }
            increment -= localIncrement;

            yield return new WaitForSeconds(delayTime*1.5f);
            int playerNewOrder = Mathf.Max(0, playerOldOrder - localIncrement);
            playerOldOrder = playerNewOrder;
            SetMainPlayerOrder(playerNewOrder);
            
           
        }
        SetMainPlayerOrder(targetOrder);
        yield return new WaitForSeconds(1f);
        endEvent?.Invoke(targetOrder);
        tapToContinueButton.SetActive(true);
    }

    public void SetMainPlayerOrder(int order)
    {
        if (order > 1)
        {
            userElements[0].SetOrder(order, -2);
            userElements[1].SetOrder(order, -1);
            userElements[2].SetOrder(order, 0, true);
            userElements[3].SetOrder(order, +1);
            userElements[4].SetOrder(order, +2);
            userElements[5].SetOrder(order, +3);
        }
        else if (order == 1)
        {
            userElements[0].SetOrder(order, -1);
            userElements[1].SetOrder(order, 0, true);
            userElements[2].SetOrder(order, +1);
            userElements[3].SetOrder(order, +2);
            userElements[4].SetOrder(order, +3);
            userElements[5].SetOrder(order, +4);
        }
        else
        {
            userElements[0].SetOrder(order, 0, true);
            userElements[1].SetOrder(order, +1);
            userElements[2].SetOrder(order, +2);
            userElements[3].SetOrder(order, +3);
            userElements[4].SetOrder(order, +4);
            userElements[5].SetOrder(order, +5);
        }
    }
}
