using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using MoreMountains.NiceVibrations;
using udoEventSystem;
using UnityEngine;

public class MoneyCollectible : Collectible
{
    public int amount = 10;

    private void Start()
    {
         transform.GetChild(0).DOLocalRotate(new Vector3(0, 0, 360), 1, RotateMode.LocalAxisAdd).SetLoops(-1)
             .SetEase(Ease.Linear);
    }

    public override void TriggerEnter(Collider other)
    {
        
            Player.Instance.CollectMoney(amount);
            VibrateManager.Instance.Vibrate(HapticTypes.LightImpact);
           // Taptic.Light();
         //   EventManager.Get<PriceColorChanged>().Execute();
    }
}
