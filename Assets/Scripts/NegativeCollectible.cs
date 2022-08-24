using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using udoEventSystem;
using UnityEngine;

public class NegativeCollectible : Collectible
{
    public int amount = 10;

    public override void TriggerEnter(Collider other)
    {
        Player.Instance.SpendMoney(amount , false);
        VibrateManager.Instance.Vibrate(HapticTypes.HeavyImpact);
       //EventManager.Get<PriceColorChanged>().Execute();
        
    }
}

