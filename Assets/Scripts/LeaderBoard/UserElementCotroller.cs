using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserElementCotroller : MonoBehaviour
{
    public TextMeshProUGUI orderInfo;
    public TextMeshProUGUI nameInfo;
    public TextMeshProUGUI moneyInfo;
    public Image border;
    public bool isYou = false;

    private Tween _tapToContinueTween;

    private void Start()
    {
        // if (isYou)
        // {
        //     _tapToContinueTween = transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine)
        //         .SetLoops(-1, LoopType.Yoyo);
        // }
       
    }
    
    public void SetOrder(int order, int textOrder = 0, bool isMe = false)
    {
        isYou = isMe;
      
        orderInfo.text = "#" + (order + textOrder+1);
        if (isYou)
        {
            _tapToContinueTween = transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            nameInfo.text = "You";
            border.transform.localScale = Vector3.one * 1.1f;
            border.color = new Color(0.6602019f, 1, 0, 1);
            SetMoney(SaveManager.Instance.GetTotalSpentAmount());
        }
        else
        {
            nameInfo.text = People.GetName((order + (textOrder > 0 ? (textOrder - 1) : (textOrder))) % People.peopleNames.Length);
            border.transform.localScale = Vector3.one;
            border.color = Color.white;
           // SetMoney(Mathf.Max(0, (20000-(order+textOrder)*220)));
            SetMoney(Mathf.Max(0, (200000-(order+textOrder)*220)));
        }
    }


    void SetMoney(int money)
    {
        moneyInfo.text = money + "$";
    }

}
