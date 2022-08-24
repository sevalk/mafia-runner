using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelCompletedPanel : Panel
{
    [Header("Level Completed Panel Settings")] [SerializeField]
    public GameObject tapToContinueButton;

    private Tween _tapToContinueTween;
    public TextMeshProUGUI _CaseFiredText;
    public TextMeshProUGUI _GodfatherText;

    public Action OnTapToContinue;

    [SerializeField] protected List<CurrencyAmountHolder> currencyAmountHolders;
    [SerializeField] protected PanelWithCurrencyAmountsSettings panelSettings;

    private Dictionary<CurrencyAmountHolderID, List<GameObject>> _createdFlyingCurrencies =
        new Dictionary<CurrencyAmountHolderID, List<GameObject>>();

    [SerializeField] private RectTransform _initialTransformForFlyingCurrencies;

    private void Awake()
    {
        tapToContinueButton.GetComponent<Button>().onClick.AddListener(OnTapToContinueButtonClicked);
    }

    public void CreateFlyingCurrencies(CurrencyAmountHolderID holderID, CurrencyAnimationType animType, int finalAmount,
        Action<CurrencyAmountHolderID, bool> onAnimationCompleted = null)
    {
        var holder = currencyAmountHolders.Find(cah => cah.HolderID == holderID);
        if (holder == null || holder.IsAnimating)
            return;

        // holder.ClearCreatedFlyingCurrencies();
        ClearCreatedFlyingCurrencies(holderID);
        holder.SetIsAnimating(true);

        for (int i = 0; i < panelSettings.flyingCurrencyAmountToBeCreated; i++)
        {
            Vector2 randPosAround = (Vector2) _initialTransformForFlyingCurrencies.localPosition +
                                    Random.insideUnitCircle * panelSettings.randomPositionRadius;
            GameObject currencyImageGO = holder.GetFlyingCurrencyGameObject();
            currencyImageGO.transform.SetParent(_initialTransformForFlyingCurrencies.transform, false);
            currencyImageGO.transform.localScale = Vector3.one;
            currencyImageGO.transform.localPosition = Vector3.zero;

            
            var randomPosition = Random.insideUnitCircle * panelSettings.randomPositionRadius;
            var a = new Vector3(randomPosition.x, randomPosition.y, 0f);

            currencyImageGO.transform.DOMove(UIManager.Instance.leaderboard.transform.position + a, panelSettings.randomPositionMoveDuration)
                .SetDelay(Random.Range(panelSettings.randomPositionMoveDelayRange.x,
                    panelSettings.randomPositionMoveDelayRange.y))
                .SetEase(panelSettings.movingEase)
                .OnComplete(() =>
                {
                    currencyImageGO.transform.SetParent(holder.transform, true);
                    currencyImageGO.transform.localScale = Vector3.one;
                    DOTween.Sequence()
                        .Join(currencyImageGO.transform.DOMove(holder.CurrencyImage.rectTransform.position,
                            panelSettings.currencyImagePositionMoveDuration).SetSpeedBased())
                        .Join(currencyImageGO.transform.DOScale(panelSettings.flyingCurrencyImageReachScale,
                            panelSettings.currencyImagePositionMoveDuration))
                        .SetDelay(Random.Range(panelSettings.currencyImagePositionMoveDelayRange.x,
                            panelSettings.currencyImagePositionMoveDelayRange.y))
                        .SetEase(panelSettings.movingEase)
                        .OnComplete(() =>
                        {
                            // OnFlyingCurrencyReachThePosition(holder, currencyImageGO, animType, finalAmount,
                            //     onAnimationCompleted);
                        });
                    OnFlyingCurrencyReachThePosition(holder, currencyImageGO, animType, finalAmount,
                        onAnimationCompleted);
                });
            // holder.AddCreatedFlyingCurrency(currencyImageGO);
            AddCreatedFlyingCurrency(holderID, currencyImageGO);
        }
    }

    private void OnFlyingCurrencyReachThePosition(CurrencyAmountHolder holder, GameObject flyingCurrency,
        CurrencyAnimationType currencyAnimationType, int finalAmount,
        Action<CurrencyAmountHolderID, bool> onAnimationCompleted)
    {
        // int indexOfFlyingCurrency = holder.GetFlyingCurrencyIndex(flyingCurrency);
        int indexOfFlyingCurrency = _createdFlyingCurrencies[holder.HolderID].IndexOf(flyingCurrency);

        holder.AnimateCurrencyImage(panelSettings);

        if (indexOfFlyingCurrency == 0 && currencyAnimationType == CurrencyAnimationType.Counter)
        {
            holder.AnimateCounter(finalAmount, panelSettings, null);
        }

        if (indexOfFlyingCurrency >= _createdFlyingCurrencies[holder.HolderID].Count - 1)
        {
            if (currencyAnimationType == CurrencyAnimationType.Normal)
                holder.AnimateNormal(finalAmount, panelSettings, null);

            Debug.Log("Fly Completed!");
            onAnimationCompleted?.Invoke(holder.HolderID, true);
            holder.SetIsAnimating(false);
        }

        holder.DestroyFlyingCurrencyGameObject(flyingCurrency);
    }

    private void ClearCreatedFlyingCurrencies(CurrencyAmountHolderID currencyAmountHolderID)
    {
        if (_createdFlyingCurrencies.Count > 0 && _createdFlyingCurrencies[currencyAmountHolderID] != null &&
            _createdFlyingCurrencies[currencyAmountHolderID].Count > 0)
        {
            var currencyAmountHolder = GetCurrencyAmountHolder(currencyAmountHolderID);
            if (currencyAmountHolder != null)
            {
                foreach (var flyingCurrency in _createdFlyingCurrencies[currencyAmountHolderID])
                {
                    currencyAmountHolder.DestroyFlyingCurrencyGameObject(flyingCurrency);
                }

                _createdFlyingCurrencies[currencyAmountHolderID].Clear();
            }
        }
    }

    public CurrencyAmountHolder GetCurrencyAmountHolder(CurrencyAmountHolderID holderID)
    {
        return currencyAmountHolders.Find(cah => cah.HolderID == holderID);
    }

    private void AddCreatedFlyingCurrency(CurrencyAmountHolderID holderID, GameObject flyingCurrecyImageGO)
    {
        if (_createdFlyingCurrencies.ContainsKey(holderID))
            _createdFlyingCurrencies[holderID].Add(flyingCurrecyImageGO);
        else
            _createdFlyingCurrencies.Add(holderID, new List<GameObject>() {flyingCurrecyImageGO});
    }

    private void OnEnable()
    {
        ResetLevelCompletedPanel();
        _tapToContinueTween = tapToContinueButton.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnTapToContinueButtonClicked()
    {
        _tapToContinueTween?.Kill();
        tapToContinueButton.GetComponent<Button>().interactable = false;
       // OnTapToContinue?.Invoke();
       //LevelManager.Instance.OpenCurrentLevel();
       LevelManager.Instance.ChangeToNextLevel(true);
    }

    private void ResetLevelCompletedPanel()
    {
        // tapToContinueButton.interactable = true;
        _tapToContinueTween?.Kill();
        tapToContinueButton.transform.localScale = Vector3.one;
    }

    public void SetBannerText(string playerState)
    {
        _CaseFiredText.SetText(playerState);
    }
}