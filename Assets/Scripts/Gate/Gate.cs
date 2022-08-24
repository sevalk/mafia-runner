using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using udoEventSystem;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [HideInInspector] public GangManager gangManager;
    [HideInInspector] public int price;
    public Transform visual;
    [Header("Gate Price Edit")]
    public int gatePrice;
    public bool useGatePrice;
    
    
    [Header("Price Text"), Space(10)]
    public TMP_Text priceText;

    private void Start()
    {
        gangManager = FindObjectOfType<GangManager>();
         priceText.text = price.ToString();
    }

    private void OnEnable()
    {
        EventManager.Get<GatePriceCalculated>().AddListener(CalculatePrice);
        EventManager.Get<PriceColorChanged>().AddListener(CheckMoneyAndChangePriceColor);
    }

    private void OnDisable()
    {
        EventManager.Get<GatePriceCalculated>().RemoveListener(CalculatePrice);
        EventManager.Get<PriceColorChanged>().RemoveListener(CheckMoneyAndChangePriceColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerParent>())
        {
            GateManager.Instance.GatePassed();
            if (IsGateAffordable())
            {
                VibrateManager.Instance.Vibrate(HapticTypes.MediumImpact);
                TriggerEnter(other);
            }
            
        }
       
    }

    public virtual void TriggerEnter(Collider other)
    {
        //print("Gate" + price);
        GetEventCost();
        //GateManager.Instance._gates.Remove(this);
        visual.DOScale(Vector3.one * 0.1f, 0.2f).OnComplete((() =>
        {
            GateManager.Instance._gates.Remove(this);
            visual.gameObject.SetActive(false);
        }));

    }
    private void GetEventCost()
    { 
       // playerData.SpendMoney(price);
       Player.Instance.SpendMoney(price , true);
      // EventManager.Get<PriceColorChanged>().Execute();
    }

    public virtual void CalculatePrice()
    {
        priceText.text = price.ToString() + " $";
        EventManager.Get<PriceColorChanged>().Execute();
    }

    private bool IsGateAffordable()
    {
        return Player.Instance.GetMoneyAmount() >= price;
    }

    private void CheckMoneyAndChangePriceColor()
    {
        //para aldığımızda, para harcadığımızda,price ı baştan hesapladığımızda
        
        //veya update de ama expensive
        
        if (IsGateAffordable())
        {
            //color green
            priceText.transform.parent.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            //color red
            priceText.transform.parent.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    
}
