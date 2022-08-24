using System;
using System.Collections;
using System.Collections.Generic;
using AI.Control;
using DG.Tweening;
using UnityEngine;

public class GangLevelUpdateGate : Gate
{
    [Header("Gang Level Update Gate"), Space(10)]
    public int count;

    public GameObject gateName;

    private void Awake()
    {
        if (useGatePrice)
        {
            price = gatePrice;
        }
    }

    public override void TriggerEnter(Collider other)
    {
       
        if (other.GetComponent<PlayerParent>())
        {
            // adam sayısının karesi X level = maliyet.

            //price base çağırılmadan önce hesaplanmalı
           
            base.TriggerEnter(other);
            //print("level update gate");

            gateName.transform.parent = gangManager.gangLevelText.transform;
            gateName.transform.DOLocalJump(Vector3.zero, 6f, 1, 1f).OnComplete((() =>
            {
                gangManager.SetGangLevel(gangManager.GetGangLevel() + count);
                gateName.SetActive(false);
            }));


        }
    }
    
    public override void CalculatePrice()
    {
        if (!useGatePrice)
        {
            price = ((int)Math.Pow(GangManager.Instance.transform.childCount , 2) ) * (GangManager.Instance.GetGangLevel() + count);
            base.CalculatePrice();
        }
        
    }
}
