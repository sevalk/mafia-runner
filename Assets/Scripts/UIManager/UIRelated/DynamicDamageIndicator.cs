using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DynamicDamageIndicator : MonoBehaviour
{
    [SerializeField] private Image topDamageIndicator;
    [SerializeField] private Image bottomDamageIndicator;
    [SerializeField] private Image leftDamageIndicator;
    [SerializeField] private Image rightDamageIndicator;
    [SerializeField] private float damageIndicatorSize = 250f;
    [SerializeField] private float damageIndicatorMinIncreasePercentage = 1f;
    [SerializeField] private bool canUpdateDamageIndicatorPositions;
    
    private float _widthHeightOfDamageIndicators;
    private float _positionIncreaseAmountEachHit;
    
    private void Start()
    {
        _widthHeightOfDamageIndicators = topDamageIndicator.rectTransform.rect.height;
        _positionIncreaseAmountEachHit = _widthHeightOfDamageIndicators;
        ResetDamageIndicators(false);
    }
    
    private void Update()
    {
        if (!canUpdateDamageIndicatorPositions) return;
        UpdateDamageIndicatorPositions();
    }
    
    private void UpdateDamageIndicatorPositions()
    {
        if (topDamageIndicator.rectTransform.anchoredPosition.y < 0f)
        {
            topDamageIndicator.rectTransform.anchoredPosition = Vector2.Lerp(topDamageIndicator.rectTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 1f);
            bottomDamageIndicator.rectTransform.anchoredPosition = Vector2.Lerp(bottomDamageIndicator.rectTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 1f);
            leftDamageIndicator.rectTransform.anchoredPosition = Vector2.Lerp(leftDamageIndicator.rectTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 1f);
            rightDamageIndicator.rectTransform.anchoredPosition = Vector2.Lerp(rightDamageIndicator.rectTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 1f);
        }
    }
    
    [ContextMenu("OnDamageReceivedTest")]
    public void OnDamageReceived()
    {
        float ratioOfReceivedDamageToCurrentHealth = 1f;
        float damageIndicatorPositionIncreaseAmount = ratioOfReceivedDamageToCurrentHealth * damageIndicatorSize;
        float minIncreaseSize = damageIndicatorSize * damageIndicatorMinIncreasePercentage;

        float topDamageIndicatorMovePosY = Mathf.Clamp(topDamageIndicator.rectTransform.anchoredPosition.y - damageIndicatorPositionIncreaseAmount, damageIndicatorSize * -1f, minIncreaseSize * -1f);
        float bottomDamageIndicatorMovePosY = Mathf.Clamp(bottomDamageIndicator.rectTransform.anchoredPosition.y + damageIndicatorPositionIncreaseAmount, minIncreaseSize, damageIndicatorSize);

        float leftDamageIndicatorMovePosX = Mathf.Clamp(leftDamageIndicator.rectTransform.anchoredPosition.x + damageIndicatorPositionIncreaseAmount, minIncreaseSize, damageIndicatorSize);
        float rightDamageIndicatorMovePosX = Mathf.Clamp(rightDamageIndicator.rectTransform.anchoredPosition.x - damageIndicatorPositionIncreaseAmount, damageIndicatorSize * -1f, minIncreaseSize * -1f);

        topDamageIndicator.rectTransform.DOAnchorPosY(topDamageIndicatorMovePosY, 0.1f, true);
        bottomDamageIndicator.rectTransform.DOAnchorPosY(bottomDamageIndicatorMovePosY, 0.1f, true);
        leftDamageIndicator.rectTransform.DOAnchorPosX(leftDamageIndicatorMovePosX, 0.1f, true);
        rightDamageIndicator.rectTransform.DOAnchorPosX(rightDamageIndicatorMovePosX, 0.1f, true);
    }
    
    public void ResetDamageIndicators(bool withAnimation)
    {
        if (withAnimation)
        {
            topDamageIndicator.rectTransform.DOAnchorPos(Vector2.zero, 0.5f, true);
            bottomDamageIndicator.rectTransform.DOAnchorPos(Vector2.zero, 0.5f, true);
            leftDamageIndicator.rectTransform.DOAnchorPos(Vector2.zero, 0.5f, true);
            rightDamageIndicator.rectTransform.DOAnchorPos(Vector2.zero, 0.5f, true);
        }
        else
        {
            topDamageIndicator.rectTransform.anchoredPosition = Vector2.zero;
            bottomDamageIndicator.rectTransform.anchoredPosition = Vector2.zero;
            leftDamageIndicator.rectTransform.anchoredPosition = Vector2.zero;
            rightDamageIndicator.rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
