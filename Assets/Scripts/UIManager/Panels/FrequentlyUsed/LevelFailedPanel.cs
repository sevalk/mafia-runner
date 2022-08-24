using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Dreamteck;
using TMPro;

public class LevelFailedPanel : Panel
{
    [Header("Fail Panel Settings")]
    [SerializeField] private Button retryButton;

    private Tween _retryButtonTween;
    public Action OnTapToRetry;

    private void Awake()
    {
        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    private void OnEnable()
    {
        ResetFailedPanel();
        _retryButtonTween = retryButton.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
    
    

    private void OnRetryButtonClicked()
    {
        _retryButtonTween?.Kill();
        retryButton.interactable = false;
        OnTapToRetry.SafeInvoke();
    }
    
    private void ResetFailedPanel()
    {
        retryButton.interactable = true;
        _retryButtonTween?.Kill();
        retryButton.transform.localScale = Vector3.one;
    }
}
